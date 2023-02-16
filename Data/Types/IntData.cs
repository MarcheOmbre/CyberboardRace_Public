using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Scripts.Data.Types
{
    [CreateAssetMenu(fileName = "Int_Data", menuName = "Cyberboard/Data/Int", order = 1)]
    public class IntData : ScriptableObject
    {
        public class RuntimeData
        {
            public event Action OnMinimumReached = delegate { };
            public event Action<int, int> OnValueChanged = delegate { };
            public event Action OnMaximumReached = delegate { };

            public BoolInt Minimum => _minimum;
            public BoolInt Maximum => _maximum;

            public int Value
            {
                get => _value;

                set
                {
                    if (_minimum.boolean)
                        value = Mathf.Max(value, _minimum.value);
                    if (_maximum.boolean)
                        value = Mathf.Min(value, _maximum.value);

                    if (value == _value)
                        return;

                    var oldValue = _value;
                    _value = value;

                    OnValueChanged(oldValue, _value);

                    if (_value == _minimum.value)
                        OnMinimumReached();
                    else if (_value == _maximum.value)
                        OnMaximumReached();
                }
            }

            private int _value;
            private readonly BoolInt _minimum;
            private readonly BoolInt _maximum;

            public RuntimeData(int value, BoolInt minimum, BoolInt maximum)
            {
                _value = value;
                _minimum = minimum;
                _maximum = maximum;
            }
        }

        [SerializeField] private int value;
        [SerializeField] private BoolInt minimum;
        [SerializeField] private BoolInt maximum;

        public RuntimeData Generate() => new RuntimeData(value, minimum, maximum);

        #region Limits

        [Serializable]
        public struct BoolInt
        {
            public bool boolean;
            public int value;
        }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(BoolInt), true)]
        public class BoolIntPropertyDrawer : PropertyDrawer
        {
            private const float BoolWidth = 50;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);

                //Rec
                var nameRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                var booleanRect = new Rect(nameRect.x + nameRect.width, position.y, BoolWidth, position.height);
                var genderRect = new Rect(booleanRect.x + booleanRect.width, position.y,
                    position.x + position.width - booleanRect.xMax, position.height);

                //Label
                EditorGUI.LabelField(nameRect, label);
                position.x += EditorGUIUtility.labelWidth;

                //Boolean
                var boolean = property.FindPropertyRelative("boolean");
                boolean.boolValue = EditorGUI.Toggle(booleanRect, boolean.boolValue);
                position.x += BoolWidth;

                //Value
                var value = property.FindPropertyRelative("value");
                if (boolean.boolValue)
                    value.intValue = EditorGUI.IntField(genderRect, value.intValue);


                EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
                EditorGUIUtility.singleLineHeight;
        }
#endif

        #endregion
    }
}