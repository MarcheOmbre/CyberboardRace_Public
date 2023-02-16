using UnityEngine;

namespace Project.Scripts.Damage.Abstracts
{
    public class ADamage : MonoBehaviour
    {
        private DamagesManager _damagesManager;
        
        protected virtual void Awake()
        {
            _damagesManager = FindObjectOfType<DamagesManager>();
        }

        protected void StartHit() => _damagesManager.RegisterHit();

        protected void Hit(float value) => _damagesManager.Hit(value);

        protected void EndHit() => _damagesManager.UnregisterHit();
    }
}
