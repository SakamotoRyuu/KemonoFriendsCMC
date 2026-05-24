using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInertIFR : MonoBehaviour {
    
	void Awake () {
		if (CharacterManager.Instance) {
            CharacterManager.Instance.inertIFR = true;
        }
	}
}
