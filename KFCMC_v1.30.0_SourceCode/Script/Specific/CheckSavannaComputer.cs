using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSavannaComputer : MonoBehaviour {

    public SavannaComputer savannaComputer;
    public GameObject activateTarget;

    private bool completed = false;
	
	// Update is called once per frame
	void Update () {
		if (!completed && savannaComputer && savannaComputer.missionCompleted) {
            completed = true;
            activateTarget.SetActive(true);
        }
	}
}
