using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialChatCheckFriends : MonoBehaviour {

    public int friendsID;
    public string dicKey;
    public float delay;
    // public float chatTimer = 5f;
    public bool setFaceEnabled;
    public FriendsBase.FaceName faceName;
    // public float faceTimer = 5f;
    public string otherFaceName;
    public bool checkMessageZero;
    public bool checkTrophy;

    bool activated;
    bool reserved;
    float timer;

    protected virtual void SetChatBody() {
        if (MessageUI.Instance && CharacterManager.Instance) {
            float chatTimer = MessageUI.Instance.GetSpeechAppropriateTime(TextManager.Get(dicKey).Length);
            CharacterManager.Instance.SetSpecialChat(dicKey, friendsID, chatTimer);
            if (setFaceEnabled) {
                if (faceName == FriendsBase.FaceName.Other && !string.IsNullOrEmpty(otherFaceName)) {
                    CharacterManager.Instance.SetFaceSpecificFriendString(friendsID, otherFaceName, chatTimer);
                } else {
                    CharacterManager.Instance.SetFaceSpecificFriend(friendsID, faceName, chatTimer);
                }
            }
            if (checkTrophy && TrophyManager.Instance) {
                TrophyManager.Instance.CheckTrophy_SpecialChat(friendsID);
            }
        }
    }

    protected virtual void Update() {
        if (!activated && (friendsID < 0 || CharacterManager.Instance.GetFriendsExist(friendsID)) && !string.IsNullOrEmpty(dicKey)) {
            reserved = true;
        }
        if (reserved && Time.timeScale > 0f && !activated && (!checkMessageZero || MessageUI.Instance.GetMessageCount(1) == 0)) {
            timer += Time.deltaTime;
            if (timer >= delay) {
                SetChatBody();
                activated = true;
                enabled = false;
            }
        }
    }
}
