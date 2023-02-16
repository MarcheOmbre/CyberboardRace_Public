using Project.Scripts.Track;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Logic
{
    public class TrackStart : MonoBehaviour
    {
        [SerializeField] private Canvas menu;
        [SerializeField] private Button button;

        private TrackManager _trackManager;

        private void Awake()
        {
            _trackManager = FindObjectOfType<TrackManager>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            menu.enabled = false;
            _trackManager.Play();
            button.onClick.RemoveListener(OnClicked);
        }
    }
}
