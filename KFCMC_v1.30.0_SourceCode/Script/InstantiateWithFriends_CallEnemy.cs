using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateWithFriends_CallEnemy : InstantiateWithFriends {

    public int enemyID = 0;
    public int level = 0;

    protected override void InstantiatePrefab() {
        if (CharacterDatabase.Instance.CheckFacilityEnabled(facilityID)) {
            EnemyBase eBase = Instantiate(CharacterDatabase.Instance.GetEnemy(enemyID), transform).GetComponent<EnemyBase>();
            if (eBase) {
                eBase.enemyID = enemyID;
                eBase.SetLevel(level, false, false);
                if (PauseController.Instance) {
                    PauseController.Instance.SetFacilityObj(facilityID, eBase.gameObject);
                }
            }
        }
    }

}
