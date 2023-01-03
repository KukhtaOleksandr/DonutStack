using System;
using Donuts.StateMachine;
using UnityEngine;
using Zenject;

namespace Donuts
{
    public class DonutFactory : IInitializable, IDisposable
    {
        private GameObject _donutPrefab;
        private SignalBus _signalBus;
        private DiContainer _container;
        private Transform _creationPosition;

        public DonutFactory(GameObject donutPrefab, DiContainer container, Transform creationPosition, SignalBus signalBus = null)
        {
            _donutPrefab = donutPrefab;
            _container = container;
            _creationPosition = creationPosition;
            _signalBus = signalBus;
            Create();
        }

        public void Initialize()
        {
            _signalBus.Subscribe<SignalDonutMovedToPosition>(Create);
        } 

        public void Dispose()
        {
            _signalBus.Unsubscribe<SignalDonutMovedToPosition>(Create);
        }

        private void Create()
        {
            _container.InstantiatePrefab(_donutPrefab,_creationPosition.position,Quaternion.identity,null);
        }
    }
}