using UnityEngine;

namespace Project.Scripts.Physics
{
    public class RaycastHelper : MonoBehaviour
    {
        public Camera RaycastCamera => raycastCamera;
        public int PlayerInputLayer { get; private set; }
        public int GroundLayer { get; private set; }
        public int SlidableLayer { get; private set; }
        public int GroundableLayer { get; private set; }

        [SerializeField] private Camera raycastCamera;

        private void Awake()
        {
            PlayerInputLayer = 1 << LayerMask.NameToLayer(Shared.Constants.PlayerInputLayer);
            
            GroundLayer = 1 << LayerMask.NameToLayer(Shared.Constants.GroundableLayer);
            SlidableLayer = 1 << LayerMask.NameToLayer(Shared.Constants.SlidableLayer);
            GroundableLayer = GroundLayer | SlidableLayer;
        }

        public bool ScreenToRaycast(Vector2 screenPosition, out RaycastHit raycastHit, LayerMask? layerMask = null)
        {
            var ray = raycastCamera.ScreenPointToRay(screenPosition);

            return layerMask == null ? 
                UnityEngine.Physics.Raycast(ray, out raycastHit, raycastCamera.farClipPlane) : 
                UnityEngine.Physics.Raycast(ray, out raycastHit, raycastCamera.farClipPlane, layerMask.Value);
        }
    }
}
