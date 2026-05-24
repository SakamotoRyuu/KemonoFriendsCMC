using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rewired.UI.ControlMapper;

public class ControlMapperWrapper : MonoBehaviour
{

    public ControlMapper controlMapper;
    public GameObject controlMapperActiveCheck;
    public TMP_Text controlInformationText;
    public LanguageData[] languageData;
    public ThemeSettings[] themeSettings;

    public void InitializeTexts() {
        controlMapper.language = languageData[Mathf.Clamp(GameManager.Instance.save.language, 0, languageData.Length - 1)];
        controlMapper.themeArrange = themeSettings[Mathf.Clamp(GameManager.Instance.save.language, 0, themeSettings.Length - 1)];
        controlInformationText.text = TextManager.Get("CONFIG_KEYCONFIG");
    }
    
    public void Open() {
        controlMapper.Open();
    }

    public bool IsControlMapperActive {
        get {
            return controlMapper && controlMapperActiveCheck.activeSelf;
        }
    }

}
