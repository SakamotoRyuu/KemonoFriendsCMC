using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandstarBlow : MonoBehaviour {

    public GameObject blowObj;
    public ParticleSystem particle;
    
    public void SetParam(float scale) {
        var main = particle.main;
        float lifeMultiplier = 0.8875f + scale * (5f / 6f);
        main.duration = 0.4125f + scale * 2.5f;
        main.startLifetimeMultiplier = lifeMultiplier;
        main.startSpeedMultiplier = 0.49375f + scale * 3.75f;
        blowObj.SetActive(true);
        Destroy(gameObject, lifeMultiplier * 12f + 1f);
    }

}
