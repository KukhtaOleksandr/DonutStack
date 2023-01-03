using StateMachine.Base;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Donuts.StateMachine
{
    public class MoveToPositionState : IState
    {
        private const float Speed = 5; 

        [Inject]
        private SignalBus _signalBus;
        [Inject]
        private Transform _donut;

        public Vector3 PositionToMove { get; set; }

        public void Enter()
        {
            MoveAnimation();
        }


        public void Exit()
        {

        }

        private void MoveAnimation()
        {
            _donut.position = new Vector3(PositionToMove.x, 0, _donut.position.z+1);
            Vector3 movePosition = new Vector3(PositionToMove.x, 0, PositionToMove.z);
            
            float duration = Vector3.Distance(_donut.position,movePosition)/Speed;
            duration = Mathf.Clamp(duration,0.35f,0.75f);
            Debug.Log(duration);
            
            _donut.DOMove(movePosition, duration).OnComplete(() => _signalBus.Fire<SignalDonutMovedToPosition>());
        }
    }
}