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

        public List<DonutType> SimulatedDonuts { get; set; }
        public int FreeDonutPlaces { get; set; }
        public int DonutsCount { get => _donuts.Count; }
        public GameObject Cylinder { get => _cylinder; }

        [Inject] private DonutsChangeHandler donutsChangeHandler;
        [SerializeField] private GameObject _cylinder;

        private List<Donut> _donuts;
        private float _duration;

        void Start()
        {
            _donuts = GetComponentsInChildren<Donut>().ToList();
            _donuts = _donuts.OrderBy(d => d.transform.position.y).ToList();

            ResetSimulatedDonuts();
            FreeDonutPlaces = MaxDonutSpaces - _donuts.Count;
        }
        public async Task RemoveDonut(Donut donut, DonutStack to)
        {
            _donuts.Remove(donut);
            donut.transform.parent = to.transform;
            FreeDonutPlaces++;
            await Task.Delay(200);
            await donutsChangeHandler.Handle(this);
            ResetSimulatedDonuts();
        }

        public async Task AddDonut(Donut donut)
        {
            _donuts.Add(donut);
            FreeDonutPlaces--;
            await donutsChangeHandler.Handle(this);
            ResetSimulatedDonuts();
        }

        public void ResetSimulatedDonuts()
        {
            SimulatedDonuts = new();
            foreach (Donut donut in _donuts)
            {
                SimulatedDonuts.Add(donut.Type);
            }
        }

        public bool IsFull()
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

        public List<DonutType> GetTopSimulatedDonutsOfOneType()
        {
            List<DonutType> donuts = new(SimulatedDonuts);
            List<DonutType> result = new List<DonutType>();
            donuts.Reverse();
            DonutType topDonutType = GetTopSimulatedDonut();
            foreach (DonutType donut in donuts)
            {
                if (donut == topDonutType)
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

        public Donut GetTopDonut()
        {
            return _donuts[_donuts.Count - 1];
        }

        public DonutType GetTopSimulatedDonut()
        {
            return SimulatedDonuts[SimulatedDonuts.Count - 1];
        }

        public async Task MoveDonutStack(Vector3 position)
        {
            Vector3 movePosition = new Vector3(position.x, 0, position.z);

            _duration = Vector3.Distance(transform.position, movePosition) / Speed;
            _duration = Mathf.Clamp(_duration, 0.35f, 0.75f);

            transform.DOMove(movePosition, _duration);
            await Task.Delay((int)(_duration * 1000));
        }
    }
}