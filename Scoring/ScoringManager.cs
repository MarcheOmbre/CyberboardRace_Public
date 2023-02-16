using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Damage;
using Project.Scripts.Data;
using Project.Scripts.Data.Types;
using Project.Scripts.Player.Status;
using Project.Scripts.Settings;
using Project.Scripts.Shared;
using UnityEngine;
using Random = System.Random;

namespace Project.Scripts.Scoring
{
    public class ScoringManager : MonoBehaviour
    {

        public struct TimeScoreData
        {
            public string Guid;
            public float StartTime;
            public SimpleScoreData SimpleScoreData;
        }

        public struct SimpleScoreData
        {
            public string Prefix;
            public ScoreConfig ScoreConfig;
            public int ComputedScore;
        }
        
        public event Action<string, TimeScoreData> OnTimeScoreStartProcessing = delegate {  }; 
        public event Action<string, TimeScoreData> OnTimeScoreEndProcessing = delegate {  }; 
        public event Action<SimpleScoreData> OnSimpleScoreAdded = delegate {  };
        public event Action<bool, int> OnComputed = delegate {  };

        
        public float LineTime { get; private set; }
        public int CurrentCombo => _computedScore.Count + 1;
        
        private DamagesManager _damagesManager;
        private GroundDetection _groundDetection;
        private SlideDetection _slideDetection;
        private LevelSettings _levelSettings;
        private LevelData _levelData;

        private readonly List<SimpleScoreData> _computedScore = new List<SimpleScoreData>();
        private readonly Dictionary<string, TimeScoreData> _processingTimeScoreData = new Dictionary<string, TimeScoreData>();

        private IntData.RuntimeData _runtimeData;
        
        private Coroutine _computeCoroutine;
        private int _cacheScore;

        private void Awake()
        {
            _runtimeData = FindObjectOfType<LevelData>().IntData[Constants.DataKeyScore];
            
            _damagesManager = FindObjectOfType<DamagesManager>();
            _groundDetection = FindObjectOfType<GroundDetection>();
            _slideDetection = FindObjectOfType<SlideDetection>();
            _levelSettings = FindObjectOfType<LevelSettings>();
        }


        private void OnEnable()
        {
            _damagesManager.OnHit += OnHit;
        }

        private void Update()
        {
            //Line up
            if (LineTime > 0 && _slideDetection.Hit == null && _groundDetection.IsFlatGrounded || _processingTimeScoreData.Count > 0)
                LineTime = Mathf.Max(LineTime - Time.deltaTime, 0);

            //End line
            var isGrounded = _slideDetection.Hit == null && _groundDetection.IsFlatGrounded;
            if (_computedScore.Count > 0 && isGrounded && _computeCoroutine == null)
                _computeCoroutine = StartCoroutine(ComputeLine());
        }
        
        private void OnDisable()
        {
            _damagesManager.OnHit -= OnHit;
        }

        private void OnHit(float damages)
        {
            if (_processingTimeScoreData.Count == 0 && _computedScore.Count == 0)
                return;

            //Coroutine
            if (_computeCoroutine != null)
            {
                StopCoroutine(_computeCoroutine);
                _computeCoroutine = null;   
            }

            //Score list
            _computedScore.Clear();
            foreach (var timeScoreData in _processingTimeScoreData)
                OnTimeScoreEndProcessing(timeScoreData.Key, timeScoreData.Value);
            _processingTimeScoreData.Clear();
            
            //Line up
            LineTime = 0;
            
            //Events
            OnComputed(false, 0);
        }
        
        private IEnumerator ComputeLine()
        {
            if(_computedScore.Count == 0)
                yield break;

            //Wait for damages
            yield return new WaitUntil(() => LineTime <= 0);

            //Compute score
            var score = _computedScore.Sum(x => x.ComputedScore);
            _runtimeData.Value += score;
            OnComputed(true, score);
            
            _computedScore.Clear();
            _computeCoroutine = null;
        }

        private void AddScore(SimpleScoreData simpleScoreData)
        {
            _computedScore.Add(simpleScoreData);

            //Score multiplied by combo divide by the number of same trick in the line
            LineTime += _levelSettings.ScoringSettings.ScoringLineDelay;
        }
        
        public string StartTimeScoreData(string prefix, ScoreConfig scoreConfig, float customMultiplier = 1)
        {
            var score = new Random().Next(scoreConfig.ScoreFork.x, scoreConfig.ScoreFork.y) * customMultiplier;
            
            var timeScoreData = new TimeScoreData
            {
                Guid = Guid.NewGuid().ToString(),
                StartTime = Time.time,
                SimpleScoreData = new SimpleScoreData
                {
                    Prefix = prefix,
                    ScoreConfig = scoreConfig,
                    ComputedScore = (int)(score * CurrentCombo)
                }
            };
            
            _processingTimeScoreData.Add(timeScoreData.Guid, timeScoreData);
            OnTimeScoreStartProcessing(timeScoreData.Guid, timeScoreData);

            return timeScoreData.Guid;
        }

        public void StopTimeScoreData(string guid)
        {
            if (!_processingTimeScoreData.TryGetValue(guid, out var timeScoreStartData))
                return;
            
            timeScoreStartData.SimpleScoreData.ComputedScore =
                (int) ((Time.time - timeScoreStartData.StartTime) * timeScoreStartData.SimpleScoreData.ComputedScore);
            
            AddScore(timeScoreStartData.SimpleScoreData);
            _processingTimeScoreData.Remove(guid);
            
            OnTimeScoreEndProcessing(guid, timeScoreStartData);
        }



        public void AddSimpleScore(SimpleScoreData simpleScoreData, float customMultiplier = 1)
        {
            var score = new Random().Next(simpleScoreData.ScoreConfig.ScoreFork.x,
                simpleScoreData.ScoreConfig.ScoreFork.y) * customMultiplier;
            
            simpleScoreData.ComputedScore = (int) (score * CurrentCombo * _levelSettings.ScoringSettings.ComboMultiplier
                                            / (_computedScore.Count(x =>
                                                x.ScoreConfig == simpleScoreData.ScoreConfig) + 1));

            AddScore(simpleScoreData);
            OnSimpleScoreAdded(simpleScoreData);
        }
    }
}
