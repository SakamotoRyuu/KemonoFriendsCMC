using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePlayerLightOnTriggerEnter : MonoBehaviour {

    public int lightType;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ItemGetter") && CharacterManager.Instance) {
            CharacterManager.Instance.SetPlayerLightActiveTemporal(true, lightType);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ItemGetter") && CharacterManager.Instance) {
            CharacterManager.Instance.SetPlayerLightActiveTemporal(false, lightType);
        }
    }

}
