using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour {

    public Vector3 posRange;

	void Start () {
        transform.position += new Vector3(Random.Range(posRange.x * -0.5f, posRange.x * 0.5f), Random.Range(posRange.y * -0.5f, posRange.y * 0.5f), Random.Range(posRange.z * -0.5f, posRange.z * 0.5f));
	}
}
