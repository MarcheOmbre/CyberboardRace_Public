using Project.Scripts.Data;
using Project.Scripts.Data.Types;
using Project.Scripts.Shared;
using Project.Scripts.Track;
using UnityEngine;

namespace Project.Scripts.Logic
{
    public class PlayerDeath : MonoBehaviour
    {
        [SerializeField] private Canvas menu;
        [SerializeField] private GameObject player;
        
        private TrackManager _trackManager;
        
        private FloatData.RuntimeData _playerLife;

        private void Awake()
        {
            _trackManager = FindObjectOfType<TrackManager>();
            _playerLife = FindObjectOfType<LevelData>().FloatData[Constants.DataKeyPlayerLife];
        }

        private void OnEnable()
        {
            _playerLife.OnMinimumReached += OnMinimumReached;
        }

        private void OnDisable()
        {
            _playerLife.OnMinimumReached -= OnMinimumReached;
        }

        private void OnMinimumReached()
        {
            _trackManager.Stop();
            menu.enabled = true;
            player.gameObject.SetActive(false);
        }
    }
}
