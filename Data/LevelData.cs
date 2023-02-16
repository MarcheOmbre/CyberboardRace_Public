using System;
using System.Collections.Generic;
using Project.Scripts.Data.Types;
using UnityEngine;

namespace Project.Scripts.Data
{
    public class LevelData : MonoBehaviour
    {
        [Serializable]
        private struct IntDataEntry
        {
            public string key;
            public IntData intData;
        }
        
        [Serializable]
        private struct FloatDataEntry
        {
            public string key;
           public FloatData floatData;
        }

        public IReadOnlyDictionary<string, IntData.RuntimeData> IntData
        {
            get
            {
                if (_intData != null) 
                    return _intData;
                
                _intData = new Dictionary<string, IntData.RuntimeData>();
                foreach (var intEntry in intEntries)
                    _intData.Add(intEntry.key, intEntry.intData.Generate());

                return _intData;
            }
        }
        
        public IReadOnlyDictionary<string, FloatData.RuntimeData> FloatData
        {
            get
            {
                if (_floatData != null) 
                    return _floatData;
                
                _floatData = new Dictionary<string, FloatData.RuntimeData>();
                foreach (var floatEntry in floatEntries)
                    _floatData.Add(floatEntry.key, floatEntry.floatData.Generate());

                return _floatData;
            }
        }

        [SerializeField] private IntDataEntry[] intEntries;
        [SerializeField] private FloatDataEntry[] floatEntries;

        private Dictionary<string, IntData.RuntimeData> _intData;
        private Dictionary<string, FloatData.RuntimeData> _floatData;
    }
}
