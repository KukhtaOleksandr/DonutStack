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
            another.DonutStack=DonutStack;
            DonutStack = null;
        }

    }
}