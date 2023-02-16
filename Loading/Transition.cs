using System;
using System.Collections;
using UnityEngine;

namespace Project.Scripts.Loading
{
    public class Transition : MonoBehaviour
    {
        public enum FadeType
        {
            FadedIn,
            FadedOut
        }
        
        [SerializeField] private AnimationClip fadeInClip;
        [SerializeField] private AnimationClip fadeOutClip;
        
        private Animation _fadeAnimation;

        private void Awake()
        {
            _fadeAnimation = GetComponent<Animation>();
        }

        public IEnumerator FadeProcess(Action onFinished, FadeType fadeType)
        {
            _fadeAnimation.Play(fadeType == FadeType.FadedIn ? fadeInClip.name : fadeOutClip.name, PlayMode.StopAll);
            
            yield return new WaitUntil(() => !_fadeAnimation.isPlaying);
            onFinished?.Invoke();
        }

        public IEnumerator FadeProcessInOut(Action onFadedIn, Action onFadedOut)
        {
            yield return StartCoroutine(FadeProcess(onFadedIn, FadeType.FadedIn));
            
            yield return StartCoroutine(FadeProcess(onFadedOut, FadeType.FadedOut));
        }
    }
}
