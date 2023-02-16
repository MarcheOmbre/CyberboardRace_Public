using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.InputManager
{
    public class InputManager : MonoBehaviour
    {
        public struct TouchData
        {
            public TouchPhase Phase;
            public float StartTime;
            public Vector2 StartPosition;
        }

        public struct SwipeData
        {
            public float StartTime;
            public Vector2 StartPosition;
            public float TimeToMaxRatio;
        }
        
        private const float TapMaximumNormalizedDistance = 0.025f;
        private const float TapMaximumTime = 0.25f;
        private const float SwipeMinimumNormalizedDistance = 0.05f;
        private const float SwipeMaximumTime = 1f;

        public event Action<Touch> OnTouchBegin = delegate(Touch touch) { };
        public event Action<Touch> OnTouchStationary = delegate(Touch touch) { };
        public event Action<Touch> OnTouchMoved = delegate(Touch touch) { };
        public event Action<Touch> OnTouchEnd = delegate(Touch touch) { };
        public event Action<Touch> OnTap = delegate(Touch touch) { };
        public event Action<Touch, SwipeData> OnSwipe = delegate(Touch touch, SwipeData swipeData) { };

        public IReadOnlyDictionary<int, TouchData> TouchesByFingerId => _touchesByFingerId;
        private readonly Dictionary<int, TouchData> _touchesByFingerId = new Dictionary<int, TouchData>();

        private void Update()
        {
            foreach (var touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnBegin(touch);
                        break;
                    case TouchPhase.Ended:
                        OnEnd(touch);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                    case TouchPhase.Canceled:
                    default:
                        OnStateChanged(touch);
                        break;
                }
            }
        }

        private void OnBegin(Touch touch)
        {
            _touchesByFingerId.Add(touch.fingerId, new TouchData
            {
                Phase = touch.phase,
                StartTime = Time.time,
                StartPosition = touch.position
            });

            OnTouchBegin(touch);
        }

        private void OnStateChanged(Touch touch)
        {
            var touchData = _touchesByFingerId[touch.fingerId];
            if (touchData.Phase == touch.phase)
                return;

            if (touch.phase == TouchPhase.Stationary)
                OnTouchStationary(touch);
            else if (touch.phase == TouchPhase.Moved)
                OnTouchMoved(touch);
            else
                Debug.Log(_touchesByFingerId.ContainsKey(touch.fingerId));

            touchData.Phase = touch.phase;
            _touchesByFingerId[touch.fingerId] = touchData;
        }

        private void OnEnd(Touch touch)
        {
            var touchData = _touchesByFingerId[touch.fingerId];
            var touchTime = Time.time - touchData.StartTime;
            
            var swipeVector = touch.position - touchData.StartPosition;
            var screenNormalizedSwipe = new Vector2(swipeVector.x/Screen.width, swipeVector.y/Screen.height);
            var magnitude = screenNormalizedSwipe.magnitude;

            if (touchTime <= TapMaximumTime && magnitude <= TapMaximumNormalizedDistance)
            {
                OnTap(touch);
            }
            else if (touchTime <= SwipeMaximumTime && magnitude >= SwipeMinimumNormalizedDistance)
            {
                OnSwipe(touch, new SwipeData
                {
                    StartTime = touchData.StartTime,
                    StartPosition = touchData.StartPosition,
                    TimeToMaxRatio = touchTime / SwipeMaximumTime

                });
            }

            OnTouchEnd(touch);
            _touchesByFingerId.Remove(touch.fingerId);
        }
    }
}