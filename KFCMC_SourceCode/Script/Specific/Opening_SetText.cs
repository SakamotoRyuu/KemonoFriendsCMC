using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Opening_SetText : MonoBehaviour {

    public TMP_Text[] targetText;
    public string[] keyName;
    public bool changeLangEnabled;
    
    int langSave;
    
	void Start () {
        if (TextManager.IsInitialized) {
            for (int i = 0; i < targetText.Length; i++) {
                targetText[i].text = TextManager.Get(keyName[i]);
            }
        }
        if (changeLangEnabled)
        {
            langSave = GameManager.Instance.save.language;
        }
        else
        {
            enabled = false;
        }
	}

    void Update()
    {
        if (changeLangEnabled && GameManager.Instance && GameManager.Instance.save != null && GameManager.Instance.save.language != langSave)
        {
            langSave = GameManager.Instance.save.language;
            if (TextManager.IsInitialized)
            {
                for (int i = 0; i < targetText.Length; i++)
                {
                    targetText[i].text = TextManager.Get(keyName[i]);
                }
            }
        }
    }
	
}
