using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCharacterLBSP : ItemCharacter {

    protected const int kabanID = 1;
    protected const string kabanOption = "_KABAN";

    bool IsLBComplete() {
        if (ShopDatabase.Instance) {
            return (ShopDatabase.Instance.GetShopLevel() >= 4);
        } else {
            return false;
        }
    }

    protected override string GetTalkMargedName_Home() {
        if (isHomeAp && CharacterManager.Instance && CharacterManager.Instance.GetFriendsExist(kabanID, true)) {
            return base.GetTalkMargedName_Home() + kabanOption;
        } else {
            return base.GetTalkMargedName_Home();
        }
    }

    protected override bool CheckCondition() {
        bool answer = true;
        if (GameManager.Instance.save.luckyBeast[bonusIndex] == 0) {
            answer = false;
        }
        return answer;
    }

    protected override void SetHomeTalkIndex() {
        if (isHomeAp) {
            base.SetHomeTalkIndex();
        } else {
            homeTalkIndex = IsLBComplete() ? 1 : 0;
        }
    }

    protected override void HomeAction() {
        SetHomeTalkIndex();
        base.HomeAction();
    }

}
