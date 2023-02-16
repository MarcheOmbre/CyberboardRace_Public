using System.Collections.Generic;
using Project.Scripts.Physics;
using Project.Scripts.Player.Modifiers;
using Project.Scripts.Player.Status;
using Project.Scripts.Settings;
using UnityEngine;

namespace Project.Scripts.Player.Controls
{
    public class Jump : MonoBehaviour
    {
        [SerializeField] private Rigidbody jumpRigidbody;
        [SerializeField] private GravityModifier gravityModifier;
        [SerializeField] private GroundDetection groundDetection;

        private LevelSettings _levelSettings;
        private InputManager.InputManager _inputManager;
        private RaycastHelper _raycastHelper;

        private readonly List<int> _fingersOnBoard = new List<int>();

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
            if (_raycastHelper.ScreenToRaycast(position, out _, _raycastHelper.PlayerInputLayer))
                _fingersOnBoard.Add(touch.fingerId);
        }

        private void OnTouchEnd(Touch touch)
        {
            _fingersOnBoard.Remove(touch.fingerId);
        }
        
        private void OnSwipe(Touch touch, InputManager.InputManager.SwipeData swipeData)
        {
            if (groundDetection.Hit == null || !_fingersOnBoard.Contains(touch.fingerId))
                return;
            
            var distanceModifier = _levelSettings.HandlingSettings.JumpDistanceToForce
                .Evaluate((touch.position - swipeData.StartPosition).magnitude / new Vector2(Screen.width, Screen.height).magnitude);
            var timeModifier = _levelSettings.HandlingSettings.JumpTimeToForce.Evaluate(swipeData.TimeToMaxRatio);
            
            jumpRigidbody.AddForce
            (
                -gravityModifier.CurrentGravity.normalized * distanceModifier * timeModifier
                * _levelSettings.HandlingSettings.JumpForceModifier, _levelSettings.HandlingSettings.JumpForceMode
            );
        }
    }
}