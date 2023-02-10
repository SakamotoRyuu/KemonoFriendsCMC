using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerByPlayerCheck : MonoBehaviour {

    public bool flag = false;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == CharacterManager.Instance.playerObj) {
            flag = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject == CharacterManager.Instance.playerObj) {
            flag = false;
        }
    }

}
