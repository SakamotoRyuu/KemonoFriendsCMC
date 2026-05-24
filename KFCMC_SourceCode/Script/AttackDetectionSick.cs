using Pixeye.Unity;
using UnityEngine;

public class AttackDetectionSick : AttackDetection
{
    [System.Serializable]
    public class SickPropertyClass
    {
        public CharacterBase.SickType sickType;
        public float sickTime = -1;
        public float probability = 0;
    }


    [Foldout("Sick")] public CharacterBase.SickType sickType;
    [Foldout("Sick")] public float sickTime = -1;
    [Foldout("Sick")] public int probability = 0;
    [Foldout("Sick")] public SickPropertyClass[] additionalSicks;
    [Foldout("Sick")] public bool isBlownAway;
    [Foldout("Sick")] public GameObject blownAwayEffectPrefab;

    protected override bool SendDamage(ref DamageDetection damageDetection, ref Vector3 closestPoint, ref Vector3 direction)
    {
        if (base.SendDamage(ref damageDetection, ref closestPoint, ref direction))
        {
            if (probability >= 100 ||
                (probability > 0 && Random.Range(0, 100) < probability))
            {
                damageDetection.ReceiveSick(sickType, sickTime, this);
            }
            if (additionalSicks.Length > 0)
            {
                foreach(var additionalSick in additionalSicks)
                {
                    if (additionalSick.probability >= 100 ||
                        (additionalSick.probability > 0 && Random.Range(0, 100) < additionalSick.probability))
                    {
                        damageDetection.ReceiveSick(additionalSick.sickType, additionalSick.sickTime, this);
                    }
                }
            }
            if (isBlownAway)
            {
                CharacterBase targetCharacter = damageDetection.GetCharacterBase();
                if (targetCharacter)
                {
                    BlownAway(targetCharacter, closestPoint);
                }
            }
            return true;
        }
        return false;
    }

    void BlownAway(CharacterBase targetCharacter, Vector3 closestPoint)
    {
        if (CharacterManager.Instance && !GameManager.Instance.megatonCoin)
        {
            targetCharacter.BlownAway();
            if (blownAwayEffectPrefab)
            {
                GameObject effect1 = Instantiate(blownAwayEffectPrefab, targetCharacter.transform);
                GameObject effect2 = Instantiate(blownAwayEffectPrefab, closestPoint, Quaternion.identity);
                AudioSource aSrc;
                if (targetCharacter.isPlayer)
                {
                    aSrc = effect2.GetComponent<AudioSource>();
                }
                else
                {
                    aSrc = effect1.GetComponent<AudioSource>();
                }
                if (aSrc)
                {
                    aSrc.enabled = false;
                }
            }
        }
    }

}
