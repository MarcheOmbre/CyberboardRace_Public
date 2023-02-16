using System;
using System.Collections.Generic;
using System.Linq;
using LDG.SoundReactor;
using Project.Scripts.Settings;
using Project.Scripts.Shared;
using Project.Scripts.Track.Abstract;
using UnityEngine;
using Random = System.Random;

namespace Project.Scripts.Element
{
    public class RandomSpawnerManager : PropertyDriver
    {
        [Serializable]
        private struct RandomSpawnerData
        {
            public TrackElement trackElement;
            [Min(1)] public int weight;
        }
        
        private struct SpawnItem
        {
            public TrackElement TrackElement;
            public float Progression;
            public float ProgressionScale;
        }
        
        [SerializeField] private RandomSpawnerData[] trackElements;
        
        private LevelSettings _levelSettings;
        private ATrack _track;

        private readonly List<SpawnItem> _spawnQueue = new List<SpawnItem>();
        private readonly List<SpawnItem> _despawnQueue = new List<SpawnItem>();

        private int _totalWeight;
        private float _lastProgression;
        private float _offset;

        private void Awake()
        {
            _levelSettings = FindObjectOfType<LevelSettings>();
            _track = FindObjectOfType<ATrack>(); ;
        }

        private void Start()
        {
            _offset = _levelSettings.GeneralSettings.TrackDistanceOffset / _track.TotalLength;
            _totalWeight = trackElements.Sum(x => x.weight);
        }

        protected override void Update()
        {
            base.Update();

            RefreshSpawn();
            RefreshDespawn();
        }

        private TrackElement PickObject()
        {
            var random = new Random().Next(0, _totalWeight);

            TrackElement trackElement = null;
            
            var sum = 0;
            foreach (var randomSpawnerData in trackElements)
            {
                sum += randomSpawnerData.weight;
                
                if(random >= sum)
                    continue;
                
                trackElement = randomSpawnerData.trackElement;
                break;
            }

            return trackElement;
        }

        private void RefreshSpawn()
        {
            //Check
            if (_spawnQueue.Count <= 0)
                return;

            //Set variables
            var spawnItem = _spawnQueue[0];
            var halfProgressionScale = spawnItem.ProgressionScale / 2;
            
            if ((spawnItem.Progression + halfProgressionScale - _track.Progression) * _track.TotalLength > _levelSettings.GeneralSettings.SpawnDistance)
                return;
            
            //Spawn
            var trackObject = Pools.ElementsPool.Spawn(spawnItem.TrackElement.gameObject).GetComponent<TrackElement>();
            trackObject.Set(_track, spawnItem.Progression + halfProgressionScale);
            
            //Add to despawn
            spawnItem.TrackElement = trackObject;
            _despawnQueue.Add(spawnItem);
            
            //Remove from spawn
            _spawnQueue.RemoveAt(0);
        }

        private void RefreshDespawn()
        {
            if (_despawnQueue.Count <= 0)
                return;
            
            //Compute progression plus lenght
            var totalProgression = _despawnQueue[0].Progression + _despawnQueue[0].ProgressionScale;
            if ((_track.Progression - totalProgression) * _track.TotalLength < _levelSettings.GeneralSettings.DespawnDistance)
                return;

            Pools.ElementsPool.Despawn(_despawnQueue[0].TrackElement.gameObject);
            _despawnQueue.RemoveAt(0);
        }
        
        protected override void DoLevel()
        {
            base.DoLevel();

            if (onBeat && !isBeat)
                return;

            //Check distance
            var currentProgression = level.frequencyMode == FrequencyBase.Audio
                ? level.spectrumFilter.spectrumSource.audioSource.time
                : level.spectrumFilter.spectrumSource.midiSource.time;
            
            var progression = _track.GetNormalizedProgression(currentProgression) +
                              _offset;

            //Check distance
            if (progression <= _lastProgression)
                return;

            //Add to queue
            var trackObject = PickObject();
            var spawnItem = new SpawnItem
            {
                TrackElement = trackObject,
                Progression = progression,
                ProgressionScale = trackObject.Length / _track.TotalLength * _levelSettings.TrackSettings.MutualDistanceMultiplier
            };

            _spawnQueue.Add(spawnItem);

            //Set progression
            _lastProgression = progression + spawnItem.ProgressionScale;
        }
    }
}
