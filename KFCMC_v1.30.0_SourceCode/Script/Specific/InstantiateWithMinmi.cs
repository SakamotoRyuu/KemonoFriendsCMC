using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWithMinmi : InstantiateWithFriends {

    public int shiftNum;

    protected override bool CheckCondition() {
        return (GameManager.Instance.save.minmi & (1 << shiftNum)) != 0;
    }

}
