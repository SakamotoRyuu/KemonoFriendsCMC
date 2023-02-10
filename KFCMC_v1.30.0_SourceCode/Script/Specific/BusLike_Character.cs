using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Rewired;

public class BusLike_Character : MonoBehaviour {

    public string talkName;
    public int faceIndex;
    public int homeTalkMax = 3;
    public int conditionFriendsID;
    public int specialTalkIndex = 0;

    Player playerInput;
    MessageBackColor mesBackColor;
    StringBuilder sb = new StringBuilder();
    int homeTalkIndex = 0;
    bool onStay;
    float ignoreTimeRemain;
    const string talkHeader = "TALK_";
    const string homeHeader = "_HOME";
    const string specialHeader = "_SPECIAL";
    const string targetTag = "ItemGetter";

    private void Awake() {
        mesBackColor = GetComponent<MessageBackColor>();
    }

    void Start() {
        if (GameManager.Instance) {
            playerInput = GameManager.Instance.playerInput;
        }
    }

    void Update() {
        if (ignoreTimeRemain > 0f) {
            ignoreTimeRemain -= Time.deltaTime;
        }
        if (onStay && ignoreTimeRemain <= 0f && playerInput.GetButtonDown(RewiredConsts.Action.Submit)) {
            Action();
        }
    }

    protected virtual void Action() {
        if (CharacterManager.Instance && ignoreTimeRemain <= 0f) {
            ignoreTimeRemain = 1.5f;
            sb.Clear().Append(talkHeader).Append(talkName).Append(homeHeader).AppendFormat("_{0:00}", homeTalkIndex);
            if (homeTalkIndex == specialTalkIndex && CharacterManager.Instance.GetFriendsExist(conditionFriendsID, false)) {
                sb.Append(specialHeader);
            }
            string margedName = sb.ToString();
            MessageUI.Instance.SetMessageOptional(TextManager.Get(margedName), mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex + 100, -1, 1, 1, true);
            MessageUI.Instance.SetMessageLog(TextManager.Get(margedName), faceIndex + 100);
            Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.talk], transform.position, Quaternion.identity);
            homeTalkIndex = (homeTalkIndex + 1) % homeTalkMax;
        }
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            onStay = true;
            Action();
        }
    }

    protected virtual void OnTriggerExit(Collider other) {
        if (other.CompareTag(targetTag)) {
            onStay = false;
        }
    }
}
