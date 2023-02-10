using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerSlot : EventTrigger {

    public int slotIndex;
    const int paramBase = 100;

    public override void OnPointerEnter(PointerEventData eventData) {
        if (PauseController.Instance) {
            PauseController.Instance.EventEnter(paramBase + slotIndex);
        }
    }

    public override void OnPointerClick(PointerEventData eventData) {
        if (PauseController.Instance) {
            PauseController.Instance.EventClick(paramBase + slotIndex);
        }
    }

}
