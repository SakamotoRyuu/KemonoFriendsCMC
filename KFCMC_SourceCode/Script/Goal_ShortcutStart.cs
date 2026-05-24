using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_ShortcutStart : Goal {

    protected override void Start() {
        base.Start();
        int index = StageManager.Instance.GetShortcutFloorIndex();
        if (index >= 0) {
            floorNum = StageManager.Instance.dungeonMother.shortcutSettings[index].shortcutFloor;
        } else {
            Destroy(gameObject);
        }
    }

}
