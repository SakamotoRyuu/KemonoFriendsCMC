using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderQueue : MonoBehaviour {
    
    public int matIndex;
    public int renderQueue = 3001;
    
	void Start () {
        Renderer rend = GetComponent<Renderer>();
        if (rend) {
            Material[] mats = rend.materials;
            if (matIndex < 0) {
                for (int i = 0; i < mats.Length; i++) {
                    mats[i].renderQueue = renderQueue;
                }
                rend.materials = mats;
            } else {
                if (mats.Length > matIndex) {
                    mats[matIndex].renderQueue = renderQueue;
                    rend.materials = mats;
                }
            }
        }
	}
}
