using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterDetection : MonoBehaviour {

    public string targetTag = "EnemyAttackDetection";
    public bool ignorePlayer = false;

    protected CharacterBase parentCBase;
    protected GameObject otherObj;

    protected virtual void Awake() {
        parentCBase = GetComponentInParent<CharacterBase>();
    }

    protected virtual void Action(bool isProjectile) {
    }

    protected virtual void Check(Collider other) {
        if (parentCBase != null && other.CompareTag(targetTag)) {
            otherObj = other.gameObject;
            bool condition = true;
            bool isProjectile = false;
            AttackDetection attackDetection = otherObj.GetComponent<AttackDetection>();
            if (attackDetection) {
                isProjectile = attackDetection.isProjectile;
                if (ignorePlayer && CharacterManager.Instance.GetCharacterIsPlayer(attackDetection.GetParentCharacterBase())) {
                    condition = false;
                }
            }
            if (condition) {
                Action(isProjectile);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        Check(other);
    }
    
}
