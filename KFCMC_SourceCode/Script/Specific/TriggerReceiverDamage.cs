using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerReceiverDamage : TriggerReceiver
{
    public virtual void ReceiveDamage(Vector3 effectPosition, int damage, float knockAmount, Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1)
    { 
    }
}
