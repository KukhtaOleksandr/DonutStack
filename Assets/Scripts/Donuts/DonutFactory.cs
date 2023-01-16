using System;
using System.Collections.Generic;
using Donuts.StateMachine;
using Level;
using UnityEngine;
using Zenject;

namespace Donuts
{
    public class DonutFactory
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
        }

        public void Create()
        {
            DonutStack donutPrefab= _donutPrefabs[UnityEngine.Random.Range(0,_donutPrefabs.Count)];
            CurrentDonutStack= _container.InstantiatePrefabForComponent<DonutStack>
                            (donutPrefab, _creationPosition.position, Quaternion.identity, null);
        }
    }
}