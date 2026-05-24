using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLocalPositionForWeapon : MonoBehaviour {

    public Vector3[] localPosEachClawType;
    int clawTypeSave = -1;
	
	// Update is called once per frame
	void Update () {
		if (CharacterManager.Instance) {
            int clawTemp = CharacterManager.Instance.GetClawType();
            if (clawTypeSave != clawTemp) {
                clawTypeSave = clawTemp;
                transform.localPosition = localPosEachClawType[clawTypeSave];
            }
        }
	}
}
