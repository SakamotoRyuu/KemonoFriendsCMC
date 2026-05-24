using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SetCastShadows : MonoBehaviour {

    public ShadowCastingMode shadowCastingMode;
    
	void Start () {
        Renderer rend = GetComponent<Renderer>();
        if (rend) {
            rend.shadowCastingMode = shadowCastingMode;
        }
	}
}
