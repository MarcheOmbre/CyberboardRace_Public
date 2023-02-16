using Project.Scripts.Player.Controls;
using Project.Scripts.Settings;
using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Player.Modifiers
{
    public class RailFollower : MonoBehaviour
    {
        [SerializeField] private Rigidbody behaviourRigidbody;
        [SerializeField] private RailSelector railSelector;
        
        private LevelSettings _levelSettings;
        private ATrack _track;

        private Vector3 _followVelocity;

        private void Awake()
        {
            _levelSettings = FindObjectOfType<LevelSettings>();
            _track = FindObjectOfType<ATrack>();
        }

        private void FixedUpdate()
        {
            var spatialData = _track.GetCurrentRailData(railSelector.CurrentRail);
            var inverseRotation = Quaternion.Inverse(spatialData.Rotation);
            
            var localVelocity = inverseRotation * behaviourRigidbody.velocity;
            var localPosition = inverseRotation * (behaviourRigidbody.position - spatialData.Position);

            //Velocity
            localVelocity.x = -localPosition.x * Time.fixedDeltaTime * _levelSettings.HandlingSettings.RailSlideForce;
            localVelocity.z = -localPosition.z * Time.fixedDeltaTime * _levelSettings.HandlingSettings.RailFollowForce;
            behaviourRigidbody.velocity = spatialData.Rotation * localVelocity;
        }
    }
}
