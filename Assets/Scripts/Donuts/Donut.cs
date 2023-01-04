using UnityEngine;

namespace Donuts
{
    public class Donut : MonoBehaviour
    {
        [SerializeField] private DonutType donutType;

        public DonutType Type { get => donutType; }
    }
}