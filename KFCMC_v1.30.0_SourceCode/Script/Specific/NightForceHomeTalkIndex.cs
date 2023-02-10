using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightForceHomeTalkIndex : MonoBehaviour {

    public int newHomeTalkIndex = 2;
    bool forced = false;
	
	void Update () {
		if (!forced && LightingDatabase.Instance.IsNight) {
            ItemCharacter itemChar = GetComponent<ItemCharacter>();
            if (itemChar) {
                itemChar.ForceHomeTalkIndex(newHomeTalkIndex);
            }
            forced = true;
        }
	}
}
