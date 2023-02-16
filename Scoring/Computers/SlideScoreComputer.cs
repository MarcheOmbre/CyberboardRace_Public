using Project.Scripts.Player.Status;
using Project.Scripts.Settings;
using UnityEngine;

namespace Project.Scripts.Scoring.Computers
{
    [RequireComponent(typeof(ScoringManager))]
    public class SlideScoreComputer : MonoBehaviour
    {
        private enum SlideType
        {
            Slide,
            DarkSlide,
            FrontSlide,
            BackSlide,
            SideSlide
        }

        [SerializeField] private Rigidbody slidingRigidbody;
        [SerializeField] private ScoreConfig slide;
        [SerializeField] private ScoreConfig darkSlide;
        [SerializeField] private ScoreConfig frontSlide;
        [SerializeField] private ScoreConfig backSlide;
        [SerializeField] private ScoreConfig sideSlide;

        private ScoringManager _scoringManager;
        private SlideDetection _slideDetection;
        private LevelSettings _levelSettings;

        private bool _isSliding;
        private SlideType? _lastSlideType;

        private string _currentGuid;


        private void Awake()
        {
            _scoringManager = FindObjectOfType<ScoringManager>();
            _slideDetection = FindObjectOfType<SlideDetection>();
            _levelSettings = FindObjectOfType<LevelSettings>();
        }

        private void Update()
        {
            var slidingHit = _slideDetection.Hit;
            SlideType? slideType = null;

            if (slidingHit != null)
            {
                //Direction
                var rigidbodyUp = slidingRigidbody.rotation * Vector3.up;
                var sideDot = Vector3.Dot(rigidbodyUp, slidingHit.Value.normal);
                slideType = SlideType.Slide;

                if (Mathf.Abs(sideDot) < _levelSettings.ScoringSettings.SlidingSideOffset)
                {
                    var sideSideDot = Vector3.Dot(rigidbodyUp, slidingHit.Value.transform.forward);
                    if (sideSideDot > _levelSettings.ScoringSettings.SlidingSideOffset)
                        slideType = SlideType.FrontSlide;
                    else if (sideSideDot < -_levelSettings.ScoringSettings.SlidingSideOffset)
                        slideType = SlideType.BackSlide;
                    else
                        slideType = SlideType.SideSlide;
                }
                else if (sideDot <= -_levelSettings.ScoringSettings.SlidingSideOffset)
                    slideType = SlideType.DarkSlide;
            }

            if (slideType == _lastSlideType)
                return;

            if (_lastSlideType != null)
            {
                _scoringManager.StopTimeScoreData(_currentGuid); //TODO : Add custom score//
                _lastSlideType = null;
            }

            if (slideType == null) 
                return;
            
            var scoreConfig = slideType switch
            {
                SlideType.Slide => slide,
                SlideType.DarkSlide => darkSlide,
                SlideType.FrontSlide => frontSlide,
                SlideType.BackSlide => backSlide,
                SlideType.SideSlide => sideSlide,
                _ => null
            };

            if (scoreConfig == null)
                return;
            
            _currentGuid = _scoringManager.StartTimeScoreData(null, scoreConfig);
            _lastSlideType = slideType;
        }
    }
}