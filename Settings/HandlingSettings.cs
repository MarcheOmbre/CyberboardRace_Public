using UnityEngine;

namespace Project.Scripts.Settings
{
    [CreateAssetMenu(fileName = "Handling_Settings", menuName = "Cyberboard/Settings/Handling", order = 1)]
    public class HandlingSettings : ScriptableObject
    {
        //Rigidbody
        public float Mass => mass;
        public float Drag => drag;
        public float AngularDrag => angularDrag;
        public Vector3 InertiaTensor => inertiaTensor;
        public Vector3 InertiaTensorRotation => inertiaTensorRotation;

        //Levitate
        public float LevitateHeight => levitateHeight;
        public float LevitateRange => levitateRange;
        public float LevitateMinDot => levitateMinDot;
        public float LevitateDepthForceModifier => levitateDepthForceModifier;
        public AnimationCurve LevitateDepthToForce => levitateDepthToForce;
        public ForceMode LevitateForceMode => levitateForceMode;
        public float LevitateDepthDragModifier => levitateDepthDragModifier;
        public AnimationCurve LevitateDepthToDrag => levitateDepthToDrag;
        public ForceMode LevitateDragForceMode => levitateDragForceMode;
        
        //Rail
        public float RailXDistanceFromMiddle => railXDistanceFromMiddle;
        public float RailSlideForce => railSlideForce;
        public float RailFollowForce => railFollowForce;

        //Jump
        public float JumpForceModifier => jumpForceModifier;
        public AnimationCurve JumpDistanceToForce => jumpDistanceToForce;
        public AnimationCurve JumpTimeToForce => jumpTimeToForce;
        public ForceMode JumpForceMode => jumpForceMode;
        
        //Rotate
        public float RotateForceModifier => rotateForceModifier;
        public AnimationCurve RotateDistanceToForce => rotateDistanceToForce;
        public AnimationCurve RotateTimeToForce => rotateTimeToForce;
        public ForceMode RotateForceMode => rotateForceMode;
        
        //Stabilize
        public float StabilizeStabilityModifier => stabilizeStabilityModifier;
        public AnimationCurve StabilizeTimeToStabilityForce => stabilizeTimeToStabilityForce;
        public float StabilizeSpeedModifier => stabilizeSpeedModifier;
        public AnimationCurve StabilizeTimeToSpeedForce => stabilizeTimeToSpeedForce;
        public ForceMode StabilizeForceMode => stabilizeForceMode;


        [Header("Rigidbody")] 
        [SerializeField] [Min(0)] private float mass;
        [SerializeField] [Min(0)] private float drag;
        [SerializeField] [Min(0)] private float angularDrag;
        [SerializeField] [Min(0)] private Vector3 inertiaTensor;
        [SerializeField] [Min(0)] private Vector3 inertiaTensorRotation;

        [Header("Levitate")]
        [SerializeField][Min(0.001f)] private float levitateHeight;
        [SerializeField][Min(0.001f)] private float levitateRange;
        [SerializeField][Range(0, 0.5f)] private float levitateMinDot;
        [SerializeField][Min(0)] private float levitateDepthForceModifier;
        [SerializeField] private AnimationCurve levitateDepthToForce;
        [SerializeField] private ForceMode levitateForceMode;
        [SerializeField][Min(0)] private float levitateDepthDragModifier;
        [SerializeField] private AnimationCurve levitateDepthToDrag;
        [SerializeField] private ForceMode levitateDragForceMode;
        
        [Header("Rail")]
        [SerializeField] [Range(0, 1f)] private float railXDistanceFromMiddle;
        [SerializeField] [Min(0)] private float railSlideForce;
        [SerializeField] [Min(0)] private float railFollowForce;

        [Header("Jump")]
        [SerializeField] [Min(0)] private float jumpForceModifier;
        [SerializeField] private AnimationCurve jumpDistanceToForce;
        [SerializeField] private AnimationCurve jumpTimeToForce;
        [SerializeField] private ForceMode jumpForceMode;
        
        [Header("Rotate")]
        [SerializeField] [Min(0)] private float rotateForceModifier;
        [SerializeField] private AnimationCurve rotateDistanceToForce;
        [SerializeField] private AnimationCurve rotateTimeToForce;
        [SerializeField] private ForceMode rotateForceMode;
        
        [Header("Stabilize")]
        [SerializeField] [Min(0)] private float stabilizeStabilityModifier;
        [SerializeField] private AnimationCurve stabilizeTimeToStabilityForce;
        [SerializeField] [Min(0)] private float stabilizeSpeedModifier;
        [SerializeField] private AnimationCurve stabilizeTimeToSpeedForce;
        [SerializeField] private ForceMode stabilizeForceMode;
    }
}