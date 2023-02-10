using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileControl : MonoBehaviour {

    public GameObject[] modObj;

    public void SetTile(int index) {
        if (modObj.Length > 0) {
            index = index % modObj.Length;
            for (int i = 0; i < modObj.Length; i++) {
                if (modObj[i]) {
                    if (i == index) {
                        modObj[i].SetActive(true);
                    } else {
                        Destroy(modObj[i]);
                    }
                }
            }
        }
    }
}
