using Input;
using UnityEngine;
using Zenject;

namespace Rows
{
    public class RowsClickHandler : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;
        [Inject] private SignalBus _signalBus;

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
                Vector3 cellPosition = hit.transform.GetComponent<Row>().GetFreeCellPosition();
                _signalBus.Fire<SignalRowClicked>(new SignalRowClicked() { Position = cellPosition });
            }
        }
    }
}