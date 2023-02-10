using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCharacter_AnotherServal : ItemCharacter {

    public RuntimeAnimatorController homeAnimCon;

    protected override void Awake() {
        base.Awake();
        if (anim && isHome && homeAnimCon) {
            anim.runtimeAnimatorController = homeAnimCon;
        }
    }

}
