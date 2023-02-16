using Project.Scripts.Player.Modifiers;
using Project.Scripts.Player.Status;
using Project.Scripts.Settings;
using UnityEngine;

namespace Project.Scripts.Scoring.Computers
{
    [RequireComponent(typeof(ScoringManager))]
    public class ManualScoreComputer : MonoBehaviour
    {
        private enum ManualType
        {
            FrontManual,
            BackManual,
            SideManual
        }

        [SerializeField] private Rigidbody manualRigidbody;
        [SerializeField] private ScoreConfig frontManual;
        [SerializeField] private ScoreConfig backManual;
        [SerializeField] private ScoreConfig sideManual;

        private ScoringManager _scoringManager;
        private SlideDetection _slideDetection;
        private GroundDetection _groundDetection;
        private Levitate _levitate;
        private LevelSettings _levelSettings;

        private bool _isSliding;
        private ManualType? _lastManualType;

        private string _currentGuid;


        private void Awake()
        {
            _scoringManager = FindObjectOfType<ScoringManager>();
            _slideDetection = FindObjectOfType<SlideDetection>();
            _groundDetection = FindObjectOfType<GroundDetection>();
            _levitate = FindObjectOfType<Levitate>();
            _levelSettings = FindObjectOfType<LevelSettings>();
        }

        private void Update()
        {
            var isGrounded = _groundDetection.Hit != null && _slideDetection.Hit == null && _levitate.Hit == null;
            ManualType? manualType = null;

            if (isGrounded)
            {
                //Direction
                var rigidbodyUp = manualRigidbody.rotation * Vector3.up;
                var sideDot = Vector3.Dot(rigidbodyUp, _groundDetection.Hit.Value.normal);

                if (Mathf.Abs(sideDot) < _levelSettings.ScoringSettings.SlidingSideOffset)
                {
                    var sideSideDot = Vector3.Dot(rigidbodyUp, _groundDetection.Hit.Value.transform.forward);
                    if (sideSideDot > _levelSettings.ScoringSettings.SlidingSideOffset)
                        manualType = ManualType.FrontManual;
                    else if (sideSideDot < -_levelSettings.ScoringSettings.SlidingSideOffset)
                        manualType = ManualType.BackManual;
                    else
                        manualType = ManualType.SideManual;
                }
            }

            if (manualType == _lastManualType)
                return;

            if (_lastManualType != null)
            {
                _scoringManager.StopTimeScoreData(_currentGuid); //TODO : Add custom score//
                _lastManualType = null;
            }

            if (manualType == null) 
                return;
            
            var scoreConfig = manualType switch
            {
                ManualType.FrontManual => frontManual,
                ManualType.BackManual => backManual,
                ManualType.SideManual => sideManual,
                _ => null
            };

            if (scoreConfig == null)
                return;
            
            _currentGuid = _scoringManager.StartTimeScoreData(null, scoreConfig);
            _lastManualType = manualType;
        }
    }
}