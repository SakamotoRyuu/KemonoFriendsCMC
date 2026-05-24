using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour {

    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;

	void Start () {
        float angleX = rotateX ? Random.Range(-180f, 180f) : transform.localEulerAngles.x;
        float angleY = rotateY ? Random.Range(-180f, 180f) : transform.localEulerAngles.y;
        float angleZ = rotateZ ? Random.Range(-180f, 180f) : transform.localEulerAngles.z;
        transform.localEulerAngles = new Vector3(angleX, angleY, angleZ);
	}
}
