using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveObjectsComponents : MonoBehaviour {

    public GameObject[] objects;
    public Behaviour[] components;
    public bool childColliders = true;

    public void SetActiveAssigned(bool flag) {
        for (int i = 0; i < objects.Length; i++) {
            if (objects[i]) {
                objects[i].SetActive(flag);
            }
        }
        for (int i = 0; i < components.Length; i++) {
            if (components[i]) {
                components[i].enabled = flag;
            }
        }
        if (childColliders) {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++) {
                if (colliders[i]) {
                    colliders[i].enabled = flag;
                }
            }
        }
    }
}
