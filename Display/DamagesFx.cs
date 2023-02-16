using Project.Scripts.Damage;
using UnityEngine;

namespace Project.Scripts.Display
{
    public class DamagesFx : MonoBehaviour
    {
        private static readonly int IsRecoveringAnimatorKey = Animator.StringToHash("isRecovering");
        
        [SerializeField] private Animator animator;
        
        private DamagesManager _damagesManager;

        private void Awake()
        {
            _damagesManager = FindObjectOfType<DamagesManager>();
        }

        private void Update() => animator.SetBool(IsRecoveringAnimatorKey, _damagesManager.IsRecovering);
    }
}
