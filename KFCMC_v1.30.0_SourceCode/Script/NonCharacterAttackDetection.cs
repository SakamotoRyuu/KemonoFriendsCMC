using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCharacterAttackDetection : MonoBehaviour {

    public int damage;
    public float knockAmount;
    public bool penetrate = true;
    public int colorType = -1;
    public string targetTag = "EnemyDamageDetection";

    protected List<int> idList = new List<int>(32);

    protected void OnEnable() {
        idList.Clear();
    }

    protected virtual void WorkEnter(DamageDetection targetDD, Vector3 closestPoint, Vector3 direction) {
        targetDD.ReceiveDamage(ref closestPoint, damage, knockAmount, ref direction, null, penetrate, colorType);
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(targetTag)) {
            DamageDetection targetDD = other.GetComponent<DamageDetection>();
            if (targetDD) {
                int targetID = targetDD.characterId;
                if (!idList.Contains(targetID)) {
                    idList.Add(targetID);
                    Vector3 closestPoint = other.ClosestPoint(transform.position);
                    Vector3 direction = targetDD.transform.position - transform.position;
                    MyMath.NormalizeXZ(ref direction);
                    WorkEnter(targetDD, closestPoint, direction);
                }
            }
        }
    }

}
