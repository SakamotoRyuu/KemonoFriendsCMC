using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ImperatrixSun : EnemyBase {

    public SearchTargetReference searchTargetReference;
    public Transform scalePivot;
    public AudioSource flareAudio;
    public bool burstFlag;
    public float knockMultiplier;
    public Renderer mapChipRenderer;
    public int burstDamage;

    bool readyFlag;
    float burstDeadTimer;
    float quakeInterval;
    float requireKnock = 20000;
    bool flareFadeoutFlag;
    float flareFadeTime;
    Enemy_ImperatrixMundi parentEnemy;
    float buildBreakTimeRemain;
    
    public GameObject timerTextSetPrefab;

    private float remainTime;
    private GameObject timerTextSetInstance;
    private TMPTextArrayHolder tmpHolder;

    string GetTimeText() {
        int remainInteger = (int)remainTime;
        int remainDecimal = (int)(remainTime * 100) % 100;
        return string.Format("{0:00}\'{1:00}\"{2:00}", remainInteger / 60, remainInteger % 60, remainDecimal);
    }

    protected override void Awake() {
        base.Awake();
        roveInterval = -1;
        attackWaitingLockonRotSpeed = 0;
        attackLockonDefaultSpeed = 0;
        SetDropRate(0);
        dropExpDisabled = true;
        mapChipSize = -1;
        sickHealSpeed = 10000;
        knockRecovery = 0f;
        spawnStiffTime = 0f;
        attackedTimeRemain = -1f;
        deadTimer = 0f;
        attractionTime = 0f;
        confuseTime = 0f;
        angryAttractionTime = 0f;
        angryFixTime = 0f;
        isBoss = true;
    }

    protected override void Start() {
        base.Start();
        remainTime = 10f;
        timerTextSetInstance = Instantiate(timerTextSetPrefab, PauseController.Instance.offPauseCanvas.transform);
        tmpHolder = timerTextSetInstance.GetComponent<TMPTextArrayHolder>();
        if (tmpHolder) {
            tmpHolder.tmpTexts[0].text = GetTimeText();
        }
    }

    public void SetSearchTargetReference(GameObject targetObj) {
        if (searchTargetReference && targetObj) {
            searchTargetReference.referObj = targetObj;
        }
    }

    public void SetParentEnemy(Enemy_ImperatrixMundi mundiBase, float requireKnock) {
        parentEnemy = mundiBase;
        this.requireKnock = requireKnock;
    }

    protected override void BootDeathEffect(EnemyDeath enemyDeath) {
        for (int i = 1; i < 6; i++) {
            enemyDeath.colorNum = i;
            base.BootDeathEffect(enemyDeath);
            enemyDeath.deadEffect.prefab = null;
        }
    }

    private void SetFadeout() {
        flareFadeoutFlag = true;
        flareFadeTime = 0f;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (Event_LastBattleSecond.Instance && Event_LastBattleSecond.Instance.eventNow) {
            Destroy(gameObject);
        }
        if (burstFlag) {
            if (buildBreakTimeRemain > 0f) {
                buildBreakTimeRemain -= deltaTimeCache;
                if (buildBreakTimeRemain <= 0f) {
                    if (StageManager.Instance && StageManager.Instance.graphBuildNowFloor) {
                        StageManager.Instance.graphBuildNowFloor.Recollapse();
                    }
                }
            }
            burstDeadTimer -= deltaTimeCache;
            if (burstDeadTimer <= 0f) {
                Destroy(gameObject);
            }
        } else {
            if (state == State.Attack) {
                if (flareAudio) {
                    flareFadeTime += deltaTimeCache;
                    if (flareFadeoutFlag) {
                        flareAudio.volume = Mathf.Clamp01(1f - flareFadeTime * 2f);
                    } else {
                        flareAudio.volume = Mathf.Clamp01(flareFadeTime * 0.25f);
                    }
                }
                if (scalePivot) {
                    if (!burstFlag) {
                        float scaleTemp = scalePivot.localScale.x;
                        ed.radius = Mathf.Clamp(scaleTemp * 0.5f, 1f, 20f);
                        ed.scale = Mathf.Lerp(0.265f, 0.53f, Mathf.Clamp01(scaleTemp / 32f));
                        ed.numOfPieces = Mathf.RoundToInt(Mathf.Lerp(16f, 80f, Mathf.Clamp01(scaleTemp / 32f)));
                        quakeInterval -= deltaTimeCache;
                        if (quakeInterval <= 0f) {
                            quakeInterval = 0.03f;
                            CameraManager.Instance.SetQuake(GetCenterPosition(), 0.25f + Mathf.Clamp(scaleTemp * 0.025f, 0f, 1f), 8, 0, 0.05f, 0, scaleTemp * 0.5f + 10f, scaleTemp * 2f + 40f);
                        }
                    }
                }
            } else {
                if (!readyFlag) {
                    SetState(State.Attack);
                } else {
                    Destroy(gameObject);
                }
            }
        }
        bool mapToActive = (StageManager.Instance.mapActivateFlag != 0);
        if (mapChipRenderer && mapChipRenderer.enabled != mapToActive) {
            mapChipRenderer.enabled = mapToActive;
        }
        if (Time.timeScale > 0f) {
            remainTime -= deltaTimeCache;
            if (remainTime < 0f) {
                remainTime = 0f;
            }
            if (tmpHolder) {
                tmpHolder.tmpTexts[0].text = GetTimeText();
            }
        }
        if (searchTargetReference && searchTarget[0] && CharacterManager.Instance.playerTarget == searchTarget[0]) {
            CameraManager.Instance.OverwriteTargetRadius(searchTargetReference.referRadius);
        }
    }

    protected override void DamageCommonProcess() {
        if (knockRemain <= knockEndurance - requireKnock && !burstFlag) {
            nowHP = 0;
        } else {
            nowHP = GetMaxHP();
        }
    }

    protected override void DeadProcess() {
        base.DeadProcess();
        CameraManager.Instance.SetQuake(GetCenterPosition(), 5, 8, 0, 0f, 3f, 50f, 200f);
        if (parentEnemy) {
            parentEnemy.ReceiveSunIndependentDead();
        }
        if (timerTextSetInstance) {
            AutoDestroy autoDestroy = timerTextSetInstance.GetComponent<AutoDestroy>();
            if (autoDestroy) {
                autoDestroy.life = 2f;
            }
        }
    }

    void Burst() {
        if (state != State.Dead && nowHP > 0) {
            attackPowerMultiplier = 10f;
            knockPowerMultiplier = 10f;
            throwing.ThrowStart(0);
            if (throwing.throwSettings[0].instance) {
                AttackDetectionProjectileFixDamage burstAD = throwing.throwSettings[0].instance.GetComponentInChildren<AttackDetectionProjectileFixDamage>();
                if (burstAD) {
                    burstAD.fixDamage = burstDamage;
                }
            }
            ed.numOfPieces = 0;
            ed.deadEffect.prefab = null;
            burstFlag = true;
            burstDeadTimer = 2f;
            buildBreakTimeRemain = 0.1f;
            CameraManager.Instance.SetQuake(GetCenterPosition(), 10, 8, 0, 0f, 2f, 50f, 200f);
            if (flareAudio) {
                flareAudio.Stop();
            }
            if (parentEnemy) {
                parentEnemy.ReceiveSunIndependentBurst();
            }
            if (timerTextSetInstance) {
                AutoDestroy autoDestroy = timerTextSetInstance.GetComponent<AutoDestroy>();
                if (autoDestroy) {
                    autoDestroy.life = 2f;
                }
            }
        }
    }    

    public override void SetLevel(int newLevel, bool effectFlag = false, bool isLevelUp = true, int variableLevel = 0) { }

    protected override void LoadEnemyCanvas() { }

    public override void SetSick(SickType sickType, float duration, AttackDetection attacker = null) { }

    public override void Attraction(GameObject decoy, AttractionType type, bool lockForce = false, float targetFixingTime = 0) { }

    public override void MakeAngry(GameObject decoyObject, CharacterBase attacker = null) { }

    public override void Confuse() { }

    protected override void SetMapChip() { }

    public override float GetKnocked() {
        float weaponMul = GameManager.Instance.save.GetEquip(5) ? 1f : GameManager.Instance.save.GetEquip(4) ? 1.2f : GameManager.Instance.save.GetEquip(3) ? 1.4f : 1.6f;
        if (knockMultiplier > 1f) {
            return 1f / (knockMultiplier * weaponMul);
        }
        return 1f / weaponMul;
    }

    protected override void Attack() {
        base.Attack();
        knockMultiplier = 1f;
        AttackBase(0, 10, 100, 0, 20f, 30f, 0, 1, false);
        readyFlag = true;
        flareFadeoutFlag = false;
        flareFadeTime = 0f;
        if (flareAudio) {
            flareAudio.volume = 0f;
            flareAudio.Play();
        }
    }
}
