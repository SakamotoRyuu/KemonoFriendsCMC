using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAttackEnabledToActivate : MonoBehaviour {

    public GameObject activateTarget;
    public AttackDetection attackDetection;
    
	void Start () {
        if (attackDetection == null) {
            attackDetection = GetComponentInParent<AttackDetection>();
        }
        if (activateTarget) {
            activateTarget.SetActive(false);
        }
	}
	
	void Update () {
		if (activateTarget && attackDetection && attackDetection.attackEnabled != activateTarget.activeSelf) {
            activateTarget.SetActive(attackDetection.attackEnabled);
        }
	}
}
