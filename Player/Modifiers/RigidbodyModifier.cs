using Project.Scripts.Settings;
using UnityEngine;

namespace Project.Scripts.Player.Modifiers
{
    public class RigidbodyModifier : MonoBehaviour
    {
        [SerializeField] private Rigidbody modifierRigidbody;

        private LevelSettings _levelSettings;
        
        private void Awake()
        {
            _levelSettings = FindObjectOfType<LevelSettings>();
        }

        private void Start()
        {
            modifierRigidbody.centerOfMass = Vector3.zero;
            modifierRigidbody.useGravity = false;
            modifierRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            modifierRigidbody.mass = _levelSettings.HandlingSettings.Mass;
            modifierRigidbody.drag = _levelSettings.HandlingSettings.Drag;
            modifierRigidbody.angularDrag = _levelSettings.HandlingSettings.AngularDrag;
            modifierRigidbody.inertiaTensor = _levelSettings.HandlingSettings.InertiaTensor;
            modifierRigidbody.inertiaTensorRotation = Quaternion.Euler(_levelSettings.HandlingSettings.InertiaTensorRotation);
        }
    }
}
