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

        public async void MoveDonutToAnotherCell(Cell another)
        {
            await DonutStack.MoveDonutStack(another.transform.position);
            another.DonutStack = DonutStack;
            DonutStack = null;
        }

        public bool CanTransferDonutTo(Cell another)
        {
            if (another.DonutStack.FreeDonutPlaces > 0 && DonutStack.FreeDonutPlaces < 3)
                if (another.DonutStack.GetTopDonut().Type == DonutStack.GetTopDonut().Type)
                    return true;
            return false;
        }

    }
}