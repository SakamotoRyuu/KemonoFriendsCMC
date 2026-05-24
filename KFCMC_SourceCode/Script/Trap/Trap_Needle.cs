using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Needle : TrapBase {

    public AutoMove needleMove;
    public AudioSource aSrc;
    public GameObject destroyEffect;

    protected override void WorkEnter(int index) {
        if (CharacterManager.Instance && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Needle) != 0 && destroyEffect) {
            Instantiate(destroyEffect, transform.position, transform.rotation);
            CharacterManager.Instance.CheckTrophy_CampoFlicker();
            Destroy(gameObject);
        } else {
            cBaseList[index].DamageNeedle();
            needleMove.Action();
            aSrc.Play();
        }
    }
    
}
