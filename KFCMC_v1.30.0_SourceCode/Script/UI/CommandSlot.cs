using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandSlot : MonoBehaviour {

    public GameObject targetMark;
    public TMP_Text text;
    public TMP_ColorGradient[] colorGradient;
    int commandIndex;

    public void SetCommandText(CharacterBase.Command command) {
        commandIndex = (int)command;
        text.colorGradientPreset = colorGradient[commandIndex];
        text.text = TextManager.Get("COMMAND_" + commandIndex.ToString());
    }

    public void ResetCommandText() {
        text.text = TextManager.Get("COMMAND_" + commandIndex.ToString());
    }

}
