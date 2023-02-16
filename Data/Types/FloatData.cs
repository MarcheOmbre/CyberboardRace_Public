using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Scripts.Data.Types
{
    [CreateAssetMenu(fileName = "Float_Data", menuName = "Cyberboard/Data/Float", order = 1)]
    public class FloatData : ScriptableObject
    {
        public class RuntimeData
        {
            public event Action OnMinimumReached = delegate {  };
            public event Action<float, float> OnValueChanged = delegate{ };
            public event Action OnMaximumReached = delegate {  };
            
            public BoolFloat Minimum => _minimum;
            public BoolFloat Maximum => _maximum;
            public float Value
            {
                get => _value;

                set
                {
                    if (_minimum.boolean)
                        value = Mathf.Max(value, _minimum.value);
                    if (_maximum.boolean)
                        value = Mathf.Min(value, _maximum.value);

                    //Check perfect same value
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (value == _value)
                        return;

                    var oldValue = _value;
                    _value = value;
                    
                    OnValueChanged(oldValue, _value);
                    
                    if (_value <= _minimum.value)
                        OnMinimumReached();
                    
                    else if (_value >= _maximum.value)
                        OnMaximumReached();
                }
            }

            private float _value;
            private readonly BoolFloat _minimum;
            private readonly BoolFloat _maximum;

            public RuntimeData(float value, BoolFloat minimum, BoolFloat maximum)
            {
                _value = value;
                _minimum = minimum;
                _maximum = maximum;
            }
        }

        [SerializeField] private float value;
        [SerializeField] private BoolFloat minimum;
        [SerializeField] private BoolFloat maximum;

        public RuntimeData Generate() => new RuntimeData(value, minimum, maximum);
        
        #region Limits
        
        [Serializable]
        public struct BoolFloat
        {
            public bool boolean;
            public float value;
        }
        
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(BoolFloat), true)]
        public class BoolFloatPropertyDrawer : PropertyDrawer
        {
            private const float BoolWidth = 50;
        
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);
            
                //Rec
                var nameRect = new Rect( position.x, position.y, EditorGUIUtility.labelWidth, position.height );
                var booleanRect = new Rect( nameRect.x + nameRect.width, position.y, BoolWidth, position.height );
                var genderRect = new Rect( booleanRect.x + booleanRect.width, position.y, position.x + position.width - booleanRect.xMax, position.height);
            
                //Label
                EditorGUI.LabelField(nameRect, label);
                position.x += EditorGUIUtility.labelWidth;
            
                //Boolean
                var boolean = property.FindPropertyRelative("boolean");
                boolean.boolValue = EditorGUI.Toggle(booleanRect, boolean.boolValue);
                position.x += BoolWidth;

                //Value
                var value = property.FindPropertyRelative("value");
                if(boolean.boolValue)
                    value.floatValue = EditorGUI.FloatField(genderRect, value.floatValue);
            
            
                EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
        }
#endif
        
        #endregion
    }
}
