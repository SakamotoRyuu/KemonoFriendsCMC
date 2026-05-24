using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Enter : TrapBase {

    public GameObject effectPrefab;
    public CharacterBase.SickType sickType;
    public float sickDuration = 10f;
    public bool destroyForAntidote;
    public GameObject destroyEffect;

    protected override void WorkEnter(int index) {
        if (destroyForAntidote && CharacterManager.Instance && CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Antidote) && destroyEffect) {
            Instantiate(destroyEffect, transform.position, transform.rotation);
            if (destroyParent && transform.parent != null) {
                Destroy(transform.parent.gameObject);
            } else {
                Destroy(gameObject);
            }
        } else {
            if (effectPrefab) {
                Instantiate(effectPrefab, cBaseList[index].transform);
            }
            cBaseList[index].SetSick(sickType, sickDuration);
        }
    }

}