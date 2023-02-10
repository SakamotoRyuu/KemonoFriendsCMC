using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetectionAdditiveCollider : AttackDetection {

    public Collider[] additiveCollider;

    protected override void AwakeProcess() {
        base.AwakeProcess();
        for (int i = 0; i < additiveCollider.Length; i++) {
            if (additiveCollider[i]) {
                additiveCollider[i].enabled = false;
            }
        }
    }

    public override void DetectionStart(bool effectEnabled = true, bool weaponTrailsEnabled = true) {
        base.DetectionStart(effectEnabled, weaponTrailsEnabled);
        for (int i = 0; i < additiveCollider.Length; i++) {
            if (additiveCollider[i]) {
                additiveCollider[i].enabled = true;
            }
        }
    }

    public override void DetectionEnd(bool effectEnabled = true, bool weaponTrailsEnabled = true) {
        base.DetectionEnd(effectEnabled, weaponTrailsEnabled);
        for (int i = 0; i < additiveCollider.Length; i++) {
            if (additiveCollider[i]) {
                additiveCollider[i].enabled = false;
            }
        }
    }

}
