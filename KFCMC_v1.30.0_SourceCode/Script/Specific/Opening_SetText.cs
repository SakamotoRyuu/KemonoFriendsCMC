using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Opening_SetText : MonoBehaviour {

    public TMP_Text[] targetText;
    public string[] keyName;
    
	void Start () {
        if (TextManager.IsInitialized) {
            for (int i = 0; i < targetText.Length; i++) {
                targetText[i].text = TextManager.Get(keyName[i]);
            }
        }
	}
	
}
