using Donuts;
using Input;
using UnityEngine;
using Zenject;

namespace Level
{
    public class RowsClickHandler : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;
        [Inject] private SignalBus _signalBus;
        [Inject] private ConnectionHandler _connectionHandler;
        [Inject] private DonutFactory _donutFactory;

        void OnEnable()
        {
            _signalBus.Subscribe<SignalMouseClicked>(HandleClick);
        }

        void OnDisable()
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
                _connectionHandler.CurrentCell=freeCell;
                _connectionHandler.CurrentRow=row;
                _connectionHandler.CurrentCell.DonutStack=_donutFactory.CurrentDonutStack;
                _signalBus.Fire<SignalRowClicked>(new SignalRowClicked() { Position = freeCell.transform.position });
            }
        }
    }
}