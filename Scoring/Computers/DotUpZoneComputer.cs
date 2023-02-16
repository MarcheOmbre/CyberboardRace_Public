using Project.Scripts.Scoring.Abstracts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Scripts.Scoring.Computers
{
    public class DotUpZoneComputer : AScoreZone
    {
        [SerializeField] private ScoreConfig scoreConfig;
        [FormerlySerializedAs("targetAlignVector")] [SerializeField] private Vector3 targetAlignLocalDirection;

        private float _dot;
        
        protected override void OnTriggerEnter(Collider other)
        {
            _dot = Mathf.Abs(Vector3.Dot(other.transform.TransformDirection(targetAlignLocalDirection),
                transform.up));
            
            base.OnTriggerEnter(other);
        }

        protected override bool OnComputeTrick(out ScoringManager.SimpleScoreData simpleScoreData, out float multiplier)
        {
            multiplier = _dot;
            simpleScoreData = new ScoringManager.SimpleScoreData {ScoreConfig = scoreConfig};
            return true;
        }
    }
}
