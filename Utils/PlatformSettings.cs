using UnityEngine;

namespace Project.Scripts.Utils
{
    public class PlatformSettings : MonoBehaviour
    {
        [SerializeField][Min(1)] private int targetFps;
        [SerializeField] private int vSync;

        private void Awake()
        {
            Application.targetFrameRate = targetFps;
            QualitySettings.vSyncCount = vSync;
        }
    }
}
