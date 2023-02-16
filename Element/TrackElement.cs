using System;
using Project.Scripts.Track.Abstract;
using UnityEngine;

namespace Project.Scripts.Element
{
    public class TrackElement : MonoBehaviour
    {
        public event Action<ATrack, ATrack.SpatialData> OnSet = delegate { };

        public float Length => length;
        
        [SerializeField] [Min(0)] private float length;
        
        public void Set(ATrack track, float progression)
        {
            var splineData = track.GetSplineData(progression);

            var cachedTransform = transform;
            cachedTransform.position = splineData.Position;
            cachedTransform.rotation =  splineData.Rotation;
            
            OnSet(track, splineData);
        }
    }
}
