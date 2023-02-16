using System;
using System.Collections;
using LDG.SoundReactor;
using Project.Scripts.Data;
using Project.Scripts.Data.Types;
using Project.Scripts.Shared;
using UnityEngine;

namespace Project.Scripts.Track
{
    public class TrackManager : MonoBehaviour
    {
        public event Action OnTrackFinished = delegate { };
        
        public bool IsPlaying => _trackProcess != null;
        
        [SerializeField] [Min(0)] private float midiStartTime;
        [SerializeField] private MidiSource midiSource;
        [SerializeField] private AudioSource audioSource;

        private Coroutine _trackProcess;
        private FloatData.RuntimeData _runtimeIntData;

        private void Awake()
        {
            _runtimeIntData = FindObjectOfType<LevelData>().FloatData[Constants.DataKeyTimeScale];
            OnValueChanged(_runtimeIntData.Value, _runtimeIntData.Value);
        }

        private void OnEnable()
        {
            _runtimeIntData.OnValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _runtimeIntData.OnValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(float oldValue, float value)
        {
            audioSource.pitch = value;
            midiSource.speed = value;
        }
        
        private IEnumerator TrackProcess()
        {
            audioSource.Play();
            midiSource.Play();
            
            //Set variables
            audioSource.time = 0;
            midiSource.time = midiStartTime;

            //Handle events
            yield return new WaitUntil(() => !audioSource.isPlaying);
            OnTrackFinished();

            Stop();
        }
        
        public void Play()
        {
            if (_trackProcess != null)
                return;

            _trackProcess = StartCoroutine(TrackProcess());
        }

        public void Stop()
        {
            if (_trackProcess != null)
            {
                StopCoroutine(_trackProcess);
                _trackProcess = null;
            }

            audioSource.Pause();
            midiSource.Pause();
        }
    }
}
