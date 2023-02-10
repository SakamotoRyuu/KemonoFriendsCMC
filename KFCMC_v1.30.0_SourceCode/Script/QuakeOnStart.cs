using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuakeOnStart : MonoBehaviour {

    public float amplitude = 10;
    public float frequency = 4;
    public float attackTime = 0;
    public float sustainTime = 0;
    public float decayTime = 1;
    public float impactRadius = 1;
    public float dissipationDistance = 100;
    
	void Start () {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetQuake(transform.position, amplitude, frequency, attackTime, sustainTime, decayTime, impactRadius, dissipationDistance);
        }
	}
	
}
