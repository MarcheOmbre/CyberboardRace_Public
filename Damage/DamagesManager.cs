using System;
using Project.Scripts.Data;
using Project.Scripts.Data.Types;
using Project.Scripts.Settings;
using Project.Scripts.Shared;
using UnityEngine;

namespace Project.Scripts.Damage
{
    public class DamagesManager : MonoBehaviour
    {
        public event Action<float> OnHit = delegate { };

        public bool IsHit => _hitCount > 0;
        public bool IsRecovering => _invincibilityEndTime >= Time.time;

        private FloatData.RuntimeData _runtimeData;
        private LevelSettings _levelSettings;

        private float _invincibilityEndTime;
        private int _hitCount;

        private void Awake()
        {
            _runtimeData = FindObjectOfType<LevelData>().FloatData[Constants.DataKeyPlayerLife];
            _levelSettings = FindObjectOfType<LevelSettings>();
        }

        public void RegisterHit()
        {
            _hitCount++;
        }

        public void Hit(float damages)
        {
            if (IsRecovering)
                return;
            
            _runtimeData.Value -= damages;
            OnHit(damages);
        }

        public void UnregisterHit()
        {
            _hitCount--;
            if (_hitCount > 0 || IsRecovering)
                return;

            _invincibilityEndTime = Time.time + _levelSettings.DamageSettings.InvincibilityTime;
        }
    }
}