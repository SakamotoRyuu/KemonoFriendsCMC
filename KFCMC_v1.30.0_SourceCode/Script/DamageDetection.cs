using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection : MonoBehaviour {
    
    public GameObject[] hitEffect;
    public int hitEffectNum = 0;
    public float damageRate = 1.0f;
    public float defenceRate = 1.0f;
    public float knockedRate = 1.0f;
    public int characterId = -1;
    public int colorType = 0;
    public bool checkDodge = false;
    public Collider specialCollider;

    [System.NonSerialized]
    public bool isNotCharacter;

    protected CharacterBase parentCBase;
    protected double timeStamp;
    protected double timeDifference;
    protected int sounded;
    protected Vector3 savePosition;
    protected Transform trans;
    protected GameObject effectInstance;
    protected static readonly Vector3 vecForward = Vector3.forward;
    protected static readonly Quaternion quaIden = Quaternion.identity;
    
    protected virtual void Awake() {
        trans = transform;
        if (parentCBase == null) {
            parentCBase = GetComponentInParent<CharacterBase>();
        }
    }

    protected virtual void Start() {
        if (parentCBase) {
            characterId = parentCBase.characterId;
        }
    }

    public void SetParentCharacterBase(CharacterBase cBase) {
        parentCBase = cBase;
    }

    void DodgeBody(Vector3 closestPoint) {
        int dodgeDir = Random.Range(-1, 2);
        if (trans.position != closestPoint) {
            closestPoint.y = trans.position.y;
            float axis = Vector3.Cross(trans.TransformDirection(vecForward), closestPoint - trans.position).y;
            if (axis > 0.001f) {
                dodgeDir = -1;
            } else if (axis < -0.001f) {
                dodgeDir = 1;
            }
        }
        parentCBase.CounterDodge(dodgeDir);
    }

    public virtual bool ReceiveDamage(ref Vector3 effectPosition, int damage, float knockAmount, ref Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1) {
        if (parentCBase && parentCBase.enabled && parentCBase.GetCanTakeDamage(penetrate)) {
            if (checkDodge && !penetrate && parentCBase.DodgeChallenge()) {
                DodgeBody(effectPosition);
            } else {
                int colorTemp = overrideColorType >= 0 ? overrideColorType : colorType;
                if (colorType == CharacterBase.damageColor_Hard && overrideColorType == CharacterBase.damageColor_Back) {
                    colorTemp = CharacterBase.damageColor_HardBack;
                }
                if (hitEffectNum < hitEffect.Length && hitEffect[hitEffectNum]) {
                    timeDifference = 0;
                    effectInstance = null;
                    if (timeStamp != GameManager.Instance.time) {
                        sounded = 0;
                        timeDifference = GameManager.Instance.time - timeStamp;
                        if (timeDifference > 2) {
                            timeDifference = 2;
                        }
                        timeStamp = GameManager.Instance.time;
                    }
                    if (sounded > 0) {
                        if (sounded == 1 || (savePosition - effectPosition).sqrMagnitude > 0.0625f) {
                            effectInstance = Instantiate(hitEffect[hitEffectNum], effectPosition, quaIden);
                            AudioSource aSrc = effectInstance.GetComponent<AudioSource>();
                            if (aSrc) {
                                aSrc.enabled = false;
                            }
                            sounded = 2;
                        }
                    } else {
                        float volumeRate = CharacterManager.Instance.GetDamageSoundVolume(colorTemp);
                        if (overrideColorType >= 0) {
                            volumeRate *= 0.5f;
                        }
                        if (volumeRate >= 1f) {
                            effectInstance = Instantiate(hitEffect[hitEffectNum], effectPosition, quaIden);
                        } else {
                            effectInstance = Instantiate(hitEffect[hitEffectNum], effectPosition, quaIden);
                            AudioSource aSrc = effectInstance.GetComponent<AudioSource>();
                            if (aSrc) {
                                if (volumeRate <= 0f) {
                                    aSrc.enabled = false;
                                } else {
                                    aSrc.volume *= volumeRate;
                                }
                            }
                        }
                        savePosition = effectPosition;
                        sounded = 1;
                    }
                    if (effectInstance) {
                        HoldParticle holdParticle = effectInstance.GetComponent<HoldParticle>();
                        if (holdParticle) {
                            for (int i = 0; i < holdParticle.pSets.Length; i++) {
                                if (holdParticle.pSets[i].particle) {
                                    var emissionModule = holdParticle.pSets[i].particle.emission;
                                    var particleBurst = emissionModule.GetBurst(0);
                                    particleBurst.count = Mathf.Clamp((int)(timeDifference * holdParticle.pSets[i].multiplier) + 1, holdParticle.pSets[i].minCount, holdParticle.pSets[i].maxCount);
                                    emissionModule.SetBurst(0, particleBurst);
                                }
                            }
                            if (holdParticle.playManual && holdParticle.parentParticle) {
                                holdParticle.parentParticle.Play(true);
                            }
                        }
                        if (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] != 0 && ((parentCBase && !parentCBase.isEnemy && !parentCBase.isPlayer) || (attackDetection && attackDetection.parentCBase && !attackDetection.parentCBase.isEnemy && !attackDetection.parentCBase.isPlayer))) {
                            AudioSource aSrc = effectInstance.GetComponent<AudioSource>();
                            if (aSrc) {
                                aSrc.volume *= CharacterManager.Instance.GetObscureRateAudio();
                            }
                        }
                    }
                }
                if (attackDetection != null) {
                    parentCBase.TakeDamage(damage, effectPosition, knockAmount * knockedRate, knockVector, attackDetection.parentCBase, colorTemp, penetrate);
                    parentCBase.RegisterTargetHate(attackDetection.parentCBase, knockAmount * knockedRate);
                    if (parentCBase.isEnemy){
                        if (attackDetection.parentCBase == CharacterManager.Instance.pCon) {
                            if (overrideColorType < 0) {
                                CharacterManager.Instance.JustDodgeAmountPlus(knockAmount * Mathf.Clamp(knockedRate, 0.5f, 2f));
                            }
                        }
                        if (attackDetection.reportType != AttackDetection.ReportType.None) {
                            //CheckTrophyPart
                            switch (attackDetection.reportType) {
                                case AttackDetection.ReportType.Judgement:
                                    if (parentCBase.GetNowHP() <= 0 && CharacterManager.Instance.pCon) {
                                        CharacterManager.Instance.pCon.CheckTrophy_Judgement();
                                    }
                                    break;
                                case AttackDetection.ReportType.Hippo:
                                    if (colorType == CharacterBase.damageColor_Effective && (parentCBase.GetEnemyID() == 40 || parentCBase.GetEnemyID() == 55)) {
                                        TrophyManager.Instance.CheckTrophy(TrophyManager.t_AlligatorClipsHippo, true);
                                    }
                                    break;
                                case AttackDetection.ReportType.PrairieDog:
                                    if (colorType == CharacterBase.damageColor_Effective && parentCBase.GetEnemyID() == 44) {
                                        TrophyManager.Instance.CheckTrophy(TrophyManager.t_PrairieDog, true);
                                        Enemy_Akula akulaTemp = parentCBase.GetComponent<Enemy_Akula>();
                                        if (akulaTemp) {
                                            akulaTemp.SetFollowerResetTimer();
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                } else {
                    parentCBase.TakeDamage(damage, effectPosition, knockAmount * knockedRate, knockVector, null, colorTemp, penetrate);
                }
                return true;
            }
        }
        return false;
    }

    public virtual void ReceiveSick(CharacterBase.SickType sickType, float sickTime = -1, AttackDetection attackDetection = null) {
        if (parentCBase && parentCBase.enabled) {
            parentCBase.SetSick(sickType, sickTime, attackDetection);
        }
    }

    public void NonDamageDodge(Vector3 pivotPosition) {
        if (parentCBase && parentCBase.enabled && parentCBase.GetCanTakeDamage(false) && checkDodge && parentCBase.DodgeChallenge()) {
            DodgeBody(pivotPosition);
        }
    }

    public float GetDefence() {
        return parentCBase ? parentCBase.GetDefense() * defenceRate : 0f;
    }

    public bool GetCanTakeDamage(bool penetrate = false) {
        return parentCBase && parentCBase.enabled && parentCBase.GetCanTakeDamage(penetrate);
    }

    public CharacterBase GetCharacterBase() {
        return parentCBase;
    }

}
