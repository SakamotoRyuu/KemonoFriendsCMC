using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionManager : MonoBehaviour {

    public TMP_Text text;
    public TMP_Text mod;
    
	void Start () {
        if (GameManager.version % 100 == 0) {
            text.text = string.Format("v{0}.{1}.{2}", GameManager.version / 1000000, GameManager.version / 10000 % 100, GameManager.version / 100 % 100);
        } else {
            text.text = string.Format("v{0}.{1}.{2}test{3}", GameManager.version / 1000000, GameManager.version / 10000 % 100, GameManager.version / 100 % 100, GameManager.version % 100);
        }
        if (GameManager.Instance && GameManager.Instance.modFlag) {
            mod.enabled = true;
        }
	}
	
}
