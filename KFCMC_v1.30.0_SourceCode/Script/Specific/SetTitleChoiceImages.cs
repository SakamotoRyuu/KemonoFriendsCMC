using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTitleChoiceImages : MonoBehaviour {

    [System.Serializable]
    public class LanguageSprite {
        public Sprite[] sprite;
    }

    public Image[] image;
    public LanguageSprite[] languageSprite;
    public int defaultLanguage;
    
    void Start () {
        // GameManager.Instance.InitLanguage();
        int index = GameManager.Instance.save.language;
        if (!(index >= 0 && index < languageSprite.Length)) {
            index = defaultLanguage;
        }
		for (int i = 0; i < image.Length; i++) {
            image[i].sprite = languageSprite[index].sprite[i];
        }
	}
}
