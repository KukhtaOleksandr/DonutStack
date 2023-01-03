using UnityEngine;
using Zenject;

namespace Input
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [Inject] private SignalBus _signalBus;

        void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _signalBus.Fire(new SignalMouseClicked() { MousePosition = UnityEngine.Input.mousePosition });
            }
        }
    }
}