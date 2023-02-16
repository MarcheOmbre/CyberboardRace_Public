using Project.Scripts.Data;
using Project.Scripts.Data.Types;
using Project.Scripts.Shared;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Display
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private TMP_Text score;

        private IntData.RuntimeData _score;
        
        private void Awake()
        {
            _score = FindObjectOfType<LevelData>().IntData[Constants.DataKeyScore];
        }

        private void OnEnable()
        {
            _score.OnValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _score.OnValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(int oldValue, int newValue)
        {
            score.SetText(newValue.ToString());
        }
    }
}
