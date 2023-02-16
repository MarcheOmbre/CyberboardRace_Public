using Project.Scripts.Data;
using Project.Scripts.Data.Types;
using Project.Scripts.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Display
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        private FloatData.RuntimeData _lifeData;
        
        private void Awake()
        {
            _lifeData = FindObjectOfType<LevelData>().FloatData[Constants.DataKeyPlayerLife];
            
            slider.minValue = _lifeData.Minimum.value;
            slider.maxValue = _lifeData.Maximum.value;
            slider.value = _lifeData.Value;
        }

        private void OnEnable()
        {
            _lifeData.OnValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _lifeData.OnValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(float oldValue, float newValue)
        {
            slider.value = newValue;
        }
    }
}
