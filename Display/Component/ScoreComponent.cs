using TMPro;
using UnityEngine;

namespace Project.Scripts.Display.Component
{
    public class ScoreComponent : MonoBehaviour
    {
        public TMP_Text Name => nameTmp;
        public TMP_Text Value => valueTmp;
        
        [SerializeField] private TMP_Text nameTmp;
        [SerializeField] private TMP_Text valueTmp;
    }
}
