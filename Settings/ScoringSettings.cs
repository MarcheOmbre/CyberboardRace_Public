using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts.Settings
{
    [CreateAssetMenu(fileName = "Scoring_Settings", menuName = "Cyberboard/Settings/Scoring", order = 1)]
    public class ScoringSettings : ScriptableObject
    {
        //Trick detection
        public float RotationDirectionComparisonOffset => rotationDirectionComparisonOffset;
        public float RotationPivotSwitchOffset => rotationPivotSwitchOffset;
        public float RotationHalfFlipSensibility => rotationHalfFlipSensibility;
        public float RotationMinimumVelocity => rotationMinimumVelocity;

        //Managers
        public float ScoringLineDelay => scoringLineDelay;
        public float SlidingSideOffset => slidingSideOffset;
        public float ComboMultiplier => comboMultiplier;
        
        
        [Header("Trick detection")]
        [SerializeField][Range(0f, 1f)] private float rotationPivotSwitchOffset;
        [SerializeField][Range(0f, 1f)] private float rotationHalfFlipSensibility;
        [SerializeField][Range(0f, 1f)] private float rotationDirectionComparisonOffset;
        [SerializeField][Min(0)] private float rotationMinimumVelocity;
        
        [Header("Managers")] 
        [SerializeField] [Min(0)] private float scoringLineDelay;
        [SerializeField] [Min(0)] private float slidingSideOffset;
        [SerializeField] [Min(0)] private float comboMultiplier;
    }
}
