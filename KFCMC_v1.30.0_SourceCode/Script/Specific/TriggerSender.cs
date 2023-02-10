using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSender : MonoBehaviour {

    public TriggerReceiver receiver;
    public int index;

    private void OnTriggerEnter(Collider other) {
        if (receiver) {
            receiver.Receive(index, other);
        }
    }

}
