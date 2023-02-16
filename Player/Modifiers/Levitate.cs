using Project.Scripts.Physics;
using Project.Scripts.Settings;
using UnityEngine;

namespace Project.Scripts.Player.Modifiers
{
    public class Levitate : MonoBehaviour
    {
        public RaycastHit? Hit { get; private set; }
        
        [SerializeField] private Rigidbody levitateRigidbody;
        [SerializeField] private GravityModifier gravityModifier;
        
        private LevelSettings _levelSettings;
        private RaycastHelper _raycastHelper;

        private void Awake()
        {
            _levelSettings = FindObjectOfType<LevelSettings>();
            _raycastHelper = FindObjectOfType<RaycastHelper>();
        }

        private void FixedUpdate()
        {
            Hit = null;
            
            var normalizedGravity = gravityModifier.CurrentGravity.normalized;
            
            //Check orientation
            if (Vector3.Dot(levitateRigidbody.rotation * Vector3.down, normalizedGravity) < _levelSettings.HandlingSettings.LevitateMinDot)
                return;
            
            var halfLevitationRange = _levelSettings.HandlingSettings.LevitateRange / 2;
            var minimumHeight = _levelSettings.HandlingSettings.LevitateHeight - halfLevitationRange;
            var maximumHeight = _levelSettings.HandlingSettings.LevitateHeight + halfLevitationRange;
            
            //Check distance
            if (!UnityEngine.Physics.Raycast(levitateRigidbody.position, normalizedGravity, out var raycastHit, maximumHeight, _raycastHelper.GroundLayer))
                return;

            Hit = raycastHit;
            
            //Gravity inversion
            levitateRigidbody.AddForce(-gravityModifier.CurrentGravity, ForceMode.Acceleration);

            var levitationDelta = Mathf.Clamp01((raycastHit.distance - minimumHeight) / _levelSettings.HandlingSettings.LevitateRange);
            levitationDelta = (levitationDelta - 0.5f) * 2f;
            var absoluteLevitationDelta = Mathf.Abs(levitationDelta);
            
            //Depth force
            var depthModifier = _levelSettings.HandlingSettings.LevitateDepthToForce.Evaluate(absoluteLevitationDelta) * _levelSettings.HandlingSettings.LevitateDepthForceModifier;
            levitateRigidbody.AddForce(normalizedGravity * depthModifier * levitationDelta, _levelSettings.HandlingSettings.LevitateForceMode);
            
            //DepthDrag
            var velocity = levitateRigidbody.velocity;
            var gravityDot = Vector3.Dot(velocity.normalized, normalizedGravity);
            var currentGravityVelocity = Mathf.Abs(gravityDot) * velocity.magnitude;
            var dragModifier = _levelSettings.HandlingSettings.LevitateDepthToDrag.Evaluate(absoluteLevitationDelta) * _levelSettings.HandlingSettings.LevitateDepthDragModifier;
            levitateRigidbody.AddForce(normalizedGravity * Mathf.Clamp(dragModifier, 0, currentGravityVelocity) * - gravityDot, _levelSettings.HandlingSettings.LevitateDragForceMode);
        }
    }
}
