using UnityEngine;
using Zenject;

namespace Donuts
{
    public class DonutFactory
    {
        private GameObject _donutPrefab;
        private DiContainer _container;
        private Transform _creationPosition;

        public DonutFactory(GameObject donutPrefab, DiContainer container, Transform creationPosition)
        {
            _donutPrefab = donutPrefab;
            _container = container;
            _creationPosition = creationPosition;
        }

        public void Create()
        {
            _container.InstantiatePrefab(_donutPrefab,_creationPosition.position,Quaternion.identity,null);
        }
    }
}