using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Toudai : MonoBehaviour {

    public GameObject activateTarget;
    public GameObject deactivateTarget;
    public GameManager.SecretType secretType;
    GameObject mapChip;

    private void Start() {
        mapChip = Instantiate(MapDatabase.Instance.prefab[MapDatabase.other], transform);
    }

    private void OnTriggerEnter(Collider other) {
        if (activateTarget && !activateTarget.activeSelf && other.CompareTag("PlayerAttackDetection")) {
            AttackDetection attack = other.GetComponent<AttackDetection>();
            if (attack && attack.elementalAttribute != AttackDetection.ElementalAttribute.None) {
                activateTarget.SetActive(true);
                if (deactivateTarget) {
                    deactivateTarget.SetActive(false);
                }
                if (mapChip) {
                    mapChip.SetActive(false);
                }
                GameManager.Instance.SetSecret(secretType);
            }
        }
    }

}
