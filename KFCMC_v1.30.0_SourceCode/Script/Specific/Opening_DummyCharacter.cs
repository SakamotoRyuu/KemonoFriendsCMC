using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class Opening_DummyCharacter : MonoBehaviour {

    public XWeaponTrail[] weaponTrail0;
    public XWeaponTrail[] weaponTrail1;
    
    private void Start() {
        for (int i = 0; i < weaponTrail0.Length; i++) {
            weaponTrail0[i].Init();
            weaponTrail0[i].Deactivate();
        }
        for (int i = 0; i < weaponTrail1.Length; i++) {
            weaponTrail1[i].Init();
            weaponTrail1[i].Deactivate();
        }
    }

    void AttackStart(int index) {
        if (index == 0) {
            for (int i = 0; i < weaponTrail0.Length; i++) {
                weaponTrail0[i].Activate();
            }
        } else if (index == 1) {
            for (int i = 0; i < weaponTrail1.Length; i++) {
                weaponTrail1[i].Activate();
            }
        }
    }
    void AttackEnd(int index) {
        if (index == 0) {
            for (int i = 0; i < weaponTrail0.Length; i++) {
                weaponTrail0[i].StopSmoothly(0.1f);
            }
        } else if (index == 1) {
            for (int i = 0; i < weaponTrail1.Length; i++) {
                weaponTrail1[i].StopSmoothly(0.1f);
            }
        }
    }
    void LockonEnd() { }
    void EmitEffectString(string type) { }
    void AnimationStopTiming(int index) { }
    void SuperarmorStart() { }
    void SuperarmorEnd() { }
    void ThrowReady() { }
    void ThrowStart() { }
    void ThrowSpin() {
        AttackStart(0);
        AttackStart(1);
    }
    void ThrowSlash() { }
    void ScrewStart() { }
    void ScrewEnd() { }
    void S_ParticleStopAll() {
        AttackEnd(0);
        AttackEnd(1);
    }

}
