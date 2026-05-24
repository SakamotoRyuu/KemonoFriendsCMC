using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActivateOnSecondLap : MonoBehaviour {

    public GameObject[] activateObjs;
    public TMP_Text text;
    public string dicKey;

    private void Start() {
        if (GameManager.Instance && GameManager.Instance.isSecondLap) {
            if (text && !string.IsNullOrEmpty(dicKey) && TextManager.IsInitialized) {
                text.text = TextManager.Get(dicKey);
            }
            for (int i = 0; i < activateObjs.Length; i++) {
                if (activateObjs[i]) {
                    activateObjs[i].SetActive(true);
                }
            }
        }
    }

}
