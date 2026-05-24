using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnSuperman : MonoBehaviour {

    public GameObject[] targetObjects;

    CharacterBase parentCBase;
    bool stateSave = false;
    
	void Awake () {
        parentCBase = GetComponentInParent<CharacterBase>();
        stateSave = (parentCBase && parentCBase.isSuperman);
        for (int i = 0; i < targetObjects.Length; i++) {
            if (targetObjects[i]) {
                targetObjects[i].SetActive(stateSave);
            }
        }
	}

    void Update() {
        bool flag = (parentCBase && parentCBase.isSuperman);
        if (stateSave != flag) {
            stateSave = flag;
            for (int i = 0; i < targetObjects.Length; i++) {
                if (targetObjects[i]) {
                    targetObjects[i].SetActive(stateSave);
                }
            }
        }
    }
}
