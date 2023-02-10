using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysicsDeactivate : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject bulletPhysicsObj = GameObject.Find("MMD4MecanimBulletPhysics");
        if (bulletPhysicsObj) {
            bulletPhysicsObj.SetActive(false);
        }
        Destroy(gameObject);
	}
}
