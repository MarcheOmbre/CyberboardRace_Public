using UnityEngine;

namespace Project.Scripts.Settings
{
    [CreateAssetMenu(fileName = "Track_Settings", menuName = "Cyberboard/Settings/Track", order = 1)]
    public class TrackSettings : ScriptableObject
    {
        public float MutualDistanceMultiplier => mutualDistanceMultiplier;
        
        [SerializeField][Min(0)] private float mutualDistanceMultiplier;
    }
}
