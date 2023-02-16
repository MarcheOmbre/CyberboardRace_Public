using Project.Scripts.Player.Controls;
using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class TargetView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform modelTransform; //Used to avoid to loose position of Rigidbody when deactivated
        [SerializeField] private Rigidbody targetRigidbody;
        [SerializeField] private RailSelector railSelector;

        [Header("Configuration")]
        [SerializeField] private float distanceOffset;

        private ATrack _track;
        
        private float _normalizedTimeOffset;

        private void Awake()
        {
            _track = FindObjectOfType<ATrack>();
        }

        private void Start()
        {
            _normalizedTimeOffset = distanceOffset / _track.TotalLength;
        }

        private void FixedUpdate()
        {
            var currentRailData = _track.GetCurrentRailData(railSelector.CurrentRail);
            var targetRailData = _track.GetRailData(_track.GetSplineData(_track.Progression + _normalizedTimeOffset), railSelector.CurrentRail);
            
            //Current position
            var height = (Quaternion.Inverse(currentRailData.Rotation) * (modelTransform.transform.position - currentRailData.Position)).y;
            
            targetRigidbody.MovePosition(targetRailData.Position + currentRailData.Rotation * Vector3.up * height);
            targetRigidbody.MoveRotation(targetRailData.Rotation);
        }
    }
}
