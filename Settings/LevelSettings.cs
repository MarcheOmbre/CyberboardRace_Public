using UnityEngine;

namespace Project.Scripts.Settings
{
    public class LevelSettings : MonoBehaviour
    {
        public GeneralSettings GeneralSettings => generalSettings;
        public HandlingSettings HandlingSettings => handlingSettings;
        public TrackSettings TrackSettings => trackSettings;
        public DamageSettings DamageSettings => damageSettings;
        public ScoringSettings ScoringSettings => scoringSettings;
        
        [SerializeField] private GeneralSettings generalSettings;
        [SerializeField] private HandlingSettings handlingSettings;
        [SerializeField] private TrackSettings trackSettings;
        [SerializeField] private DamageSettings damageSettings;
        [SerializeField] private ScoringSettings scoringSettings;
    }
}
