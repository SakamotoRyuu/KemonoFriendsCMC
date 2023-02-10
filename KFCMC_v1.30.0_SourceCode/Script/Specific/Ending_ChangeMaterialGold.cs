using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending_ChangeMaterialGold : MonoBehaviour {

    [System.Serializable]
    public struct ChangeMatSet {
        public int index;
        public Material goldMaterial;
    }

    public Renderer rend;
    public ChangeMatSet[] changeMatSet;

    void Start() {
        if (GameManager.Instance && GameManager.Instance.minmiGolden) {
            if (rend == null) {
                rend = GetComponent<Renderer>();
            }
            if (rend) {
                Material[] mats = rend.materials;
                for (int i = 0; i < changeMatSet.Length; i++) {
                    int renderQueue = mats[changeMatSet[i].index].renderQueue;
                    mats[changeMatSet[i].index] = changeMatSet[i].goldMaterial;
                    mats[changeMatSet[i].index].renderQueue = renderQueue;
                }
                rend.materials = mats;
            }
        }
    }

}
