using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterDetection_Attack : CounterDetection {

    protected override void Action(bool isProjectile) {
        base.Action(isProjectile);
        parentCBase.CounterAttack(otherObj, isProjectile);
    }

}
