using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoInstantiate_SetCullingPivot : AutoInstantiate {

    protected override void ExecuteInstantiate() {
        base.ExecuteInstantiate();
        ItemCharacter ifr = instance.GetComponent<ItemCharacter>();
        if (ifr) {
            ifr.SetCullingPivotTrans(transform);
        }
    }

}
