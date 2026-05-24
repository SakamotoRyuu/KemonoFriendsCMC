using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAttackDetection : MonoBehaviour {

    public Rigidbody carRigid;
    public Transform centerOfMass;
    public int attackPower;
    public float knockAmount;
    public bool penetrate = true;
    public int colorType = -1;
    public string targetTag = "EnemyDamageDetection";        

    protected virtual void WorkEnter(DamageDetection targetDD, Vector3 closestPoint, Vector3 direction) {
        targetDD.ReceiveDamage(ref closestPoint, Mathf.RoundToInt(Mathf.Max(1f, CharacterBase.CalcDamage(attackPower * targetDD.damageRate, targetDD.GetDefence()))), knockAmount, ref direction, CharacterManager.Instance.pCon.attackDetection[0], penetrate, colorType);
    }

    private void OnTriggerEnter(Collider other) {
        if (carRigid && attackPower > 0 && other.CompareTag(targetTag)) {
            DamageDetection targetDD = other.GetComponent<DamageDetection>();
            if (targetDD) {
                Vector3 closestPoint = other.ClosestPoint(centerOfMass.position);
                Vector3 direction = closestPoint - centerOfMass.position;
                Vector3 carVelocity = carRigid.velocity;
                if ((closestPoint - centerOfMass.position).sqrMagnitude < 0.0001f || Vector3.Angle(direction, carVelocity) < 45f) {
                    MyMath.NormalizeXZ(ref direction);
                    WorkEnter(targetDD, closestPoint, direction);
                }
            }
        }
    }
}
