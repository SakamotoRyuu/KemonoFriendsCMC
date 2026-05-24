using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour {

    public Material[] changeMat;
    public int matIndex;

	// Use this for initialization
	void Start () {
        Renderer rend = GetComponent<Renderer>();
        Material[] materials = rend.materials;
        materials[matIndex] = changeMat[Random.Range(0, changeMat.Length)];
        rend.materials = materials;
	}
}
