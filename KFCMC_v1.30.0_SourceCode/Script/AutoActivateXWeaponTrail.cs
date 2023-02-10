using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class AutoActivateXWeaponTrail : MonoBehaviour {

	// Use this for initialization
	void Start () {
        XWeaponTrail xwt = GetComponent<XWeaponTrail>();
        if (xwt) {
            xwt.Init();
            xwt.Activate();
        }
	}
}
