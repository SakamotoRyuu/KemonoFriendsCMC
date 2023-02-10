using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTrap : MonoBehaviour {
    
    [System.Serializable]
    public class TrapSet {
        public GameObject prefab;
        public int weighting = 1;
    }

    public TrapSet[] trapSet;

    // Use this for initialization
    void Start() {
        int[] accumulate = new int[trapSet.Length];
        accumulate[0] = trapSet[0].weighting;
        for (int i = 1; i < accumulate.Length; i++) {
            accumulate[i] = accumulate[i - 1] + trapSet[i].weighting;
        }
        int pointer = Random.Range(0, accumulate[accumulate.Length - 1]);
        for (int i = 0; i < accumulate.Length; i++) {
            if (pointer < accumulate[i]) {
                if (trapSet[i].prefab) {
                    Instantiate(trapSet[i].prefab, transform.position, Quaternion.identity, transform);
                }
                break;
            }
        }
    }
}
