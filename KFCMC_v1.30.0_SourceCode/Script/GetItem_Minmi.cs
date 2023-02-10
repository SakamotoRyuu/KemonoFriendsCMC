using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem_Minmi : GetItem {

    public int shiftNum;

    public override void GetProcess() {
        base.GetProcess();
        GameManager.Instance.save.minmi |= (1 << shiftNum);
        TrophyManager.Instance.CheckTrophy(TrophyManager.t_MinmiStatue);
    }

    public override void TouchProcess() {
        base.TouchProcess();
        GameManager.Instance.save.minmi |= (1 << shiftNum);
        TrophyManager.Instance.CheckTrophy(TrophyManager.t_MinmiStatue);
    }

}
