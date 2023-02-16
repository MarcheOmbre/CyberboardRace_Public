using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Track
{
    public class PhysicsTrackFollower : MonoBehaviour
    {
        [SerializeField] private Rigidbody followerRigidbody;

        private ATrack _track;

        private void Awake()
        {
            _track = FindObjectOfType<ATrack>();
        }

        private void FixedUpdate()
        {
            var spatialData = _track.CurrentSplineSpatialData;
            followerRigidbody.MovePosition(spatialData.Position);
            followerRigidbody.MoveRotation(spatialData.Rotation);
        }
    }
}
