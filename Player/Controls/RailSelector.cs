using System;
using Project.Scripts.Settings;
using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Player.Controls
{
    public class RailSelector : MonoBehaviour
    {
        public int CurrentRail { get; private set; }

        private LevelSettings _levelSettings;
        private InputManager.InputManager _inputManager;
        private ATrack _track;

        private void Awake()
        {
            _levelSettings = FindObjectOfType<LevelSettings>();
            _inputManager = FindObjectOfType<InputManager.InputManager>();
            _track = FindObjectOfType<ATrack>();
        }

        private void OnEnable()
        {
            _inputManager.OnTap += OnTap;
        }

        private void OnDisable()
        {
            _inputManager.OnTap -= OnTap;
        }
        
        private void OnTap(Touch touch)
        {
            var tapScreenDistance = (touch.position.x/Screen.width - 0.5f) * 2;

            if (Mathf.Abs(tapScreenDistance) < _levelSettings.HandlingSettings.RailXDistanceFromMiddle)
                return;
            
            CurrentRail = Mathf.Clamp(CurrentRail + Math.Sign(tapScreenDistance), -_track.SideLineCount, _track.SideLineCount);
        }
    }
}
