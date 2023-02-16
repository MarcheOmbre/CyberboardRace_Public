using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Scripts.Loading
{
    public class LoaderHelper : MonoBehaviour
    {
        [SerializeField] private bool current;
        [SerializeField] private string scene;
        private SceneLoader _sceneLoader;

        private void Awake()
        {
            _sceneLoader = FindObjectOfType<SceneLoader>();
        }

        public void Load()
        {
            var loadScene = current ? SceneManager.GetActiveScene().name : this.scene;
            _sceneLoader.LoadScene(loadScene);
        }
    }
    
    #if UNITY_EDITOR

    [CustomEditor(typeof(LoaderHelper))]
    public class LoaderEditor : Editor
    {
        private SerializedProperty _current;
        private SerializedProperty _scene;

        private void OnEnable()
        {
            _current = serializedObject.FindProperty("current");
            _scene = serializedObject.FindProperty("scene");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_current);
            
            if(!_current.boolValue)
                EditorGUILayout.PropertyField(_scene);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    
    #endif
}
