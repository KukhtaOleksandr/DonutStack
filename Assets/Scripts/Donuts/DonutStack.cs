using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Donuts
{
    public class DonutStack : MonoBehaviour
    {
        private const float Offset = 0.3f;

        public List<Donut> Donuts { get; private set; }

        void Start()
        {
            Donuts = GetComponentsInChildren<Donut>().ToList();
            Debug.Log(Donuts.Count);
        }

        public Donut GetTopDonut()
        {
            return Donuts[Donuts.Count - 1];
        }

        public async Task TransferTopDonutToOther(DonutStack donutStack)
        {
            Donut topDonut = GetTopDonut();
            Vector3 newPosition =new Vector3(donutStack.GetTopDonut().transform.position.x, 
                                            donutStack.GetTopDonut().transform.position.y + Offset, 
                                            donutStack.GetTopDonut().transform.position.z);

            topDonut.transform.DOMove(newPosition,0.5f).OnComplete(()=>CompleteTransfer(topDonut,donutStack));
            topDonut.transform.DOScale(donutStack.GetTopDonut().transform.localScale*0.85f,0.5f);
            donutStack.Donuts.Add(topDonut);
            await Task.Delay(500);
        }

        private void CompleteTransfer(Donut topDonut,DonutStack donutStack)
        {
            topDonut.transform.parent = donutStack.transform;
            Donuts.Remove(topDonut);
        }
    }
}