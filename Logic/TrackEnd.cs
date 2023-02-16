using Project.Scripts.Track;
using UnityEngine;

namespace Project.Scripts.Logic
{
    public class TrackEnd : MonoBehaviour
    {
        [SerializeField] private Canvas menu;
        
        private TrackManager _trackManager;

        private void Awake()
        {
            _trackManager = FindObjectOfType<TrackManager>();
        }

        private void OnEnable()
        {
            _trackManager.OnTrackFinished += OnTrackFinished;
        }

        private void OnDisable()
        {
            _trackManager.OnTrackFinished -= OnTrackFinished;
        }

        private void OnTrackFinished()
        {
            menu.enabled = true;
        }
    }
}
