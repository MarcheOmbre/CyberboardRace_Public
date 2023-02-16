using UnityEngine;

namespace Project.Scripts.Scoring
{
    [CreateAssetMenu(fileName = "Score", menuName = "Cyberboard/Config/Score", order = 1)]
    public class ScoreConfig : ScriptableObject
    {
        public string Naming => naming;
        public Vector2Int ScoreFork => scoreFork;
        
        [SerializeField] private string naming;
        [SerializeField] private Vector2Int scoreFork;
    }
}
