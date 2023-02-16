using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Display.Component;
using Project.Scripts.Scoring;
using Project.Scripts.Shared;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Display
{
    public class ScoringManagerDisplay : MonoBehaviour
    {
        [SerializeField][Min(0)] private float delayBeforeDespawning;
        [SerializeField] private RectTransform spawnParent;
        [SerializeField] private ScoreComponent slideScoreComponentPrefab;
        [SerializeField] private ScoreComponent trickScoreComponentPrefab;
        [SerializeField] private GameObject succeedPrefab;
        [SerializeField] private GameObject failedPrefab;
        [SerializeField] private TMP_Text comboTmp;
        [SerializeField] private TMP_Text lineTimeTmp;

        private readonly List<ScoreComponent> _spawnedScore = new List<ScoreComponent>();
        private readonly Dictionary<string, Tuple<ScoringManager.TimeScoreData, ScoreComponent>> _spawnedTimeScore = new Dictionary<string, Tuple<ScoringManager.TimeScoreData, ScoreComponent>>();
        
        private ScoringManager _scoreManager;

        private void Awake()
        {
            _scoreManager = FindObjectOfType<ScoringManager>();
        }

        private void OnEnable()
        {
            _scoreManager.OnTimeScoreStartProcessing += OnTimeScoreStartProcessing;
            _scoreManager.OnTimeScoreEndProcessing += OnTimeScoreEndProcessing;
            _scoreManager.OnSimpleScoreAdded += OnSimpleScoreAdded;
            _scoreManager.OnComputed += OnComputed;
        }

        private void Update()
        {
            lineTimeTmp.enabled = _scoreManager.LineTime > 0;
            
            if(lineTimeTmp.enabled)
                lineTimeTmp.SetText("Lineup : " + _scoreManager.LineTime.ToString("F1"));

            foreach (var (timeScoreData, scoreComponent) in _spawnedTimeScore.Values)
            {
                scoreComponent.Value.SetText(((int)((Time.time - timeScoreData.StartTime) 
                                                    * timeScoreData.SimpleScoreData.ComputedScore)).ToString());   
            }

            comboTmp.SetText("x" + _scoreManager.CurrentCombo);
        }

        private void OnDisable()
        {
            _scoreManager.OnComputed -= OnComputed;
            _scoreManager.OnSimpleScoreAdded -= OnSimpleScoreAdded;
            _scoreManager.OnTimeScoreEndProcessing -= OnTimeScoreEndProcessing;
            _scoreManager.OnTimeScoreStartProcessing -= OnTimeScoreStartProcessing;
        }

        private void OnTimeScoreStartProcessing(string guid, ScoringManager.TimeScoreData timeScoreData)
        {
            var scoreComponent = Pools.UiPool.Spawn(slideScoreComponentPrefab.gameObject).GetComponent<ScoreComponent>();
            scoreComponent.transform.SetParent(spawnParent, false);
            
            //Set text
            var text = string.Empty;
            if (timeScoreData.SimpleScoreData.Prefix != null)
                text += timeScoreData.SimpleScoreData.Prefix + " ";
            text += timeScoreData.SimpleScoreData.ScoreConfig.Naming;
            
            scoreComponent.Name.SetText(text);
            scoreComponent.Value.SetText("0");

            //Add to list
            _spawnedTimeScore.Add(guid, new Tuple<ScoringManager.TimeScoreData, ScoreComponent>(timeScoreData, scoreComponent));
        }

        private void OnTimeScoreEndProcessing(string guid, ScoringManager.TimeScoreData simpleScoreData)
        {
            if (!_spawnedTimeScore.TryGetValue(guid, out var tuple))
                return;

            _spawnedScore.Add(tuple.Item2);
            _spawnedTimeScore.Remove(guid);
        }

        private void OnSimpleScoreAdded(ScoringManager.SimpleScoreData simpleScoreData)
        {
            var scoreComponent = Pools.UiPool.Spawn(trickScoreComponentPrefab.gameObject).GetComponent<ScoreComponent>();
            scoreComponent.transform.SetParent(spawnParent, false);

            //Set text
            var text = string.Empty;
            if (simpleScoreData.Prefix != null)
                text += simpleScoreData.Prefix + " ";
            text += simpleScoreData.ScoreConfig.Naming;
            
            scoreComponent.Name.SetText(text);
            scoreComponent.Value.SetText(simpleScoreData.ComputedScore.ToString());

            //Add to list
            _spawnedScore.Add(scoreComponent);
        }

        private void OnComputed(bool succeed, int totalScore)
        {
            var result = Pools.UiPool.Spawn(succeed ? succeedPrefab.gameObject : failedPrefab.gameObject);
            result.SetParent(spawnParent, false);
            
            StartCoroutine(Despawn(_spawnedScore.Select(x => x.gameObject).Append(result.gameObject).ToList()));
            _spawnedScore.Clear();
        }

        private IEnumerator Despawn(IEnumerable<GameObject> scoreComponents)
        {
            yield return new WaitForSeconds(delayBeforeDespawning);

            foreach (var spawnScore in scoreComponents) 
                Pools.UiPool.Despawn(spawnScore.gameObject);
        }
    }
}