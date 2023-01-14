using System.Collections.Generic;
using System.Threading.Tasks;
using Donuts;
using UnityEngine;

namespace Level
{
    public class Cell : MonoBehaviour
    {
        public DonutStack DonutStack { get; set; }

        public void DestroyDonutStack()
        {
            GameObject.Destroy(DonutStack.gameObject);
        }

        public bool isMergingGrantsFullStack(Cell other)
        {
            List<Donut> donuts1 = DonutStack.GetTopDonutsOfOneType();
            List<Donut> donuts2 = other.DonutStack.GetTopDonutsOfOneType();
            if (donuts1[donuts1.Count - 1].Type == donuts2[donuts2.Count - 1].Type)
            {
                if (donuts1.Count + donuts2.Count >= 3)
                {
                if (other.AreOtherTypesInStack() == false || AreOtherTypesInStack() == false)
                   return true;
                }
            }
            return false;
        }

        public async Task MoveDonutToAnotherCell(Cell another)
        {
            await DonutStack.MoveDonutStack(another.transform.position);
            another.DonutStack = DonutStack;
            DonutStack = null;
        }

        public bool CanTransferDonutTo(Cell to)
        {
            if (to.DonutStack == null || DonutStack == null)
                return false;
            if (DonutStack.FreeDonutPlaces < 3 && to.DonutStack.FreeDonutPlaces > 0)
                if (to.DonutStack.GetTopDonut().Type == DonutStack.GetTopDonut().Type)
                    return true;
            return false;
        }

        private bool AreOtherTypesInStack()
        {
            return DonutStack.DonutsCount > DonutStack.GetTopDonutsOfOneType().Count;
        }

    }
}