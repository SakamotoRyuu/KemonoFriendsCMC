using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMessageOnTriggerEnter : MonoBehaviour
{

    public string textKey;
    public float messageTime = 3.5f;
    public int slotType;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter")) {
            if (MessageUI.Instance) {
                MessageUI.Instance.SetMessage(TextManager.Get(textKey), messageTime, MessageUI.panelType_Information, slotType);
            }
        }
    }

}
