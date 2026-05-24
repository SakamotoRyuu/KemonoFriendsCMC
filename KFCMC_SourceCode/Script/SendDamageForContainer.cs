using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendDamageForContainer : MonoBehaviour
{

    const string TargetTag = "EnemyDamageDetection";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TargetTag))
        {
            DamageDetectionContainer container = other.GetComponent<DamageDetectionContainer>();
            if (container)
            {
                Vector3 effectPosition = transform.position;
                Vector3 knockVector = container.transform.position - effectPosition;
                container.PlayHitEffect(effectPosition);
                container.WorkDamage(0, 0, knockVector);
            }
        }
    }
}
