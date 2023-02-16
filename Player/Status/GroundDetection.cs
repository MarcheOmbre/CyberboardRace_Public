using Project.Scripts.Physics;
using Project.Scripts.Player.Controls;
using Project.Scripts.Player.Modifiers;
using Project.Scripts.Settings;
using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Player.Status
{
    public class GroundDetection : MonoBehaviour
    {
        public bool IsFlatGrounded { get; private set; }
        public RaycastHit? Hit { get; private set; }

        [SerializeField] private Rigidbody groundableRigidbody;
        [SerializeField] private RailSelector railSelector;
        [SerializeField] private Levitate levitate;

        private LevelSettings _levelSettings;
        private RaycastHelper _raycastHelper;
        private ATrack _track;

        private void Awake()
        {
            _levelSettings = FindObjectOfType<LevelSettings>();
            _raycastHelper = FindObjectOfType<RaycastHelper>();
            _track = FindObjectOfType<ATrack>();
        }

        private void FixedUpdate()
        {
            Hit = levitate.Hit;

            var currentRailData = _track.GetCurrentRailData(railSelector.CurrentRail);
            
            if (Hit == null)
            {
                if (UnityEngine.Physics.Raycast(groundableRigidbody.position, currentRailData.Rotation * Vector3.down,
                        out var raycastHit, _levelSettings.GeneralSettings.GroundedDistance, _raycastHelper.GroundableLayer))
                    Hit = raycastHit; 
            }
            
            var rigidbodyTrackDot = Vector3.Dot(groundableRigidbody.rotation * Vector3.up, currentRailData.Rotation * Vector3.up);
            IsFlatGrounded = Hit != null && Mathf.Abs(rigidbodyTrackDot) > _levelSettings.GeneralSettings.GroundedDirectionComparisonMinimumValue;
        }
    }
}