using UnityEngine;
using Zenject;

namespace Rows
{
    public class RowsClickHandler : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        
    }
}