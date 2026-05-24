using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterDetection_Dodge : CounterDetection {

    public int dodgeDir = 0;
    public bool changeState = true;

    protected override void Action(bool isProjectile) {
        base.Action(isProjectile);
        if (parentCBase && parentCBase.enabled && parentCBase.GetCanTakeDamage() && parentCBase.DodgeChallenge()) {
            parentCBase.CounterDodge(dodgeDir, changeState);
        }
    }

}
