using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Warp_SuperWind : Trap_Warp {

    public int maxNum;
    int nowNum;

    protected override void WorkEnter(int index) {
        if (nowNum < maxNum) {
            nowNum++;
            base.WorkEnter(index);
        }
    }

}
