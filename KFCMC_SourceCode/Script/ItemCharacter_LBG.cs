using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCharacter_LBG : ItemCharacter {
    
    public GameObject explodePrefab;
    public Vector3 explodeOffset;
    protected float explodeTimer;

    protected override void SetHomeTalkIndex() {
        if (homeTalkIndex == homeTalkMax - 1) {
            explodeTimer = Mathf.Max(lookTimeRemain, 5f);
            ignoreTimeRemain = 10f;
        }
        base.SetHomeTalkIndex();
    }

    protected override void HomeUpdate() {
        base.HomeUpdate();
        if (explodeTimer > 0f) {
            explodeTimer -= deltaTimeCache;
            if (explodeTimer <= 0f) {
                Instantiate(explodePrefab, transform.position + explodeOffset, Quaternion.identity);
            }
        }
    }

}
