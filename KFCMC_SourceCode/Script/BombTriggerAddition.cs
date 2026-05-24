using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTriggerAddition : MonoBehaviour {

    public BombTrigger bombTrigger;
    public string targetTag = "";

    private void OnTriggerStay(Collider other) {
        if (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) {
            bombTrigger.Burst();
        }
    }
}
