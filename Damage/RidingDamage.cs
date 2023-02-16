using Project.Scripts.Damage.Abstracts;
using Project.Scripts.Player.Status;
using Project.Scripts.Settings;
using UnityEngine;

namespace Project.Scripts.Damage
{
    public class RidingDamage : ADamage
    {
        public bool IsDamaging
        {
            get
            {
                var isSliding = _slideDetection.Hit == null;
                var isReverseGrounded = _groundDetection.Hit != null;
                                        
                
                return isSliding && isReverseGrounded && Vector3.Dot(_groundDetection.Hit.Value.normal, 
                        ridingDamageRigidbody.rotation * Vector3.down) > _levelSettings.DamageSettings.SlideDamageMinDot;
            }
        }

        [SerializeField] private Rigidbody ridingDamageRigidbody;
        
        private LevelSettings _levelSettings;
        private SlideDetection _slideDetection;
        private GroundDetection _groundDetection;
        private bool _isDamaging;

        protected override void Awake()
        {
            base.Awake();
            
            _levelSettings = FindObjectOfType<LevelSettings>();
            _slideDetection = FindObjectOfType<SlideDetection>();
            _groundDetection = FindObjectOfType<GroundDetection>();
        }

        private void Update()
        {
            //Check damages
            var isDamaging = IsDamaging;
            
            if (_isDamaging != isDamaging)
            {
                if(isDamaging)
                    StartHit();
                else
                    EndHit();

                _isDamaging = isDamaging;
            }

            if (!isDamaging)
                return;

            Hit(_levelSettings.DamageSettings.SlideDamage * Time.deltaTime);
        }
    }
}