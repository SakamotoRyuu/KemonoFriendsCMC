using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Common : TrapBase {

    public CharacterBase.SickType sickType;
    public float sickDuration = 10f;
    public bool cancelOnExit;

    protected void Update() {
        int count = cBaseList.Count;
        if (count > 0) {
            for (int i = 0; i < count; i++) {
                cBaseList[i].SetSick(sickType, sickDuration);
                cBaseList[i].EscapeFromTrap(transform.position, sickType, radius);
            }
        }
    }

    protected override void WorkExit(int index) {
        if (cancelOnExit) {
            cBaseList[index].SetSick(sickType, 0);
        }
    }
}
