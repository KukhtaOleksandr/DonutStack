using System.Collections.Generic;
using Donuts;
using UnityEngine;

namespace Level
{
    public class Cell : MonoBehaviour
    {
        public bool IsActive { get; set; }
        public int index { get; set; }
        public List<DonutType> Donuts {get; set;}
    }
}