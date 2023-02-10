using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageSlot : MonoBehaviour {

    public RectTransform rectTransform;
    public TMP_Text content;
    public Image backSimple;
    public Image backLeft;
    public Image backRight;
    public Image faceImage;
    public TMP_Text faceName;
    public int slotType;
    [System.NonSerialized]
    public int frameCount;
    
}
