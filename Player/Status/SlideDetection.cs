using Project.Scripts.Physics;
using UnityEngine;

namespace Project.Scripts.Player.Status
{
    public class SlideDetection : MonoBehaviour
    {
        public RaycastHit? Hit { get; private set; }
        
        private GroundDetection _groundDetection;
        private RaycastHelper _raycastHelper;

        private void Awake()
        {
            _groundDetection = FindObjectOfType<GroundDetection>();
            _raycastHelper = FindObjectOfType<RaycastHelper>();
        }

        private void Update()
        {
            if (_groundDetection.Hit != null && 1 << _groundDetection.Hit.Value.transform.gameObject.layer == _raycastHelper.SlidableLayer)
                Hit = _groundDetection.Hit;
            else
                Hit = null;
        }
    }
}
