using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Australopithecus : EnemyBase {

    [System.Serializable]
    public class WeaponSet {
        public GameObject obj;
        public int level;
        public int isLeft;
    }

    [System.Serializable]
    public class ShoutEnemySet {
        public EnemyBase eBase;
        public int distance;
        public ShoutEnemySet (EnemyBase eBase, int distance) {
            this.eBase = eBase;
            this.distance = distance;
        }
    }

    public WeaponSet[] weaponSet;
    public Transform[] movePivot;
    public GameObject[] corePoint;
    public GameObject[] healPivot;
    public GameObject normalDD;
    public GameObject weakDD;
    public Transform voicePivot;
    public Transform linePivot;
    public GameObject[] attackVoicePrefab;
    public GameObject[] knockLightVoicePrefab;
    public GameObject[] knockHeavyVoicePrefab;
    public GameObject[] shoutVoicePrefab;

    int moveIndex = -1;
    int aggroHash;
    int isLefty;
    int coreIndex;
    bool aggroSave;
    bool isCoreShow;
    float fatigue;
    int nowVoicePriority;
    float voiceElapsedTime;
    GameObject voiceInstance;
    AudioSource voiceAudioSource;
    int voiceAttackIndexSave = -1;
    int voiceKnockLightIndexSave = -1;
    int voiceKnockHeavyIndexSave = -1;
    int voiceShoutIndexSave = -1;
    bool fastAttackSave;
    float coreHideTimeRemain;
    float shoutReadyTime;
    float shoutInterval;
    bool shoutReadied;
    static readonly float[] knockPowerArray = new float[5] { 0.6f, 1.2f, 1.6f, 2.0f, 0.8f };
    static readonly int[] dropRandomArray = new int[4] { 21, 43, 58, 89 };
    static readonly int[] dropRandomArrayHomo = new int[5] { 28, 29, 52, 52, 57};
    const int impactEffectIndex = 2;
    const int throwReadyEffectIndex = 7;
    const int coreEffectIndex = 9;
    const int coreHealIndex = 14;
    const int priorityAttack = 1;
    const int priorityKnockLight = 2;
    const int priorityShout = 3;
    const int priorityKnockHeavy = 4;
    const int throwShoutIndex = 2;

    protected override void Awake() {
        base.Awake();
        mapChipSize = 1;
        aggroHash = Animator.StringToHash("Aggro");
        cannotDoubleKnockDown = true;
        isLefty = Random.Range(0, 2);
        coreIndex = Random.Range(0, corePoint.Length);
        if (corePoint[coreIndex] != null) {
            for (int i = coreEffectIndex; i < coreEffectIndex + 5 && i < effect.Length; i++) {
                effect[i].pivot = corePoint[coreIndex].transform;
            }
        }
        if (healPivot.Length > coreIndex && healPivot[coreIndex] != null) {
            for (int i = coreHealIndex; i < coreHealIndex + 5 && i < effect.Length; i++) {
                effect[i].pivot = healPivot[coreIndex].transform;
            }
        }
    }

    void HideCore() {
        for (int i = 0; i < corePoint.Length; i++) {
            if (corePoint[i] && corePoint[i].activeSelf) {
                corePoint[i].SetActive(false);
            }
        }
        if (normalDD && !normalDD.activeSelf) {
            normalDD.SetActive(true);
        }
        if (weakDD && weakDD.activeSelf) {
            weakDD.SetActive(false);
        }
        isCoreShow = false;
        coreHideTimeRemain = 0f;
    }

    void ShowCore() {
        if (corePoint[coreIndex] && !corePoint[coreIndex].activeSelf) {
            corePoint[coreIndex].SetActive(true);
            EmitEffect(coreEffectIndex + level);
        }
        if (normalDD && normalDD.activeSelf) {
            normalDD.SetActive(false);
        }
        if (weakDD && !weakDD.activeSelf) {
            weakDD.SetActive(true);
        }
        isCoreShow = true;
        coreHideTimeRemain = 0f;
    }

    void VoiceAttack() {
        if ((nowVoicePriority < priorityAttack || voiceElapsedTime > 0.6f) && voiceInstance) {
            Destroy(voiceInstance);
            voiceInstance = null;
            voiceAudioSource = null;
        }
        if (!voiceInstance) {
            int index = Random.Range(0, attackVoicePrefab.Length);
            if (index == voiceAttackIndexSave) {
                index = Random.Range(0, attackVoicePrefab.Length);
            }
            voiceAttackIndexSave = index;
            voiceInstance = Instantiate(attackVoicePrefab[index], voicePivot.position, voicePivot.rotation, voicePivot);
            voiceAudioSource = voiceInstance.GetComponent<AudioSource>();
            nowVoicePriority = priorityAttack;
            voiceElapsedTime = 0f;
        }
    }

    void VoiceKnockLight() {
        if ((nowVoicePriority < priorityKnockLight || voiceElapsedTime > 0.6f) && voiceInstance) {
            Destroy(voiceInstance);
            voiceInstance = null;
            voiceAudioSource = null;
        }
        if (!voiceInstance) {
            int index = Random.Range(0, knockLightVoicePrefab.Length);
            if (index == voiceKnockLightIndexSave) {
                index = Random.Range(0, knockLightVoicePrefab.Length);
            }
            voiceKnockLightIndexSave = index;
            voiceInstance = Instantiate(knockLightVoicePrefab[index], voicePivot.position, voicePivot.rotation, voicePivot);
            voiceAudioSource = voiceInstance.GetComponent<AudioSource>();
            nowVoicePriority = priorityKnockLight;
            voiceElapsedTime = 0f;
        }
    }

    void VoiceKnockHeavy() {
        if ((nowVoicePriority < priorityKnockHeavy || voiceElapsedTime > 0.6f) && voiceInstance) {
            Destroy(voiceInstance);
            voiceInstance = null;
            voiceAudioSource = null;
        }
        if (!voiceInstance) {
            int index = Random.Range(0, knockHeavyVoicePrefab.Length);
            if (index == voiceKnockHeavyIndexSave) {
                index = Random.Range(0, knockHeavyVoicePrefab.Length);
            }
            voiceKnockHeavyIndexSave = index;
            voiceInstance = Instantiate(knockHeavyVoicePrefab[index], voicePivot.position, voicePivot.rotation, voicePivot);
            voiceAudioSource = voiceInstance.GetComponent<AudioSource>();
            nowVoicePriority = priorityKnockHeavy;
            voiceElapsedTime = 0f;
        }
    }

    void VoiceShout() {
        if (state == State.Attack) {
            if (voiceInstance) {
                Destroy(voiceInstance);
                voiceInstance = null;
                voiceAudioSource = null;
            }
            int index = Random.Range(0, shoutVoicePrefab.Length);
            if (index == voiceShoutIndexSave) {
                index = Random.Range(0, shoutVoicePrefab.Length);
            }
            voiceShoutIndexSave = index;
            voiceInstance = Instantiate(shoutVoicePrefab[index], voicePivot.position, voicePivot.rotation, voicePivot);
            voiceAudioSource = voiceInstance.GetComponent<AudioSource>();
            nowVoicePriority = priorityShout;
            voiceElapsedTime = 0f;
        }
    }

    protected override void SetLevelModifier() {
        ReleaseAttackDetections();
        for (int i = 0; i < weaponSet.Length; i++) {
            if (weaponSet[i].obj != null) {
                weaponSet[i].obj.SetActive(weaponSet[i].level == level && weaponSet[i].isLeft == isLefty);
            }
        }
        if (level >= 4) {
            actDistNum = 2;
        } else if (level >= 1) {
            actDistNum = 0;
        } else {
            actDistNum = 1;
        }
        HideCore();
        if (isHomoChild) {
            dropItem[0] = dropRandomArrayHomo[Random.Range(0, dropRandomArrayHomo.Length)];
        } else {
            if (level <= 0) {
                dropItem[0] = -1;
            } else {
                dropItem[0] = dropRandomArray[Random.Range(0, dropRandomArray.Length)];
            }
        }
        fastAttackSave = false;
        shoutReadyTime = 0f;
    }

    void MoveStart() {
        moveIndex = level;
        LockonStart();
    }

    void MoveEnd() {
        moveIndex = -1;
        LockonEnd();
    }

    void EmitEffectImpact() {
        EmitEffect(impactEffectIndex + level);
    }

    void EmitEffectReady() {
        EmitEffect(throwReadyEffectIndex + isLefty);
    }

    protected override void Update_AnimControl() {
        base.Update_AnimControl();
        UpdateAC_Speed();
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        bool aggroTemp = (target != null);
        if (aggroSave != aggroTemp) {
            aggroSave = aggroTemp;
            anim.SetBool(aggroHash, aggroSave);
        }
        if (state == State.Attack) {
            fatigue += deltaTimeCache * 2f;
        } else {
            fatigue -= deltaTimeCache;
        }
        fatigue = Mathf.Clamp(fatigue, 0f, 10f);
        if (voiceInstance && voiceAudioSource) {
            if (voiceAudioSource.isPlaying) {
                voiceElapsedTime += deltaTimeCache;
            } else {
                Destroy(voiceInstance);
                voiceInstance = null;
                voiceAudioSource = null;
                nowVoicePriority = 0;
            }
        }
        if (level >= 4) {
            if (targetTrans && Physics.Linecast(targetTrans.position, linePivot.position, fieldLayerMask)) {
                actDistNum = 2;
                if (attackedTimeRemain < 0.1f && GetTargetDistance(true, true, false) > 4f * 4f) {
                    attackedTimeRemain = 0.2f;
                }
            } else {
                actDistNum = 3;
            }
        }
        if (isCoreShow && coreHideTimeRemain > 0f) {
            coreHideTimeRemain -= deltaTimeCache;
            if (coreHideTimeRemain <= 0f) {
                HideCore();
            }
        }
        if (shoutInterval > 0f) {
            shoutInterval -= deltaTimeCache;
        }
        if (enemyCanvasLoaded && isCoreShow && target && nowHP < GetMaxHP() && !enemyCanvasChildObject[(int)EnemyCanvasChild.paperPlane].activeSelf && !enemyCanvasChildObject[(int)EnemyCanvasChild.margayVoice].activeSelf) {
            shoutReadyTime += deltaTimeCache;
        } else {
            shoutReadyTime -= deltaTimeCache;
        }
        shoutReadyTime = Mathf.Clamp(shoutReadyTime, 0f, 10f);
        if (!isHomoChild && shoutInterval <= 0f && shoutReadyTime >= 10f * CharacterManager.Instance.riskyDecrease && targetTrans && GetTargetDistance(true, true, false) <= 6f * 6f && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.australopithecusShoutInterval <= 0f && StageManager.Instance.dungeonController.EnemyCountDistance(trans.position, 50f, true) >= 2 && !enemyCanvasChildObject[(int)EnemyCanvasChild.paperPlane].activeSelf && !enemyCanvasChildObject[(int)EnemyCanvasChild.margayVoice].activeSelf) {
            shoutReadied = true;
            if (level <= 0) {
                actDistNum = 5;
            } else if (level <= 3) {
                actDistNum = 4;
            }
        } else {
            shoutReadied = false;
            if (level <= 0) {
                actDistNum = 1;
            } else if (level <= 3) {
                actDistNum = 0;
            }
        }
    }

    public override void AttackStart(int index) {
        base.AttackStart(level * 2 + index);
    }

    public override void AttackEnd(int index) {
        base.AttackEnd(level * 2 + index);
    }

    public override bool SetConcentratedAttack(Vector3 parentPosition, GameObject decoy, CharacterBase attacker = null) {
        if (base.SetConcentratedAttack(parentPosition, decoy, attacker)) {
            if (shoutInterval < 8f) {
                shoutInterval = 8f;
            }
            if (isCoreShow && coreHideTimeRemain <= 0f && nowHP >= GetMaxHP()) {
                coreHideTimeRemain = 1f;
                EmitEffect(coreHealIndex + Mathf.Clamp(level, 0, 4));
            }
            return true;
        }
        return false;
    }

    protected override void KnockLightProcess() {
        if (!isSuperarmor && knockRestoreSpeed != 1f) {
            knockRestoreSpeed = 1f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
        base.KnockLightProcess();
        if (!isSuperarmor && nowHP > 0) {
            VoiceKnockLight();
        }
    }

    protected override void KnockHeavyProcess() {
        if (knockRestoreSpeed != 0.8f) {
            knockRestoreSpeed = 0.8f;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.KnockRestoreSpeed], knockRestoreSpeed);
        }
        if (nowHP > 0) {
            ShowCore();
        }
        base.KnockHeavyProcess();
        if (nowHP > 0) {
            VoiceKnockHeavy();
        }
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (nowHP > 0 && nowHP <= GetMaxHP() * 3 / 4) {
            ShowCore();
        }
    }

    void ThrowShout() {
        if (state == State.Attack) {
            throwing.ThrowReady(throwShoutIndex);
            shoutInterval = 20f;
        }
    }

    protected override void Attack() {
        base.Attack();
        float intervalPlus = 0f;
        float stiffRate = 1f;
        float spRate = (isSuperman ? 47f / 30f : isCoreShow ? 47f / 33f : 47f / 36f);
        if (shoutReadied) {
            AttackBase(4, 0f, 0f, 0, 66f / 30f, 66f / 30f, 0f, 1f, false);
            SuperarmorStart();
            shoutInterval = 10f;
            StageManager.Instance.dungeonController.australopithecusShoutInterval = 4f;
        } else {
            if (fatigue >= 8f) {
                fatigue -= 8f;
                intervalPlus = GetAttackInterval(1.5f, 0, 0f);
            }
            if (fastAttackSave || !isCoreShow || intervalPlus > 0.1f) {
                fastAttackSave = false;
            } else {
                fastAttackSave = (Random.Range(0, 100) < 25);
            }
            if (fastAttackSave) {
                stiffRate = 0.85f;
            }
            if (level <= 3) {
                AttackBase(isLefty, 1f, knockPowerArray[Mathf.Clamp(level, 0, knockPowerArray.Length - 1)], 0, 47f / 30f / spRate * stiffRate, 47f / 30f / spRate * stiffRate + intervalPlus, 0f, spRate);
                MoveStart();
            } else {
                AttackBase(2 + isLefty, 1f, knockPowerArray[Mathf.Clamp(level, 0, knockPowerArray.Length - 1)], 0, 47f / 30f / spRate * stiffRate, 47f / 30f / spRate * stiffRate + intervalPlus, 0.5f, spRate);
                fbStepTime = 0.3f;
                fbStepMaxDist = 2f;
                SeparateFromTarget(4f);
            }
            VoiceAttack();
        }
    }

    protected override void AttackContinuous() {
        base.AttackContinuous();
        if (moveIndex >= 0 && moveIndex < movePivot.Length && movePivot[moveIndex] != null) {
            ApproachTransformPivot(movePivot[moveIndex], GetMinmiSpeed(), 0.4f, 0.125f, true);
        }
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return base.GetMaxSpeed(isWalk, ignoreSuperman, ignoreSick, ignoreConfig) * (isCoreShow ? 1.2f : 1f);
    }

    public override float GetAcceleration() {
        return base.GetAcceleration() * (isCoreShow ? 1.2f : 1f);
    }

    public override float GetDefense(bool ignoreMultiplier = false) {
        return base.GetDefense(ignoreMultiplier) * (isCoreShow ? 2f : 1f);
    }

}
