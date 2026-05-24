using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRenderQueueShared : MonoBehaviour {

    public Renderer rend;
    public int matIndex;
    public int renderQueue;

    void Start() {
        if (rend) {
            if (matIndex < 0) {
                rend.sharedMaterial.renderQueue = renderQueue;
            } else {
                Material[] mats = rend.sharedMaterials;
                mats[matIndex].renderQueue = renderQueue;
                rend.sharedMaterials = mats;
            }
        }
    }
}
