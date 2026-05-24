using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateEffectToFriendsOnTrigger : MonoBehaviour {

    public GameObject scaffoldEffect;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase) {
                fBase.SetScaffoldEffect(true, scaffoldEffect);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            FriendsBase fBase = other.GetComponent<FriendsBase>();
            if (fBase) {
                fBase.SetScaffoldEffect(false, null);
            }
        }
    }

    private void OnDisable() {
        if (CharacterManager.Instance) {
            CharacterManager.Instance.DestroyAllFriendsScaffold();
        }
    }

}
