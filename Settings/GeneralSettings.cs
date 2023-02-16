using UnityEngine;

namespace Project.Scripts.Settings
{
    [CreateAssetMenu(fileName = "General_Settings", menuName = "Cyberboard/Settings/General", order = 1)]
    public class GeneralSettings : ScriptableObject
    {
        //Environment
        public float GravityModifier => gravityModifier;
        public float GroundedDistance => groundedDistance;
        public float GroundedDirectionComparisonMinimumValue => groundedDirectionComparisonMinimumValue;
        
        //Spawn
        public float TrackDistanceOffset => trackDistanceOffset;
        public float SpawnDistance => spawnDistance;
        public float DespawnDistance => despawnDistance;
        

        [Header("Ground")]
        [SerializeField]  private float gravityModifier;
        [SerializeField] [Min(0)] private float groundedDistance;
        [SerializeField][Range(0f, 1f)] private float groundedDirectionComparisonMinimumValue;
        
        [Header("Spawn")]
        [SerializeField] private float trackDistanceOffset;
        [SerializeField][Min(0)] private float spawnDistance;
        [SerializeField][Min(0)] private float despawnDistance;
    }
}