using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStartOnStart : MonoBehaviour {

    public AttackDetection attackDetection;
    
	void Start () {
        attackDetection.DetectionStart();
	}
}
