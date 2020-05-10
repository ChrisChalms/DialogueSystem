#pragma warning disable 649

using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    private bool _canBeTriggeredMultipleTimes;
    [SerializeField]
    private LayerMask _triggerableLayers;
    [SerializeField]
    private UnityEvent _onEnterEvent, _onExitEvent;

    private bool _triggered;

    #region MonoBehaviour

    // Entries
    private void OnTriggerEnter2D(Collider2D col) => checkLayer(col.gameObject.layer, true);
    private void OnTriggerEnter(Collider col) => checkLayer(col.gameObject.layer, true);

    // Exits
    private void OnTriggerExit2D(Collider2D col) => checkLayer(col.gameObject.layer, false);
    private void OnTriggerExit(Collider col) => checkLayer(col.gameObject.layer, false);

    #endregion

    // Check if the layer is part of the layermask
    private void checkLayer(int layer, bool entry)
    {
        // Early out if we're not allowed to trigger
        if(!_canBeTriggeredMultipleTimes)
        {
            if (_triggered)
                return;
            else
                _triggered = true;
        }

        // Check the layer mask
        if ((_triggerableLayers & 1 << layer) == 1 << layer)
        {
            // Successfully triggered, invoke event
            if (entry)
                _onEnterEvent.Invoke();
            else
                _onExitEvent.Invoke();
        }
    }
}
