using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Queen : EnemyBaseBoss {

    public int[] attackGroupStartIndex;
    public int[] attackGroupMax;
    public int[] throwGroupStartIndex;
    public int[] throwGroupMax;
    public GameObject[] plusAttacker;
    public LaserOption[] laserOption;
    public RaycastToAdjustCapsuleCollider[] raycaster;
    public GameObject[] cellienCore;
    public GameObject[] defaultDetection;
    public GameObject[] anotherDetection;
    public Transform quakePivotDissodinium;
    public Transform quakePivotBirdlien;
    public Transform quakePivotKnock;
    public SupermanSettings emissionMatSettings;
    public CheckTriggerStay checkTriggerStay;
    public GameObject navMeshPrefab;
    public GameObject remainEnemy;
    public Renderer eyeRenderer;
    public int eyeRendIndex;
    public Material shiromeMat;
    public bool isReaper;
    public LayerMask blockedLayerMask;

    int attackSave = -1;
    float attackSpeed = 1;
    bool laserEnabled = false;
    float hornAttackedTimeRemain = 0f;
    float laserAttackedTimeRemain = 0f;
    bool plusAttackerActivated = true;
    Vector3 lockonRandomizePosition;
    int saveWeakProgress = -1;
    float[] throwVelocityBase;
    const float velocityUpCondition = 7f;
    bool healEffectEmitted = false;
    bool heavyKnocked = false;
    int coreIndex = -1;
    static readonly float[] throwDistanceBiasRate = new float[] { 0.5f, 0.3f, 0.1f };
    float randomDirectionUpdateRemain = 0f;
    Vector3 randomDirection;
    GameObject navMeshInstance;
    int swingDirection = 0;
    float swingTime = 0f;
    float wallBreakTimeRemain;
    bool anotherTalked;
    bool breakdownFlag;

    const int coreShowEffectIndex = 22;
    const int coreHideEffectIndex = 23;
    const int effectIndexBreakdown = 32;
    const int dropID_EX = 339;
    const int dropID_Another = 338;
    const int attackLaserBody = 32;
    const int anotherServalID = 31;
    const int attackIndexDying = 10;

    protected override void Awake() {
        base.Awake();
        mutekiTimeRemain = spawnStiffTime = 105f / 30f;
        randomDirection = new Vector3();
        randomDirectionUpdateRemain = 0f;
        deadTimer = 3.5f;
        actDistNum = 0;
        attackWaitingLockonRotSpeed = 1f;
        checkGroundPivotHeight = 1f;
        checkGroundTolerance = 2f;
        checkGroundTolerance_Jumping = 2f;
        sandstarRawKnockEndurance = 1000000;
        sandstarRawKnockEnduranceLight = 6000;
        LaserCancel();
        CheckPlusAttacker();
        CoreHide();
        if (throwing) {
            throwVelocityBase = new float[throwing.throwSettings.Length];
            for (int i = 0; i < throwVelocityBase.Length; i++) {
                throwVelocityBase[i] = throwing.throwSettings[i].velocity;
            }
        }
        killByCriticalOnly = true;
        coreShowHP = GetMaxHP();
        // coreHideDenomi = 9f;
        if (isReaper) {
            changeMusicEnabled = false;
            coreHideDenomi = 12f;
            roveInterval = 8f;
            knockRemain = knockEndurance = sandstarRawKnockEndurance;
            knockRemainLight = knockEnduranceLight = sandstarRawKnockEnduranceLight;
            attackLockonDefaultSpeed = sandstarRawLockonSpeed;
            friendsKnockRate = sandstarRawFriendsKnockRate;
            costKnockedBase = 12;
            maxSpeed = sandstarRawMaxSpeed;
            acceleration = sandstarRawAcceleration;
            if (agent) {
                agent.speed = maxSpeed;
                agent.acceleration = acceleration;
            }
        } else {
            coreHideDenomi = 6f;
        }
        coreTimeMax = 20f;
    }

    void SetEmissionMat(bool flag) {
            for (int i = 0; i < emissionMatSettings.mats.Length; i++) {
                Material[] mats = emissionMatSettings.mats[i].changeMatRenderer.materials;
                if (flag) {
                    mats[emissionMatSettings.mats[i].materialIndex] = emissionMatSettings.mats[i].specialMaterial;
                } else {
                    mats[emissionMatSettings.mats[i].materialIndex] = emissionMatSettings.mats[i].defaultMaterial;
                }
                emissionMatSettings.mats[i].changeMatRenderer.materials = mats;
            }
            emissionMatSettings.isSpecial = flag;
    }

    void EmissionActivate(int flag) {
        if (emissionMatSettings.isSpecial != (flag != 0)) {
            SetEmissionMat((flag != 0));
        }
    }

    void CheckPlusAttacker() {
        if (weakProgress <= 0 && plusAttackerActivated) {
            for (int i = 0; i < plusAttacker.Length; i++) {
                plusAttacker[i].SetActive(false);
            }
            plusAttackerActivated = false;
        } else if (weakProgress >= 1 && !plusAttackerActivated) {
            for (int i = 0; i < plusAttacker.Length; i++) {
                plusAttacker[i].SetActive(true);
            }
            plusAttackerActivated = true;
        }
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        knockRemain = knockEndurance;
        if (state != State.Attack && laserEnabled) {
            LaserCancel();
        }
        if (state != State.Attack) {
            EmissionActivate(0);
        }
        if (GameManager.Instance.save.difficulty >= GameManager.difficultyVU || sandstarRawEnabled) {
            weakProgress = (nowHP <= GetMaxHP() / 3 ? 2 : nowHP <= GetMaxHP() * 2 / 3 ? 1 : 0);
        } else {
            weakProgress = (nowHP <= GetMaxHP() / 2 ? 1 : 0);
        }
        if (isReaper) {
            attackSpeed = (weakProgress == 2 ? 1.3f : 1.15f);
            maxSpeed = 13.5f;
        } else {
            attackSpeed = (weakProgress == 2 ? 1.15f : 1);
            if (!sandstarRawEnabled) {
                maxSpeed = (weakProgress == 2 ? 7f : weakProgress == 1 ? 6f : 5f);
            }
        }
        if (saveWeakProgress != weakProgress) {
            saveWeakProgress = weakProgress;
            anim.SetFloat(AnimHash.Instance.ID[(int)AnimHash.ParamName.AnimSpeed], (weakProgress == 2 ? 1.2f : weakProgress == 1 ? 1.1f : 1f) + (isReaper ? 0.2f : 0f));
        }
        attackedTimeRemainOnDamage = (weakProgress == 2 ? 0.5f : weakProgress == 1 ? 0.8f : 1f);
        hornAttackedTimeRemain -= deltaTimeMove;
        laserAttackedTimeRemain -= deltaTimeMove;
        if (isCoreShowed && state != State.Dead) {
            coreTimeRemain -= deltaTimeCache * (coreTimeRemain >= 1f && nowHP > 1 ? CharacterManager.Instance.riskyIncSqrt : 1f);
            if (!healEffectEmitted && coreTimeRemain < 1) {
                EmitEffect(coreHideEffectIndex + coreIndex * 2);
                healEffectEmitted = true;
            }
            if (coreTimeRemain < 0) {
                CoreHide();
            }
        }
        if (state != State.Attack && navMeshInstance) {
            Destroy(navMeshInstance);
            navMeshInstance = null;
        }
        if (state != State.Attack && swingDirection != 0) {
            EndSwing();
        }
        if (wallBreakTimeRemain > 0f && state != State.Attack) {
            wallBreakTimeRemain -= deltaTimeCache;
        }
        if (isReaper) {
            if (targetTrans && attackedTimeRemain < 0.1f) {
                float sqrDist = GetTargetDistance(true, true, false);
                if ((sqrDist > 20f * 20f && wallBreakTimeRemain > 0f) || (sqrDist > 40f * 40f && !Physics.Linecast(GetCenterPosition(), targetTrans.position, blockedLayerMask, QueryTriggerInteraction.Collide))) {
                    attackedTimeRemain = 0.2f;
                }
            }
        }
        if (!anotherTalked && !isForAmusement && battleStarted) {
            if (GameManager.Instance.IsPlayerAnother) {
                Vector3 playerPos = CharacterManager.Instance.playerTrans.position;
                float sqrDist = (playerPos - trans.position).sqrMagnitude;
                if (sqrDist < 10f * 10f || (sqrDist < 20f * 20f && !Physics.Linecast(playerPos + vecUp, trans.position + vecUp, fieldLayerMask))) {
                    anotherTalked = true;
                    PlayerController_Another pConAnother = CharacterManager.Instance.pCon.GetComponent<PlayerController_Another>();
                    if (pConAnother) {
                        pConAnother.QueenBattleStartSpeech();
                    }
                }
            } else if (CharacterManager.Instance.GetFriendsExist(anotherServalID, true)) {
                Vector3 friendsPos = CharacterManager.Instance.friends[anotherServalID].trans.position;
                float sqrDist = (friendsPos - trans.position).sqrMagnitude;
                if (sqrDist < 10f * 10f || (sqrDist < 20f * 20f && !Physics.Linecast(friendsPos + vecUp, trans.position + vecUp, fieldLayerMask))) {
                    anotherTalked = true;
                    Friends_AnotherServal fBaseAnother = CharacterManager.Instance.friends[anotherServalID].fBase.GetComponent<Friends_AnotherServal>();
                    if (fBaseAnother) {
                        fBaseAnother.QueenBattleStartSpeech();
                    }
                }
            }
        }
        CharacterManager.Instance.CheckTrophy_Gather_ForQueen(state == State.Attack && attackType == 9 && laserEnabled && checkTriggerStay && checkTriggerStay.stayFlag);
    }

    private void CoreHide() {
        for (int i = 0; i < cellienCore.Length; i++) {
            cellienCore[i].SetActive(false);
        }
        for (int i = 0; i < defaultDetection.Length; i++) {
            defaultDetection[i].SetActive(true);
        }
        for (int i = 0; i < anotherDetection.Length; i++) {
            anotherDetection[i].SetActive(false);
        }
        isCoreShowed = false;
        knockRemainLight = knockEnduranceLight;
    }

    private void CoreShow() {
        coreIndex = (coreIndex + 1) % cellienCore.Length;
        cellienCore[coreIndex].SetActive(true);
        for (int i = 0; i < defaultDetection.Length; i++) {
            defaultDetection[i].SetActive(false);
        }
        for (int i = 0; i < anotherDetection.Length; i++) {
            anotherDetection[i].SetActive(true);
        }
        isCoreShowed = true;
        coreTimeRemain = coreTimeMax;
        coreShowHP = nowHP;
        coreHideConditionDamage = GetCoreHideConditionDamage();
        healEffectEmitted = false;
        heavyKnocked = false;
        EmitEffect(coreShowEffectIndex + coreIndex * 2);
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (!isCoreShowed && state != State.Dead && state != State.Spawn && mutekiTimeRemain <= 0f) {
            CoreShow();
        }
        if (isCoreShowed && nowHP == 1 && !isSuperarmor) {
            breakdownFlag = true;
            if (attackedTimeRemain > lightStiffTime) {
                attackedTimeRemain = lightStiffTime;
            }
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        heavyKnocked = true;
        if (isCoreShowed && coreTimeRemain > 1.25f) {
            coreTimeRemain = 1.25f;
        }
        if (isCoreShowed && nowHP == 1) {
            breakdownFlag = true;
            if (attackedTimeRemain > heavyStiffTime) {
                attackedTimeRemain = heavyStiffTime;
            }
        }
    }

    protected override void SetLevelModifier() {
        if (isReaper) {
            if (GameManager.Instance.IsPlayerAnother) {
                dropItem[0] = dropID_Another;
            } else {
                dropItem[0] = dropID_EX;
            }
            SetDropRate(10000);
        }
        if (!isReaper && GameManager.Instance.IsPlayerAnother) {
            dropItem[0] = dropID_EX;
            SetDropRate(10000);
        }
    }

    public override void SetSandstarRaw() {
        base.SetSandstarRaw();
        if (state != State.Dead) {
            // coreHideDenomi = 15f;
            coreHideDenomi = 12f;
        }
    }

    public override float GetKnocked() {
        if (state == State.Attack && attackType == attackIndexDying) {
            return 20000f;
        }
        return base.GetKnocked() * (!heavyKnocked && isCoreShowed && nowHP <= GetCoreHideBorder() ? 0 : 1);
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        if (effect[effectIndexBreakdown].instance) {
            Destroy(effect[effectIndexBreakdown].instance);
        }
    }

    protected override void DeadProcess() {
        if (remainEnemy) {
            GameObject remainObj = Instantiate(remainEnemy, trans.position, trans.rotation);
            Enemy_Euglena euglena = remainObj.GetComponent<Enemy_Euglena>();
            if (euglena) {
                euglena.SetForRemain(isReaper);
            }
        }
        if (!isLastOne) {
            dropItem[0] = -1;
            SetDropRate(0);
        }
        if (isReaper) {
            TrophyManager.Instance.CheckTrophy(TrophyManager.t_DefeatQueenRaw, true);
        }
        base.DeadProcess();
        if (isReaper && GameManager.Instance.IsPlayerAnother) {
            GameManager.Instance.SetSteamAchievement("ANOTHERQUEENRAW");
        }
    }

    void EmitEffectSpecial(int index) {
        switch (index) {
            case -2:
                EmitEffect(0);
                break;
            case -1:
                EmitEffect(1);
                break;
            case 0:
                EmitEffect(2);
                break;
            case 1:
                EmitEffect(3);
                break;
            case 10:
                EmitEffect(4);
                break;
            case 11:
                EmitEffect(5);
                break;
            case 20:
                EmitEffect(6);
                break;
            case 21:
                EmitEffect(7);
                break;
            case 30:
                EmitEffect(8);
                break;
            case 31:
                EmitEffect(9);
                EmitEffect(10);
                EmitEffect(11);
                EmitEffect(12);
                EmitEffect(13);
                break;
            case 40:
                EmitEffect(14);
                break;
            case 50:
                EmitEffect(15);
                break;
            case 60:
                EmitEffect(16);
                break;
            case 70:
                EmitEffect(17);
                EmitEffect(18);
                EmitEffect(19);
                break;
            case 71:
                EmitEffect(20);
                break;
            case 80:
                EmitEffect(21);
                break;
            case 90:
                EmitEffect(31);
                break;
        }
    }

    void MoveAttack(int index) {
        if (state == State.Attack) {
            float sqrDist = 10f;
            if (targetTrans) {
                sqrDist = GetTargetDistance(true, true, false);
            }
            fbStepIgnoreY = true;
            fbStepEaseType = EasingType.SineInOut;
            switch (index) {
                case 1:
                    if (sqrDist < 3f * 3f) {
                        fbStepMaxDist = 4f;
                        fbStepTime = 30f / 60f;
                        BackStep(3f);
                    } else if (sqrDist > 3.5f * 3.5f){
                        SpecialStep(3.5f, 30f / 60f, 4f, 0f, 0f, true, false);
                    }
                    break;
                case 2:
                    if (sqrDist < 3f * 3f) {
                        fbStepMaxDist = 4f;
                        fbStepTime = 30f / 60f;
                        BackStep(3f);
                    } else if (sqrDist > 3.5f * 3.5f){
                        SpecialStep(3.5f, 30f / 60f, 4f, 0f, 0f, true, false);
                    }
                    break;
                case 3:
                    if (sqrDist < 3f * 3f) {
                        fbStepMaxDist = 3.4f;
                        fbStepTime = 25f / 60f;
                        BackStep(3f);
                    } else if (sqrDist > 4f * 4f){
                        SpecialStep(4f, 25f / 60f, 3.4f, 0f, 0f, true, false);
                    }
                    break;
                case 4:
                    if (sqrDist < 3f * 3f) {
                        fbStepMaxDist = 3.4f;
                        fbStepTime = 25f / 60f;
                        BackStep(3f);
                    } else if (sqrDist > 5.5f * 5.5f ){
                        SpecialStep(5.5f, 25f / 60f, 3.4f, 0f, 0f, true, false);
                    }
                    break;
                case 5:
                    if (sqrDist < 3f * 3f) {
                        fbStepMaxDist = 3.4f;
                        fbStepTime = 25f / 60f;
                        BackStep(3f);
                    } else if (sqrDist > 7f * 7f){
                        SpecialStep(7f, 25f / 60f, 3.4f, 0f, 0f, true, false);
                    }
                    break;
                case 6:
                    if (sqrDist < 4f * 4f) {
                        fbStepMaxDist = 4f;
                        fbStepTime = 30f / 60f;
                        SeparateFromTarget(4f);
                    } else if (sqrDist > 8f * 8f) {
                        SpecialStep(8f, 30f / 60f, 4f, 0f, 0f, true, false);
                    }
                    break;
                case 7:
                    if (sqrDist < 4f * 4f) {
                        fbStepMaxDist = 4f;
                        fbStepTime = 30f / 60f;
                        SeparateFromTarget(4f);
                    }
                    break;
                case 8:
                    if (sqrDist < 6f * 6f) {
                        fbStepMaxDist = 4f;
                        fbStepTime = 30f / 60f;
                        SeparateFromTarget(6f);
                    }
                    break;
            }
        }
    }

    void Quake(int index) {
        switch (index) {
            case 3:
                if (state == State.Attack) {
                    CameraManager.Instance.SetQuake(quakePivotDissodinium.position, 10, 4, 0, 0, 1.5f, 4f);
                }
                break;
            case 8:
                if (state == State.Attack) {
                    CameraManager.Instance.SetQuake(quakePivotBirdlien.position, 5, 8, 0, 1, 1f, 4f);
                }
                break;
            case -1:
                if (state == State.Damage) {
                    CameraManager.Instance.SetQuake(quakePivotKnock.position, 5, 4, 0, 0, 1.5f, 4f);
                }
                break;
        }
    }
    
    void AttackStartGroup(int index) {
        if (index >= 0 && index < attackGroupStartIndex.Length && index < attackGroupMax.Length) {
            for (int i = 0; i < attackGroupMax[index]; i++) {
                AttackStart(attackGroupStartIndex[index] + i);
            }
        }
    }

    void AttackEndGroup(int index) {
        if (index >= 0 && index < attackGroupStartIndex.Length && index < attackGroupMax.Length) {
            for (int i = 0; i < attackGroupMax[index]; i++) {
                AttackEnd(attackGroupStartIndex[index] + i);
            }
        }
    }

    void ActivateXWeaponTrailsGroup(int index) {
        if (index >= 0 && index < attackGroupStartIndex.Length && index < attackGroupMax.Length) {
            for (int i = 0; i < attackGroupMax[index]; i++) {
                attackDetection[attackGroupStartIndex[index] + i].ActivateXWeaponTrails();
            }
        }
    }

    void ThrowReadyGroup(int index) {
        if (index >= 0 && index < throwGroupStartIndex.Length && index < throwGroupMax.Length && state == State.Attack) {
            for (int i = 0; i < throwGroupMax[index]; i++) {
                throwing.ThrowReady(throwGroupStartIndex[index] + i);
            }
        }
    }

    void ThrowStartGroup(int index) {
        if (index >= 0 && index < throwGroupStartIndex.Length && index < throwGroupMax.Length && state == State.Attack) {
            for (int i = 0; i < throwGroupMax[index]; i++) {
                float dist = 0f;
                int pointer = throwGroupStartIndex[index] + i;
                if (pointer < throwing.throwSettings.Length) {
                    if (targetTrans) {
                        dist = Vector3.Distance(targetTrans.position, throwing.throwSettings[pointer].from.transform.position);
                    }
                    throwing.throwSettings[pointer].velocity = throwVelocityBase[pointer] + (dist > velocityUpCondition ? (dist - velocityUpCondition) * throwDistanceBiasRate[index] : 0f);
                    throwing.ThrowStart(pointer);
                }
            }
        }
    }

    void LaserCancel() {
        laserEnabled = false;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].CancelLaser();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].Deactivate();
            }
        }
        EmissionActivate(0);
    }

    void LaserReady() {
        laserEnabled = true;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringChargeStart();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].hitEffectEnabled = false;
                raycaster[i].Activate();
            }
        }
        EmissionActivate(1);
    }

    void LaserStart() {
        laserEnabled = true;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringChargeEnd();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].hitEffectEnabled = true;
                raycaster[i].Activate();
            }
        }
    }

    void LaserEnd() {
        laserEnabled = false;
        for (int i = 0; i < laserOption.Length; i++) {
            if (laserOption[i]) {
                laserOption[i].LightFlickeringBlastEnd();
            }
        }
        for (int i = 0; i < raycaster.Length; i++) {
            if (raycaster[i]) {
                raycaster[i].Deactivate();
            }
        }
        EmissionActivate(0);
    }

    void SetShirome() {
        if (eyeRenderer) {
            Material[] matsTemp = eyeRenderer.materials;
            matsTemp[eyeRendIndex] = shiromeMat;
            eyeRenderer.materials = matsTemp;
        }
    }

    int GetAttackType() {
        int min = 0;
        int max = (laserAttackedTimeRemain <= 0f ? 10 : 9);
        if (isReaper && targetTrans && wallBreakTimeRemain <= 0f) {
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist > 20f * 20f && Physics.Linecast(GetCenterPosition(), targetTrans.position, blockedLayerMask, QueryTriggerInteraction.Ignore)) {
                wallBreakTimeRemain = 5f;
                return (laserAttackedTimeRemain <= 0f ? 9 : 8);
            }
        }
        if (targetTrans) {
            float sqrDist = GetTargetDistance(true, true, false);
            if (sqrDist < 1.2f * 1.2f && targetTrans.position.y >= trans.position.y + 3f && hornAttackedTimeRemain <= 0f) {
                return 0;
            } else if (laserAttackedTimeRemain <= -10f && Random.Range(0, 2) == 0) {
                return 9;
            } else {
                if (sqrDist < 5.5f * 5.5f) {
                    min = 1;
                } else if (sqrDist < 7.5f * 7.5f) {
                    min = 3;
                } else if (sqrDist < 8.5f * 8.5f) {
                    min = 5;
                } else {
                    min = 6;
                }
            }
        }
        return Random.Range(min, max);
    }

    protected override Vector3 GetTargetVector(bool ignoreY = true, bool normalize = true, bool reverse = false) {
        if (attackType == 9 && checkTriggerStay && checkTriggerStay.stayFlag && !GetSick(SickType.Stop)) {
            randomDirectionUpdateRemain -= deltaTimeMove;
            if (randomDirectionUpdateRemain <= 0f) {
                randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                randomDirectionUpdateRemain = Random.Range(0.2f, 0.3f);
            }
            return randomDirection;
        } else if (swingDirection != 0) {
            swingTime += deltaTimeMove;
            Vector3 answer = base.GetTargetVector(ignoreY, normalize, reverse);
            answer = Quaternion.Euler(0f, Mathf.Sin(2f * Mathf.PI * swingTime) * 25f * swingDirection, 0f) * answer;
            return answer;
        }
        return base.GetTargetVector(ignoreY, normalize, reverse);
    }

    void StartSwing() {
        if (targetTrans && weakProgress >= 2) {
            swingDirection = (Vector3.Cross(trans.TransformDirection(vecForward), targetTrans.position - trans.position).y < 0 ? -1 : 1);
            swingTime = 0f;
        }
    }

    void EndSwing() {
        swingDirection = 0;
        swingTime = 0f;
    }

    void SetNavMesh() {
        if (navMeshInstance) {
            Destroy(navMeshInstance);
        }
        navMeshInstance = Instantiate(navMeshPrefab, trans.position, trans.rotation, trans);
    }

    protected override void BattleEnd() {
        base.BattleEnd();
        if (isReaper && StageManager.Instance) {
            StageManager.Instance.DefeatReaper();
        }
        if (GameManager.Instance.IsPlayerAnother) {
            PlayerController_Another pConAnother = CharacterManager.Instance.pCon.GetComponent<PlayerController_Another>();
            if (pConAnother) {
                pConAnother.QueenBattleEndSpeech();
            }
        } else if (CharacterManager.Instance.GetFriendsExist(anotherServalID, true)) {
            Friends_AnotherServal fBaseAnother = CharacterManager.Instance.friends[anotherServalID].fBase.GetComponent<Friends_AnotherServal>();
            if (fBaseAnother) {
                fBaseAnother.QueenBattleEndSpeech();
            }
        }
    }

    protected override void Attack() {
        if (breakdownFlag) {
            AttackBase(attackIndexDying, 0, 0, 0, 4.25f, 4.5f, 0, 1, false);
            breakdownFlag = false;
            SuperarmorStart();
            return;
        }
        CheckPlusAttacker();
        if (!battleStarted) {
            BattleStart();
        }
        if (attackSave < 0) {
            attackType = 9;
        } else {
            attackType = GetAttackType();
            if (attackType == attackSave) {
                attackType = GetAttackType();
            }
        }
        attackSave = attackType;
        float intervalPlus = (weakProgress == 2 ? Random.Range(0.4f, 0.6f) : weakProgress == 1 ? Random.Range(0.6f, 0.9f) : Random.Range(0.9f, 1.2f));
        switch (attackType) {
            case 0:
                AttackBase(0, 1.04f, 1.1f, 0, (110f / 60f) / attackSpeed, (110f / 60f) / attackSpeed, 0, attackSpeed, false);
                hornAttackedTimeRemain = 6f;
                break;
            case 1:
                AttackBase(1, 1f, 0.8f, 0, (90f / 60f) / attackSpeed, (90f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                break;
            case 2:
                AttackBase(2, 1.08f, 1.7f, 0, (135f / 60f) / attackSpeed, (135f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                break;
            case 3:
                AttackBase(3, 1.04f, 1.4f, 0, (105f / 60f) / attackSpeed, (105f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                break;
            case 4:
                AttackBase(4, 1.08f, 1.4f, 0, (110f / 60f) / attackSpeed, (110f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                break;
            case 5:
                AttackBase(5, 1.04f, 1.1f, 0, (110f / 60f) / attackSpeed, (110f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                break;
            case 6:
                AttackBase(6, 1f, 1.1f, 0, (100f / 60f) / attackSpeed, (100f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                break;
            case 7:
                AttackBase(7, 1f, 1.1f, 0, (105f / 60f) / attackSpeed, (105f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                break;
            case 8:
                AttackBase(8, 1.12f, 1.7f, 0, (130f / 60f) / attackSpeed, (130f / 60f) / attackSpeed + intervalPlus, 1, attackSpeed);
                break;
            case 9:
                AttackBase(9, 1.4f, 50f, 0, 270f / 60f, 270f / 60f + intervalPlus + 0.5f, 0, 1, true, weakProgress >= 2 ? 4.5f : weakProgress == 1 ? 3.5f : 2.5f);
                laserAttackedTimeRemain = 12f;
                randomDirectionUpdateRemain = 0f;
                attackDetection[attackLaserBody].multiHitInterval = 0.1f;
                SetNavMesh();
                break;
        }
    }

    void LaserMultiHitEnd() {
        if (state == State.Attack) {
            attackDetection[attackLaserBody].multiHitInterval = 0f;
        }
    }

    void EmitEffectBreakdown() {
        if (state != State.Dead) {
            EmitEffect(effectIndexBreakdown);
        }
    }

}
