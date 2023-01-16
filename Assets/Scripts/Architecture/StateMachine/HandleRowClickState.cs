using Donuts;
using Input;
using Level;
using StateMachine.Base;
using UnityEngine;
using Zenject;

namespace Architecture.StateMachine
{
    public class HandleRowClickState : IState
    {
        [Inject] private Camera _camera;
        [Inject] private LayerMask _layerMask;
        [Inject] private SignalBus _signalBus;
        [Inject] private ConnectionHandler _connectionHandler;
        [Inject] private DonutFactory _donutFactory;

        public void Enter()
        {
            _signalBus.Subscribe<SignalMouseClicked>(HandleClick);
        }

        public void Exit()
        {
            _signalBus.Unsubscribe<SignalMouseClicked>(HandleClick);
        }

        private void HandleClick(SignalMouseClicked args)
        {
            Ray ray = _camera.ScreenPointToRay(args.MousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, _layerMask))
            {
                Row row = hit.transform.GetComponent<Row>();
                Cell freeCell = row.GetFreeCell();

                InitializeConnectionHandler(row, freeCell);

                _signalBus.Fire<SignalRowClicked>(new SignalRowClicked() { Position = freeCell.transform.position });
                _signalBus.Fire<SignalChangedState>(new SignalChangedState() { State = new InConnectionState() });
            }
        }

        private void InitializeConnectionHandler(Row row, Cell freeCell)
        {
            _connectionHandler.CurrentCell = freeCell;
            _connectionHandler.CurrentRow = row;
            _connectionHandler.CurrentCell.DonutStack = _donutFactory.CurrentDonutStack;
        }
    }
}