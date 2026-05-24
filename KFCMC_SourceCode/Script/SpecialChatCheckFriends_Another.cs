using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialChatCheckFriends_Another : SpecialChatCheckFriends {

    public int newProgress;

    protected override void SetChatBody() {
        if (GameManager.Instance.IsPlayerAnother && (newProgress <= 0 || GameManager.Instance.save.progress < newProgress)) {
            if (newProgress > 0) {
                GameManager.Instance.save.progress = newProgress;
            }
            base.SetChatBody();
        }
    }

}
