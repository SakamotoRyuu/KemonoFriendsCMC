using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetectionEnemyChild : DamageDetection
{
    public TriggerReceiverDamage triggerReceiverDamage;

    protected override void Awake()
    {
        isNotCharacter = true;
    }

    protected override void Start()
    {
        if (CharacterManager.Instance)
        {
            characterId = CharacterManager.Instance.IssueID();
        }
    }

    public override bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1)
    {
        if (attackDetection != null && triggerReceiverDamage)
        {
            int colorTemp = overrideColorType >= 0 ? overrideColorType : colorType;
            PlayHitEffect(effectPosition, colorTemp, overrideColorType >= 0, attackDetection);
            WorkDamage(damage, knockAmount * knockedRate, knockVector);
            triggerReceiverDamage.ReceiveDamage(effectPosition, damage, knockAmount * knockedRate, knockVector, attackDetection, penetrate, overrideColorType);
        }
        return true;
    }

    public virtual void WorkDamage(int damage, float knockAmount, Vector3 knockVector)
    {
    }
}
