using System;
using System.Collections.Generic;
using Donuts.StateMachine;
using Level;
using UnityEngine;
using Zenject;

namespace Donuts
{
    public class DonutFactory : IInitializable, IDisposable
    {
        private List<DonutStack> _donutPrefabs;
        private SignalBus _signalBus;
        private DiContainer _container;
        private Transform _creationPosition;

        public DonutStack CurrentDonutStack { get; private set; }

        public DonutFactory(List<DonutStack> donutPrefabs, DiContainer container, Transform creationPosition, SignalBus signalBus)
        {
            _donutPrefabs = donutPrefabs;
            _container = container;
            _creationPosition = creationPosition;
            _signalBus = signalBus;
            Create();
        }

        public void Initialize()
        {
            _signalBus.Subscribe<SignalConnectionFinished>(Create);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<SignalConnectionFinished>(Create);
        }

        private void Create()
        {
            DonutStack donutPrefab= _donutPrefabs[UnityEngine.Random.Range(0,_donutPrefabs.Count)];
            CurrentDonutStack= _container.InstantiatePrefabForComponent<DonutStack>
                            (donutPrefab, _creationPosition.position, Quaternion.identity, null);
        }
    }
}