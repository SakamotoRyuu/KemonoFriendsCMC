using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEndDestroy : MonoBehaviour {

    private CharacterBase cBase;

    private void Awake() {
        cBase = GetComponentInParent<CharacterBase>();
    }
    
    void Update () {
		if (cBase == null || !cBase.isSuperman) {
            Destroy(gameObject);
        }
	}
}
