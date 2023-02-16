using Project.Scripts.Damage.Abstracts;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Scripts.Damage
{
    /// <summary>
    /// Physics Matrix settings needed
    /// </summary>
    public class DamageArea : ADamage
    {
        [SerializeField] [Min(0)] private float damages;
        [SerializeField] private bool repeated;
        [SerializeField] [Min(0)] private float delay;
        
        private float _lastDamageTime;
        private bool _entered;

        private void Update()
        {
            if (!_entered)
                return;
            
            //Manage time
            var currentTime = Time.time;
            if (currentTime - _lastDamageTime < delay)
                return;

            _lastDamageTime = currentTime;
            Hit(damages);
        }

        private void OnTriggerEnter(Collider other)
        {
            StartHit();
            
            if (!repeated)
                Hit(damages);
            else
                _entered = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            _entered = false;
            
            EndHit();
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(DamageArea))]
    public class DamageAreaBehaviourEditor : Editor
    {
        private SerializedProperty _damages;
        private SerializedProperty _repeated;
        private SerializedProperty _delay;

        private void OnEnable()
        {
            _damages = serializedObject.FindProperty("damages");
            _repeated = serializedObject.FindProperty("repeated");
            _delay = serializedObject.FindProperty("delay");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_damages);
            EditorGUILayout.PropertyField(_repeated);

            if (_repeated.boolValue)
                EditorGUILayout.PropertyField(_delay);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}