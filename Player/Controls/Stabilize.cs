using System.Collections.Generic;
using Project.Scripts.Physics;
using Project.Scripts.Player.Modifiers;
using Project.Scripts.Settings;
using UnityEngine;

namespace Project.Scripts.Player.Controls
{
    public class Stabilize : MonoBehaviour
    {
        [SerializeField] private Rigidbody stabilizeRigidbody;
        [SerializeField] private GravityModifier gravityModifier;
        [SerializeField] private Levitate levitate;
        
        private LevelSettings _levelSettings;
        private InputManager.InputManager _inputManager;
        private RaycastHelper _raycastHelper;

        private readonly List<int> _fingersOnBoard = new List<int>();
        private Vector3 _hitNormal;
        private float _firstTouchTime;
        private float? _levitatingTime;

        private void Awake()
        {
            _levelSettings = FindObjectOfType<LevelSettings>();
            _inputManager = FindObjectOfType<InputManager.InputManager>();
            _raycastHelper = FindObjectOfType<RaycastHelper>();
        }

        private void OnEnable()
        {
            _inputManager.OnTouchBegin += OnTouchBegin;
            _inputManager.OnTouchEnd += OnTouchEnd;
        }

        private void OnDisable()
        {
            _inputManager.OnTouchEnd -= OnTouchEnd;
            _inputManager.OnTouchBegin -= OnTouchBegin;
        }

        private bool GetStabilization(out float time, out Vector3 normal)
        {
            time = 0;
            normal = Vector3.zero;
            
            //Check levitation
            if (levitate.Hit != null && _levitatingTime == null)
                _levitatingTime = Time.time;
            else if (levitate.Hit == null && _levitatingTime != null)
                _levitatingTime = null;
            
            if(_levitatingTime == null && _fingersOnBoard.Count == 0)
                return false;
            
            if (_levitatingTime != null)
            {
                time = _levitatingTime.Value;
                normal = stabilizeRigidbody.rotation * Vector3.up;
            }
            else
            {
                time = Time.time - _firstTouchTime;
                normal = stabilizeRigidbody.transform.TransformDirection(_hitNormal);
            }

            return true;
        }
        
        private void FixedUpdate()
        {
            if (!GetStabilization(out var time, out var normal))
                return;
            
            //Compute modifiers
            var stabilityModifier = _levelSettings.HandlingSettings.StabilizeTimeToStabilityForce.Evaluate(time) * _levelSettings.HandlingSettings.StabilizeStabilityModifier;
            var speedModifier = _levelSettings.HandlingSettings.StabilizeTimeToSpeedForce.Evaluate(time) * _levelSettings.HandlingSettings.StabilizeSpeedModifier;
            var angularVelocity = stabilizeRigidbody.angularVelocity;

            //Compute directions
            var predictedUp = Quaternion.AngleAxis(angularVelocity.magnitude * Mathf.Rad2Deg * stabilityModifier / speedModifier, angularVelocity) * normal;
            var torque = Vector3.Cross(predictedUp, -gravityModifier.CurrentGravity.normalized);
            
            //Apply force
            stabilizeRigidbody.AddTorque(torque * speedModifier * speedModifier, _levelSettings.HandlingSettings.StabilizeForceMode);
        }
        
        private void OnTouchBegin(Touch touch)
        {
            var position = _inputManager.TouchesByFingerId[touch.fingerId].StartPosition;
            if (!_raycastHelper.ScreenToRaycast(position, out var raycastHit, _raycastHelper.PlayerInputLayer))
                return;

            if (_fingersOnBoard.Count == 0)
            {
                _firstTouchTime = Time.time;
                stabilizeRigidbody.angularVelocity = Vector3.zero;
                _hitNormal = stabilizeRigidbody.transform.InverseTransformDirection(raycastHit.normal);
            }
            
            _fingersOnBoard.Add(touch.fingerId);
        }

        private void OnTouchEnd(Touch touch) => _fingersOnBoard.Remove(touch.fingerId);

    }
}
