using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Track
{
    public class StraightTrack : ATrack
    {
        [SerializeField] [Min(0)] private float width;

        private float _railWidth;
        
        protected override void Awake()
        {
            base.Awake();
            
            _railWidth = width / (SideLineCount * 2 + 1);
        }

        public override SpatialData GetRailData(SpatialData spatialData, int rail)
        {
            spatialData.Position += spatialData.Rotation * Vector3.right * rail * _railWidth;
            return spatialData;
        }
    }
}
