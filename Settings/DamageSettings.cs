using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts.Settings
{
    [CreateAssetMenu(fileName = "Damage_Settings", menuName = "Cyberboard/Settings/Damage", order = 1)]
    public class DamageSettings : ScriptableObject
    {
        //Slide
        public float SlideDamageMinDot => slideDamageMinDot;
        public float SlideDamage => slideDamage;
        public float InvincibilityTime => invincibilityTime;
        
        [Header("Invincibility")]
        [SerializeField] [Min(0)] private float invincibilityTime;
        
        [FormerlySerializedAs("rideDamageMinDot")]
        [Header("Slide")]
        [SerializeField] [Range(-1, 1)] private float slideDamageMinDot;
        [FormerlySerializedAs("rideDamages")] [SerializeField] [Min(0)] private float slideDamage;
    }
}
