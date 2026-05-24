using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeOnStart : MonoBehaviour {

    public Renderer rend;
    public int index;
    public Material mat;

	// Use this for initialization
	void Start () {
		if (rend) {
            Material[] rendMats = rend.materials;
            rendMats[index] = mat;
            rend.materials = rendMats;
        }
	}
}
