using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts.Loading
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private Transition transition;
        [SerializeField] private bool fadeOutOnStart;
        
        private Coroutine _loadingCoroutine;

        private void Start()
        {
            if(fadeOutOnStart)
                _loadingCoroutine = StartCoroutine(transition.FadeProcess(OnFadeFinished, Transition.FadeType.FadedOut));
        }

        private void OnFadeFinished()
        {
            _loadingCoroutine = null;
        }
        
        private IEnumerator LoadSceneProcess(string scene)
        {
            yield return StartCoroutine(transition.FadeProcess(null, Transition.FadeType.FadedIn));
            
            SceneManager.LoadSceneAsync(scene);
        }
        
        public void LoadScene(string scene)
        {
            if (_loadingCoroutine != null)
                return;

            _loadingCoroutine = StartCoroutine(LoadSceneProcess(scene));
        }
    }
}
