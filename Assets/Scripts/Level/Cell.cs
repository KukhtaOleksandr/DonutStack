using System.Collections.Generic;
using Donuts;
using UnityEngine;

namespace Level
{
    public class Cell : MonoBehaviour
    {
        public bool IsActive { get; set; }
        public DonutStack DonutStack { get; set; }

    }
}