using System;
using System.Collections.Generic;
using Project.Scripts.Shared;
using Project.Scripts.Track.Abstract;
using UnityEngine;
using Random = System.Random;

namespace Project.Scripts.Element
{
    public class RailElementSpawner : MonoBehaviour
    {
        public event Action OnRailElementSet = delegate { };
        
        public IEnumerable<GameObject> SpawnedRailElement => _spawnedRailElement;

        [SerializeField] private TrackElement trackElement;
        [SerializeField] private GameObject[] railElements;
        [SerializeField] private bool randomPlacement;
        
        private readonly List<GameObject> _spawnedRailElement = new List<GameObject>();

        private void OnEnable()
        {
            trackElement.OnSet += OnSet;
        }

        private void OnDisable()
        {
            trackElement.OnSet -= OnSet;
        }

        private void OnSet(ATrack track, ATrack.SpatialData spatialData)
        {
            //Despawn useless
            foreach (var spawnedElement in _spawnedRailElement)
                Pools.ElementsPool.Despawn(spawnedElement);
            
            //Create list
            var railsCount = track.SideLineCount * 2 + 1;
            var elementsToSpawn = new GameObject[railsCount];
            var random = new Random();
            
            //Fill list
            for (var i = 0; i < railElements.Length; i++)
            {
                //Find index
                int index;

                if (randomPlacement)
                {
                    do
                    {
                        index = random.Next(0, elementsToSpawn.Length);
                    } 
                    while (elementsToSpawn[index] != null);
                }
                else
                    index = Mathf.Clamp(i, 0, railsCount -1);

                elementsToSpawn[index] = railElements[i];
            }

            for(var i = -track.SideLineCount; i <= track.SideLineCount; i++)
            {
                var railData = track.GetRailData(spatialData, i);
                var element = elementsToSpawn[i + track.SideLineCount];

                if (!element)
                    continue;
                
                var railElement = Pools.ElementsPool.Spawn
                (
                    element,
                    railData.Position,
                    railData.Rotation,
                    transform
                );
                
                _spawnedRailElement.Add(railElement.gameObject);
            }

            OnRailElementSet();
        }
    }
}
