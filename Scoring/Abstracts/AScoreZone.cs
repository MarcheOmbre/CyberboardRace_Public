using UnityEngine;

namespace Project.Scripts.Scoring.Abstracts
{
    public abstract class AScoreZone : MonoBehaviour
    {
        private ScoringManager _scoringManager;
        private bool _done;

        private void Awake()
        {
            _scoringManager = FindObjectOfType<ScoringManager>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_done || !OnComputeTrick(out var simpleScoreData, out var multiplier))
                return;
            
            _scoringManager.AddSimpleScore(simpleScoreData, multiplier);
            _done = true;
        }

        protected abstract bool OnComputeTrick(out ScoringManager.SimpleScoreData simpleScoreData, out float multiplier);
    }
}
