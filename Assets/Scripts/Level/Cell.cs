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

        public async Task MoveDonutToAnotherCell(Cell another)
        {
            await DonutStack.MoveDonutStack(another.transform.position);
            another.DonutStack = DonutStack;
            DonutStack = null;
        }

        public bool CanTransferDonutTo(Cell to)
        {
            if (DonutStack.FreeDonutPlaces < 3 && to.DonutStack.FreeDonutPlaces > 0)
                if (to.DonutStack.GetTopDonut().Type == DonutStack.GetTopDonut().Type)
                    return true;
            return false;
        }

    }
}