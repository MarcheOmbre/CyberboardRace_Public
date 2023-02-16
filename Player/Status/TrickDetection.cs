using System;
using System.Collections.Generic;
using Project.Scripts.Player.Controls;
using Project.Scripts.Settings;
using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Player.Status
{
    public class TrickDetection : MonoBehaviour
    {
        public event Action<IEnumerable<TrickData>> OnTrick = delegate {  };
        
        [Flags]
        public enum Direction
        {
            Left = 1,
            Right = 2,
            Front = 4,
            Back = 8,
            Top = 16,
            Bottom = 32,
            LeftToRight = 64,
            RightToLeft = 128,
            Undefined = 0
        }
        
        public enum Type
        {
            Flip,
            Spin,
            Undefined
        }
        
        public struct TrickData
        {
            public Type Type;
            public Direction Direction;
            public int Half;
        }
        
        [SerializeField] private Rigidbody tricksRigidbody;

        private ATrack _track;
        private RailSelector _railSelector;
        private GroundDetection _groundDetection;
        private LevelSettings _levelSettings;
        
        private bool _trickFinished;
        private float _lastRecordTime;

        //Angle
        private Vector3 _currentLocalPivot;
        private Quaternion _lastRotation;
        private float _angleRecord;

        private readonly List<TrickData> _tricksLine = new List<TrickData>(); 

        private void Awake()
        {
            _track = FindObjectOfType<ATrack>();
            _groundDetection = FindObjectOfType<GroundDetection>();
            _railSelector = FindObjectOfType<RailSelector>();
            _levelSettings = FindObjectOfType<LevelSettings>();
        }

        private void Update()
        {
            var trackRotation = _track.GetCurrentRailData(_railSelector.CurrentRail).Rotation;
            
            //Refresh
            var isTickFinished = _groundDetection.IsFlatGrounded;
            
            if (_trickFinished != isTickFinished)
            {
                _trickFinished = isTickFinished;

                if (!_trickFinished)
                {
                    var rotation = tricksRigidbody.rotation;
                    _lastRotation = rotation;
                    _angleRecord = 0;
                }
                else
                {
                    TryAddTricksToLine(trackRotation);

                    if (_tricksLine.Count > 0)
                    {
                        OnTrick(_tricksLine);
                        _tricksLine.Clear();
                    }
                }
            }

            if (_trickFinished)
                return;

            ComputePivotAndAngles(trackRotation);
        }

        private void ComputePivotAndAngles(Quaternion trackRotation)
        {
            //Compute rotation
            var rigidbodyRotation = tricksRigidbody.rotation;
            (rigidbodyRotation * Quaternion.Inverse(_lastRotation)).ToAngleAxis(out var angle, out var pivot);
            _lastRotation = rigidbodyRotation;
     
            //Add angles
            var trackLocalPivot = Quaternion.Inverse(trackRotation) * pivot;
            if (Vector3.Dot(trackLocalPivot, _currentLocalPivot) < _levelSettings.ScoringSettings.RotationPivotSwitchOffset
                || tricksRigidbody.angularVelocity.magnitude < _levelSettings.ScoringSettings.RotationMinimumVelocity)
            {
                //Compute 
                if (_angleRecord > 0)
                {
                    TryAddTricksToLine(trackRotation);
                    _angleRecord = 0;
                }

                _currentLocalPivot = trackLocalPivot;
            }

            //Record
            _angleRecord += angle;
        }
        
        private void TryAddTricksToLine(Quaternion trackRotation)
        {
            Type type;
            var direction = Direction.Undefined;
            float dot;
            
            //Compute half count
            var half = (int)_angleRecord / 180;
            if (Mathf.Abs(_angleRecord % 180f / 180f) >= _levelSettings.ScoringSettings.RotationHalfFlipSensibility)
                half += Math.Sign(_angleRecord);
            if (half <= 0)
                return;
            
            //Spin
            if (Mathf.Abs(dot = Vector3.Dot(_currentLocalPivot, Vector3.up)) >= _levelSettings.ScoringSettings.RotationDirectionComparisonOffset)
            {
                type = Type.Spin;

                if (Vector3.Dot(Quaternion.Inverse(trackRotation) * tricksRigidbody.rotation * Vector3.up,
                        Vector3.up) < 0)
                {
                    direction = Direction.Bottom;
                    direction |= Mathf.Sign(dot) < 0 ? Direction.Right : Direction.Left;
                }
                else
                {
                    direction = Direction.Top;
                    direction |= Mathf.Sign(dot) < 0 ? Direction.Right : Direction.Left;
                }
            }
            //Flip
            else
            {
                type = Type.Flip;
                
                if (Mathf.Abs(dot = Vector3.Dot(_currentLocalPivot, Vector3.forward))
                    >= _levelSettings.ScoringSettings.RotationDirectionComparisonOffset)
                    direction = Mathf.Sign(dot) < 0 ? Direction.Right : Direction.Left;                
         
                else if(Mathf.Abs(dot = Vector3.Dot(_currentLocalPivot, Vector3.right)) 
                        >= _levelSettings.ScoringSettings.RotationDirectionComparisonOffset)
                    direction = Mathf.Sign(dot) < 0 ? Direction.Back : Direction.Front;
            
                else if (Mathf.Abs(dot = Vector3.Dot(_currentLocalPivot, (Vector3.right + Vector3.forward).normalized))
                         >= _levelSettings.ScoringSettings.RotationDirectionComparisonOffset)
                {
                    direction = Direction.LeftToRight;
                    direction |= Mathf.Sign(dot) < 0 ? Direction.Back : Direction.Front;   
                }

                else if (Mathf.Abs(dot = Vector3.Dot(_currentLocalPivot, (Vector3.left + Vector3.forward).normalized))
                         >= _levelSettings.ScoringSettings.RotationDirectionComparisonOffset)
                {
                    direction = Direction.RightToLeft;
                    direction |= Mathf.Sign(dot) < 0 ? Direction.Front : Direction.Back;
                }
            }

            _tricksLine.Add(new TrickData
            {
                Type = type,
                Direction = direction,
                Half = half
            });
        }
    }
}
