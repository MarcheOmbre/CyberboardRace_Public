using Dreamteck.Splines;
using UnityEngine;

namespace Project.Scripts.Track.Abstract
{
    public abstract class ATrack : MonoBehaviour
    {
        public struct SpatialData
        {
            public Vector3 Position;
            public Quaternion Rotation;
        }
        
        public float TotalLength { get; private set; }
        public float Progression { get; private set;  }
        public SpatialData CurrentSplineSpatialData { get; private set; }
        public int SideLineCount => sideLineCount;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private SplineComputer splineComputer;
        [SerializeField] [Min(1)] private int sideLineCount;

        
        protected virtual void Awake()
        {
            TotalLength = splineComputer.CalculateLength();

            RefreshCurrentPoint();
        }

        protected virtual void FixedUpdate()
        {
            RefreshCurrentPoint();
        }

        private void RefreshCurrentPoint()
        {
            if (audioSource.clip == null)
                return;
            
            Progression = GetNormalizedProgression(audioSource.time);
            CurrentSplineSpatialData = GetSplineData(Progression);
        }

        public float GetNormalizedProgression(float progression) => Mathf.Clamp01(progression / audioSource.clip.length);

        public SpatialData GetSplineData(float progression)
        {
            var splineSample = splineComputer.Evaluate(progression % 1);
            
            return new SpatialData
            {
                Position = splineSample.position,
                Rotation = splineSample.rotation
            };;
        }
        
        public SpatialData GetCurrentRailData(int rail) => GetRailData(CurrentSplineSpatialData, rail);
        
        public abstract SpatialData GetRailData(SpatialData spatialData, int rail);
    }
}
