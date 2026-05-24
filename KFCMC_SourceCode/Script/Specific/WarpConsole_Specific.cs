using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpConsole_Specific : WarpConsole {

    public Transform warpPoint;

    protected override void Warp() {
        if (warpPoint) {
            CharacterManager.Instance.SuperWarp(warpPoint.position, resetCamera, effectEnabled);
        }
    }

}
