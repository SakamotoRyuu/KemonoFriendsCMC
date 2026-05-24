using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAndEnable : MonoBehaviour {

    public Behaviour[] targetComponent;
    public float waitTime;

    private void Start() {
        StartCoroutine(EnablizeAction());        
    }    

    IEnumerator EnablizeAction() {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < targetComponent.Length; i++) {
            if (targetComponent[i]) {
                targetComponent[i].enabled = true;
            }
        }
        enabled = false;
    }

}
