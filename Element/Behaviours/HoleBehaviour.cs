using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Project.Scripts.Element.Behaviours
{
    public class HoleBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject[] planks;
        [SerializeField] private int holeSize;

        private void OnValidate()
        {
            planks = planks.OrderBy(x => x.transform.localPosition.y).ToArray();
            holeSize = Mathf.Clamp(holeSize, 0, planks.Length);
        }

        private void OnEnable()
        {
            var holeStartPosition = new Random().Next(0, planks.Length - holeSize + 1);

            for (var i = 0; i < planks.Length; i++)
                planks[i].SetActive(i < holeStartPosition || i >= holeStartPosition + holeSize);
        }
    }
}
