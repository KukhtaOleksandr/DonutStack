using Donuts;
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

            if(Physics.Raycast(ray,out hit,100,_layerMask))
            {
                hit.transform.GetComponent<MeshRenderer>().enabled=true;
                _donutFactory.Create();
            }
        }
    }
}