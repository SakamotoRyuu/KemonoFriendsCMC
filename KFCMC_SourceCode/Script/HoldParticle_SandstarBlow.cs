using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldParticle_SandstarBlow : HoldParticle
{

    public override void SetParam(float multiplier) {
        var main = parentParticle.main;
        float lifeMultiplier = 0.8875f + multiplier * (5f / 6f);
        main.duration = 0.4125f + multiplier * 2.5f;
        main.startLifetimeMultiplier = lifeMultiplier;
        main.startSpeedMultiplier = 0.49375f + multiplier * 3.75f;
    }

}
