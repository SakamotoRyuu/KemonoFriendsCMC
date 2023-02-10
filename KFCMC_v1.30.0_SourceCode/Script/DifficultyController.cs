using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;

public class DifficultyController : SingletonMonoBehaviour<DifficultyController> {

    [System.Serializable]
    public class Choices {
        public Canvas canvas;
        public GridLayoutGroup gridLayoutGroup;
        public GraphicRaycaster gRaycaster;
        public RectTransform cursorRect;
        public int cursorPos;
        public Vector2 origin;
        public Vector2 interval;
    }

    public Choices choices;
    public GameObject[] slots;
    public TMP_Text title;
    public TMP_Text current;
    public TMP_Text[] nameText;
    public TMP_Text[] infoText;
    public int choicesMax = 4;

    StringBuilder sb = new StringBuilder();
    int eventEnterNum = -1;
    int eventClickReserved = -1;

    public void SetTexts(int defaultDifficulty = 0) {
        title.text = TextManager.Get("CONFIG_DIF_TITLE");
        if (current) {
            current.text = sb.Clear().Append(TextManager.Get("CONFIG_CURRENT")).Append(TextManager.Get(StringUtils.Format("CONFIG_DIF_NAME_{0}", GameManager.Instance.save.difficulty))).ToString();
        }
        for (int i = 0; i < nameText.Length; i++) {
            nameText[i].text = sb.Clear().Append(TextManager.Get(StringUtils.Format("CONFIG_DIF_NAME_{0}", i + 1))).ToString();
            infoText[i].text = sb.Clear().Append(TextManager.Get(StringUtils.Format("CONFIG_DIF_INFO_{0}", i + 1))).ToString();
        }
        for (int i = 0; i < slots.Length; i++) {
            slots[i].SetActive(i < choicesMax);
        }
        choices.cursorPos = (defaultDifficulty >= 1 ? defaultDifficulty - 1 : Mathf.Clamp(GameManager.Instance.save.difficulty - 1, 0, choicesMax - 1));
        choices.cursorRect.anchoredPosition = choices.origin + choices.interval * choices.cursorPos;
    }

    public void Activate(bool flag) {
        choices.canvas.enabled = flag;
        if (choices.gRaycaster && (flag == false || GameManager.Instance.MouseEnabled)) {
            choices.gRaycaster.enabled = flag;
        }
        choices.gridLayoutGroup.enabled = flag;
    }

    public int DifficultyControl(Vector2Int move) {
        int answer = -1;
        if (GameManager.Instance.MouseCancelling) {
            eventEnterNum = -1;
            eventClickReserved = -1;
        }
        if (move.y != 0 || eventEnterNum >= 0) {
            UISE.Instance.Play(UISE.SoundName.move);
            choices.cursorPos = (eventEnterNum >= 0 ? eventEnterNum : (choices.cursorPos + move.y + choicesMax) % choicesMax);
            choices.cursorRect.anchoredPosition = choices.origin + choices.interval * choices.cursorPos;
        } 
        if (GameManager.Instance.playerInput.GetButtonDown(RewiredConsts.Action.Submit) || eventClickReserved >= 0) {
            if (eventClickReserved >= 0) {
                choices.cursorPos = eventClickReserved;
                choices.cursorRect.anchoredPosition = choices.origin + choices.interval * choices.cursorPos;
            }
            answer = choices.cursorPos + 1;
        } else if (GameManager.Instance.GetCancelButtonDown) {
            answer = -2;
        }
        eventEnterNum = -1;
        eventClickReserved = -1;
        return answer;
    }

    public void EventEnter(int param) {
        eventEnterNum = param;
    }

    public void EventClick(int param) {
        if (!Input.GetMouseButtonUp(1)) {
            eventClickReserved = param;
        }
    }

}
