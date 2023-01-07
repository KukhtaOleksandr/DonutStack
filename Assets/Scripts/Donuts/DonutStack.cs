using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;

namespace Donuts
{
    public class DonutStack : MonoBehaviour
    {
        private const float Speed = 5;
        private const int MaxDonutSpaces = 3;

        public int FreeDonutPlaces { get; set; }
        public int DonutsCount { get => _donuts.Count; }
        public GameObject Cylinder { get => _cylinder; }

        [Inject] private SignalBus _signalBus;
        [SerializeField] private GameObject _cylinder;

        private List<Donut> _donuts;

        void Start()
        {
            _donuts = GetComponentsInChildren<Donut>().ToList();
            _donuts = _donuts.OrderBy(d => d.transform.position.y).ToList();
            FreeDonutPlaces = MaxDonutSpaces - _donuts.Count;
        }

        public void RemoveDonut(Donut donut)
        {
            _donuts.Remove(donut);
            FreeDonutPlaces++;
            if (FreeDonutPlaces == 3)
                _signalBus.Fire<SignalDonutStackIsEmpty>(new SignalDonutStackIsEmpty() { DonutStack = this });
        }

        public void AddDonut(Donut donut)
        {
            _donuts.Add(donut);
            FreeDonutPlaces--;
            if (IsDonutStackFull())
                _signalBus.Fire<SignalDonutStackIsFull>(new SignalDonutStackIsFull() { DonutStack = this });
        }

        private bool IsDonutStackFull()
        {
            return FreeDonutPlaces == 0 && GetTopDonutsOfOneType().Count == 3;
        }

        public List<Donut> GetTopDonutsOfOneType()
        {
            List<Donut> donuts = new List<Donut>(_donuts);
            List<Donut> result = new List<Donut>();
            donuts.Reverse();
            DonutType topDonutType = GetTopDonut().Type;
            foreach (Donut donut in donuts)
            {
                if (donut.Type == topDonutType)
                {
                    result.Add(donut);
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        public int GetCountOfTopDonutsOfOneType()
        {
            return GetTopDonutsOfOneType().Count;
        }

        public Donut GetTopDonut()
        {
            return _donuts[_donuts.Count - 1];
        }

        public async Task MoveDonutStack(Vector3 position)
        {
            Vector3 movePosition = new Vector3(position.x, 0, position.z);

            float duration = Vector3.Distance(transform.position, movePosition) / Speed;
            duration = Mathf.Clamp(duration, 0.35f, 0.75f);

            transform.DOMove(movePosition, duration);
            await Task.Delay((int)(duration * 1000));
        }
    }
}