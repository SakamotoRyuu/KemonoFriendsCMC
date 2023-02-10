using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : SingletonMonoBehaviour<MessageUI> {

    public Transform[] panel;
    public GameObject[] slotPrefab;
    public Sprite[] twoToneLeft;
    public Sprite[] twoToneRight;
    public Color[] preparedColors;

    private const int slotMax = 64;
    private const float backColorAlpha = 0.5f;
    private MessageSlot[] messageSlot = new MessageSlot[slotMax];
    private float[] remainTime = new float[slotMax];

    public const int panelType_Information = 0;
    public const int panelType_Speech = 1;
    public const int panelType_Log = 2;
    public const int panelMax = 3;

    public const int slotType_Normal = 0;
    public const int slotType_Speech = 1;
    public const int slotType_Caution = 2;
    public const int slotType_Log = 3;
    public const int slotType_Friends = 4;
    public const int slotType_HP = 5;
    public const int slotType_ST = 6;
    public const int slotType_Inventory = 7;
    public const int slotType_Mission = 8;
    public const int slotType_Important = 9;
    public const int slotType_Lucky = 10;
    public const int slotType_LockonEnabled = 11;
    public const int slotType_LockonDisabled = 12;
    public const int slotType_CommandAttack = 13;
    public const int slotType_CommandEvade = 14;
    public const int slotType_CommandIgnore = 15;
    public const int slotType_CommandFree = 16;
    public const int slotType_Trophy = 17;
    public const int slotType_Disadvantage = 18;
    public const int slotType_Auto = 19;

    public const float time_Default = 4f;
    public const float time_Important = 7f;
    public const float time_Infinity = 100000f;

    static readonly float[] languageWeighting = new float[GameManager.languageMax] { 0.12f, 0.054f, 0.18f, 0.108f }; 
    static readonly float[,] fontSize = new float[panelMax, 2] { { 20f, 28f }, { 20f, 26f }, { 16f, 20f } };
    static readonly float[] faceNameFontSize = new float[2] { 16f, 20f };
    const float logHeight = 480f;
    const float logMargin = 14f;

    private void Update() {
        float deltaTimeCache = Time.deltaTime * GetMessageTimeSpeed();
        for (int i = 0; i < slotMax; i++) {
            if (messageSlot[i] && remainTime[i] < 10000) {
                remainTime[i] -= deltaTimeCache;
                if (remainTime[i] < 0) {
                    Destroy(messageSlot[i].gameObject);
                    messageSlot[i] = null;
                } else if (messageSlot[i].frameCount < 2) {
                    messageSlot[i].frameCount++;
                    if (messageSlot[i].frameCount == 2) {
                        if (messageSlot[i].content) {
                            messageSlot[i].content.enableAutoSizing = false;
                        }
                        if (messageSlot[i].faceName) {
                            messageSlot[i].faceName.enableAutoSizing = false;
                        }
                    }
                }
            }
        }
    }

    public float GetMessageTimeSpeed() {
        if (GameManager.Instance) {
            return 1f / Mathf.Clamp(1f + GameManager.Instance.save.config[GameManager.Save.configID_MessageTimeRate] * 0.1f, 0.1f, 3f);
        } else {
            return 1f;
        }
    }

    public int SetMessage(string text, float time = time_Default, int panelType = 0, int slotType = -1, bool isFirstIn = false) {
        int answer = -1;
        if (slotType < 0) {
            slotType = panelType;
        }
        if (text != null) {
            for (int i = 0; i < slotMax; i++) {
                if (!messageSlot[i]) {
                    messageSlot[i] = Instantiate(slotPrefab[slotType], panel[panelType]).GetComponent<MessageSlot>();
                    if (isFirstIn) {
                        messageSlot[i].transform.SetSiblingIndex(0);
                    }
                    remainTime[i] = time;
                    messageSlot[i].content.text = text;
                    answer = i;
                    break;
                }
            }
        }
        return answer;
    }

    public float GetSpeechAppropriateTime(int stringLen) {
        int lang = GameManager.Instance.save.language;
        return 5f + Mathf.Max(0f, stringLen * languageWeighting[lang >= 0 && lang < GameManager.languageMax ? lang : GameManager.languageEnglish] - 3f);
    }

    public int SetMessageOptional(string text, Color backColor1, Color backColor2, int twoToneType, int faceId, float time = time_Default, int panelType = 0, int slotType = -1, bool isFirstIn = false) {
        if (time < 0f) {
            time = GetSpeechAppropriateTime(text.Length);
        }
        int answer = SetMessage(text, time, panelType, slotType, isFirstIn);
        if (answer >= 0) {
            MessageSlot mesSlot = messageSlot[answer];
            if (mesSlot.backSimple || mesSlot.backLeft || mesSlot.backRight) {
                Color colorTemp1 = backColor1;
                colorTemp1.a = backColorAlpha;
                if (twoToneType >= 0 && twoToneType < twoToneLeft.Length && twoToneType < twoToneRight.Length) {
                    Color colorTemp2 = backColor2;
                    colorTemp2.a = backColorAlpha;
                    mesSlot.backLeft.sprite = twoToneLeft[twoToneType];
                    mesSlot.backLeft.color = colorTemp1;
                    mesSlot.backRight.sprite = twoToneRight[twoToneType];
                    mesSlot.backRight.color = colorTemp2;
                    mesSlot.backSimple.enabled = false;
                    mesSlot.backLeft.enabled = true;
                    mesSlot.backRight.enabled = true;
                } else {
                    mesSlot.backSimple.color = colorTemp1;
                    mesSlot.backSimple.enabled = true;
                    mesSlot.backLeft.enabled = false;
                    mesSlot.backRight.enabled = false;
                }
            }
            if (mesSlot.faceImage) {
                mesSlot.faceImage.sprite = ItemDatabase.Instance.GetItemData(faceId).image;
            }
            if (mesSlot.faceName) {
                mesSlot.faceName.text = ItemDatabase.Instance.GetItemName(faceId);
                // mesSlot.faceName.FitFontSize(faceNameFontSize[0], faceNameFontSize[1]);
            }
        }
        return answer;
    }

    public int SetMessageLog(string text, int faceId) {
        string textPlus = ItemDatabase.Instance.GetItemName(faceId) + " : " + text;
        int answer = SetMessage(textPlus, time_Infinity, panelType_Log, slotType_Log, true);
        if (answer >= 0) {
            float height = messageSlot[answer].content.preferredHeight + logMargin;
            Vector2 rectPosition;
            for (int i = 0; i < messageSlot.Length; i++) {
                if (i != answer && messageSlot[i] && messageSlot[i].slotType == slotType_Log) {
                    rectPosition = messageSlot[i].rectTransform.anchoredPosition;
                    rectPosition.y += height;
                    if (rectPosition.y > logHeight) {
                        Destroy(messageSlot[i].gameObject);
                    } else {
                        messageSlot[i].rectTransform.anchoredPosition = rectPosition;
                    }
                }
            }
        }
        return answer;
    }

    public int GetMessageCount(int panelType = 0) {
        return panel[panelType].childCount;
    }

    public void DestroyMessage() {
        for (int i = 0; i < slotMax; i++) {
            if (messageSlot[i] != null) {
                Destroy(messageSlot[i].gameObject);
                messageSlot[i] = null;
            }
        }
    }

    public void DestroyMessageSlotType(int slotType) {
        for (int i = 0; i < slotMax; i++) {
            if (messageSlot[i] != null && messageSlot[i].slotType == slotType) {
                Destroy(messageSlot[i].gameObject);
                messageSlot[i] = null;
            }
        }
    }

    public void DestroySpecificMessage(int index) {
        if (index >= 0 && index < slotMax && messageSlot[index] != null) {
            Destroy(messageSlot[index].gameObject);
            messageSlot[index] = null;
        }
    }

}
