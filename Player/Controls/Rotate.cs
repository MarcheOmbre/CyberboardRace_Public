using System.Collections.Generic;
using Project.Scripts.Physics;
using Project.Scripts.Settings;
using UnityEngine;

namespace Project.Scripts.Player.Controls
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] private Rigidbody rotateRigidbody;

        private LevelSettings _levelSettings;
        private InputManager.InputManager _inputManager;
        private RaycastHelper _raycastHelper;
        
        private readonly Dictionary<int, Vector3> _fingersOnBoard = new Dictionary<int, Vector3>();

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
            _inputManager.OnSwipe += OnSwipe;
        }

        private void OnDisable()
        {
            _inputManager.OnSwipe -= OnSwipe;
            _inputManager.OnTouchEnd -= OnTouchEnd;
            _inputManager.OnTouchBegin -= OnTouchBegin;
        }

        private void OnTouchBegin(Touch touch)
        {
            var position = _inputManager.TouchesByFingerId[touch.fingerId].StartPosition;
            if (_raycastHelper.ScreenToRaycast(position, out var hit, _raycastHelper.PlayerInputLayer))
                _fingersOnBoard.Add(touch.fingerId, _raycastHelper.RaycastCamera.transform.InverseTransformDirection(hit.normal));
        }
        
        private void OnTouchEnd(Touch touch)
        {
            _fingersOnBoard.Remove(touch.fingerId);
        }
        
        
        private void OnSwipe(Touch touch, InputManager.InputManager.SwipeData swipeData)
        {
            if (!_fingersOnBoard.TryGetValue(touch.fingerId, out var normal))
                return;

            var swipeVector = (touch.position - swipeData.StartPosition) / Screen.width;

            //Get variables
            var distanceModifier = _levelSettings.HandlingSettings.RotateDistanceToForce.Evaluate(Mathf.Clamp01(swipeVector.magnitude));
            var timeModifier = _levelSettings.HandlingSettings.RotateTimeToForce.Evaluate(swipeData.TimeToMaxRatio);

            //Reset angular velocity
            rotateRigidbody.angularVelocity = Vector3.zero;
            
            //Compute torque
            var torque = Vector3.Cross(Vector3.back, swipeVector.normalized);

            rotateRigidbody.AddTorque
            (
                _raycastHelper.RaycastCamera.transform.rotation * Quaternion.FromToRotation(Vector3.back, normal) * 
                torque * distanceModifier * timeModifier * _levelSettings.HandlingSettings.RotateForceModifier,
                _levelSettings.HandlingSettings.RotateForceMode
            );
        }
    }
}
