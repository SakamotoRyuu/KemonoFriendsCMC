using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControlForAttackDetection : MonoBehaviour {

    public ParticleSystem[] particle;
    public bool supermanOnly;

    AttackDetection attackDetection;
    CharacterBase cBase;
    
	void Awake () {
		if (attackDetection == null) {
            attackDetection = GetComponent<AttackDetection>();
        }
	}

    private void Start() {
        if (attackDetection) {
            cBase = attackDetection.GetParentCharacterBase();
        }
    }
    
    void Update () {
        bool flag = (cBase && attackDetection && attackDetection.attackEnabled && (!supermanOnly || cBase.isSuperman));
        for (int i = 0; i < particle.Length; i++) {
            if (particle[i]) {
                if (!particle[i].isPlaying && flag) {
                    particle[i].Play();
                } else if (particle[i].isPlaying && !flag) {
                    particle[i].Stop();
                }
            }
        }
	}
}
