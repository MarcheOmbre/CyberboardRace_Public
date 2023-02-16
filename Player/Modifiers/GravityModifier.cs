using Project.Scripts.Player.Controls;
using Project.Scripts.Settings;
using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Player.Modifiers
{
    public class GravityModifier : MonoBehaviour
    {
        public Vector3 CurrentGravity { get; private set; }
        
        [SerializeField] private Rigidbody gravityRigidbody;
        [SerializeField] private RailSelector railSelector;

        private LevelSettings _levelSettings;
        private ATrack _track;

        private void Awake()
        {
            _levelSettings = FindObjectOfType<LevelSettings>();
            _track = FindObjectOfType<ATrack>();
        }

        private void FixedUpdate()
        {
            var currenPoint = _track.GetCurrentRailData(railSelector.CurrentRail);
            CurrentGravity = currenPoint.Rotation * Vector3.down * _levelSettings.GeneralSettings.GravityModifier;
            gravityRigidbody.AddForce(CurrentGravity, ForceMode.Acceleration);
        }
    }
}