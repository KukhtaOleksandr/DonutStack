using System.Threading.Tasks;
using DG.Tweening;
using Donuts;
using UnityEngine;

namespace Level
{
    public class Transfer
    {
        private const float Offset = 0.3f;
        public Cell From { get; set; }
        public Cell To { get; set; }
        public bool Bipolar { get; set; }

        private async Task TransferDonut()
        {
            Donut fromTopDonut = From.DonutStack.GetTopDonut();
            Vector3 newPosition = new Vector3(To.DonutStack.GetTopDonut().transform.position.x,
                                             To.DonutStack.GetTopDonut().transform.position.y + Offset,
                                             To.DonutStack.GetTopDonut().transform.position.z);

            DoAnimation(From, To, fromTopDonut, newPosition);
            await Task.Delay(500);
        }

        private void DoAnimation(Cell from, Cell to, Donut fromTopDonut, Vector3 newPosition)
        {
            if (IsTransferringLastDonut(from))
                GameObject.Destroy(from.DonutStack.Cylinder);
            fromTopDonut.transform.DOMove(newPosition, 0.5f).OnComplete(() => 
            CompleteTransfer(fromTopDonut, from.DonutStack, to.DonutStack));
            fromTopDonut.transform.DOScale(to.DonutStack.GetTopDonut().transform.localScale * 0.85f, 0.5f);
        }

        private static bool IsTransferringLastDonut(Cell from)
        {
            return from.DonutStack.FreeDonutPlaces == 2;
        }

        private void CompleteTransfer(Donut fromTopDonut, DonutStack fromDonutStack, DonutStack toDonutStack)
        {
            fromDonutStack.RemoveDonut(fromTopDonut);
            toDonutStack.AddDonut(fromTopDonut);
            fromTopDonut.transform.parent = toDonutStack.transform;
        }
    }
}