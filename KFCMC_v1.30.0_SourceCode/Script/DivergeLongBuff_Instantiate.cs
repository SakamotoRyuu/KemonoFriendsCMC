using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivergeLongBuff_Instantiate : MonoBehaviour {

    public GameObject[] prefab;
    public Transform[] parent;

    private void Awake() {
        if (CharacterManager.Instance && CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long)) {
            for (int i = 0; i < prefab.Length; i++) {
                if (prefab[i]) {
                    if (i < parent.Length && parent[i]) {
                        Instantiate(prefab[i], parent[i]);
                    } else {
                        Instantiate(prefab[i]);
                    }
                }
            }
        }
    }

}
