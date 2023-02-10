using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NCAD_CelliumCure : NonCharacterAttackDetectionParticle {

    protected override void WorkEnter(DamageDetection targetDD, Vector3 closestPoint, Vector3 direction) {
        CharacterBase targetCBase = targetDD.GetCharacterBase();
        if (targetCBase != null) {
            Enemy_BigDog bigDogTemp = targetCBase.GetComponent<Enemy_BigDog>();
            if (bigDogTemp != null) {
                damage = 50000;
                knockAmount = 50000;
            } else {
                Enemy_BigDogInside insideTemp = targetCBase.GetComponent<Enemy_BigDogInside>();
                if (insideTemp != null) {
                    damage = insideTemp.GetMaxHP();
                    knockAmount = 50000;
                    if (TrophyManager.Instance) {
                        TrophyManager.Instance.CheckTrophy(TrophyManager.t_BigDogInside, true);
                    }
                }
            }
        }
        base.WorkEnter(targetDD, closestPoint, direction);
    }

}
