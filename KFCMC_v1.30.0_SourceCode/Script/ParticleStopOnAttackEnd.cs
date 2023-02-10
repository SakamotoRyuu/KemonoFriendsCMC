using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStopOnAttackEnd : MonoBehaviour {

    public ParticleSystem particle;
    CharacterBase cBase;
    
	void Awake () {
        cBase = GetComponentInParent<CharacterBase>();
	}

    private void Start() {
        if (!cBase) {
            cBase = GetComponentInParent<CharacterBase>();
        }
    }

    void Update () {
		if (cBase && particle && particle.isPlaying) {
            if (!cBase.IsAttacking()) {
                particle.Stop();
            }
        }
	}

    public void StopExternal() {
        if (particle && particle.isPlaying) {
            particle.Stop();
        }
    }

}
