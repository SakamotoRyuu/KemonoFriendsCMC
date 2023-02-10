using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mebiustos.MMD4MecanimFaciem;

public class FriendsBase : CharacterBase {

    [System.Serializable]
    public class Weapon {
        public int[] id;
        public bool isNegative;
        public int attackColliderIndex = -1;
        public GameObject prefab;
        public Transform pivot;
        public Vector3 angle;
        public bool equiped;
        public GameObject instance;
    }

    public int friendsId = -1;
    public int battleAnimatorId = -1;
    public int itemAnimatorId = -1;
    public string talkName;
    public bool drownTolerance;
    public bool fallingTolerance;
    public Transform drownHeightPivot;
    public Transform breathPivot;
    public bool existDynamicBone;
    public Cloth cloth;
    public CheckTriggerStay[] touchChecker;
    public Weapon[] weapon;

    [System.NonSerialized]
    public bool isRestored;
    [System.NonSerialized]
    public bool isMultiBuff;
    [System.NonSerialized]
    public bool checkWeaponPreservedFlag;

    public enum FieldBuffType { Heal, Stamina, Attack };
    public enum FaceName { Idle, Blink, Idle1, Idle2, Attack, Damage, Dead, Fear, Jump, Run, Refresh, Smile, Other }

    protected float maxHPLevel;
    protected float attackMultiLevel;
    protected float defenseMultiLevel;
    protected FaceName fixFaceName;
    protected float fixFaceTimeRemain;
    protected float eventMutekiTimeRemain;
    protected float superHealRate = 0.01f;
    protected float superHealHPBias = 1f;
    protected float superHealDecimal;
    protected float[] defaultMultiHitInterval;
    protected GameObject balloonSerifUI;
    protected BalloonSerifController bsCon;
    protected HeadLookController headLookController;
    protected Vector3 specialStepBoxCenterOffset;
    protected Vector3 specialStepBoxHalfExtents;
    protected LayerMask specialStepBoxLayerMask;
    protected int preMaxHP;
    protected float sParticleTimeOutRemain;
    protected float gutsMax;
    protected float gutsRemain;

    protected const int fieldBuffMax = 3;
    protected float[] fieldBuffRemainTime = new float[fieldBuffMax];
    protected float[] fieldBuffReserved = new float[fieldBuffMax];
    protected float fieldBuffContinuousHeal;
    protected GameObject fieldBuffEffectInstance;

    protected double mesTimeStamp;
    protected int mesPrioritySave;
    protected double mesInterval = 5.0;
    protected int mesDmgLtMin = 0;
    protected int mesDmgLtMax = 2;
    protected int mesDmgHvMin = 1;
    protected int mesDmgHvMax = 3;
    protected int mesAtkMin = 0;
    protected int mesAtkMax = 2;
    protected bool chatKeyInitialized;
    protected string chatKey_Appear;
    protected string chatKey_Win;
    protected string chatKey_Dead;
    protected string[] chatKey_Attack;
    protected string[] chatKey_Damage;
    protected int chatAttackCount = 3;
    protected int chatDamageCount = 4;
    protected string chatString;
    protected bool isRevive;
    protected float battleTime;
    protected float restTime;
    protected float escapeTrapTimeRemain;
    protected Vector3 escapeTrapDestination;

    protected DynamicBone[] dynamicBone;
    protected float headLookEffect;
    protected float headLookEffectTargetFigure;

    protected float drowningRemain;
    protected int drowningDamageRemain;
    protected Vector3 lastLandingPosition;
    protected bool drownToleranceNow;
    protected float drownSurfaceHeight;
    protected bool drownIsLava;
    protected bool drownIsFalling;
    protected float drownLavaDamageDecimal;
    protected float idlingTimeForFriendsEffect;
    protected float forceGroundedTimeRemain;
    protected float forceStopReservedTimeRemain;
    protected float releaseAttackReservedTimeRemain;
    protected bool isSacrificeOnDead;
    protected GameObject breathInstance;
    protected bool ignoreMegatonCoin;
    protected float separatedBattleTime;
    protected float separateFearRate = 2f;
    protected float disadvantageTimeRemain;
    protected float reviveTimer;

    protected const float staminaBorder = 0.8f;
    protected const float staminaCostRate = 0.5f;
    protected const string talkHeader = "TALK_";
    protected const float multiHitBuffInterval = 0.1f;
    protected const string animSIName_Idle1 = "Idle1";
    protected const string animSIName_Idle2 = "Idle2";
    protected const string animSIName_Refresh = "Refresh";

    protected GameObject[] buffEffectInstance;
    protected int currentFaceIndex = -1;
    protected int[] faceIndex = new int[0];
    protected bool faceIndexInitialized;
    protected MessageBackColor mesBackColor;
    protected FaciemBlink faciemBlink;

    protected float specialRotateElapsedTime;
    protected float specialRotateDuration;
    protected float specialRotateAngle;
    protected float specialRotateLastPoint;
    protected VoidEvent specialRotateCompleteEvent;
    protected MapChipControl mapChipControl;
    protected GameObject scaffoldEffectInstance;
    protected GameObject passiveSneezeEffectInstance;
    protected MovingFloor moveByFloorBody;
    protected float moveByFloorRotationSave;

    public override int Level {
        get {
            return level;
        }
        set {
            if (CharacterManager.Instance) {
                if (CharacterManager.Instance.levelLimit > 0) {
                    level = Mathf.Min(CharacterManager.Instance.levelLimit, value);
                } else if (CharacterManager.Instance.levelLimit < 0) {
                    level = Mathf.Min(GameManager.Instance.levelLimitAuto, value);
                } else {
                    level = value;
                }
            } else {
                level = value;
            }
        }
    }

    public int LevelForStatus {
        get {
            int levelTemp = Level - 1;
            if (levelTemp < 0) {
                levelTemp = 0;
            } else if (levelTemp >= GameManager.levelMax - 1) {
                levelTemp = GameManager.levelMax;
            }
            return levelTemp;
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (GameManager.Instance) {
            if (cloth) {
                SetClothEnabled(GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation]);
            }
            if (animatorForBattle) {
                headLookEffect = 0f;
                if (isPlayer) {
                    SetLastLandingPosition(trans.position);
                } else {
                    SetLastLandingPosition(CharacterManager.Instance.lastLandingPosition);
                }
            }
            if (breathPivot && StageManager.Instance && StageManager.Instance.dungeonController) {
                if (StageManager.Instance.dungeonController.isUnderwater) {
                    if (breathInstance == null) {
                        breathInstance = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.breathBubble], breathPivot);
                    }
                } else {
                    if (breathInstance != null) {
                        Destroy(breathInstance);
                    }
                }
            }
        }
    }

    private void OnDisable() {
        if (cloth) {
            cloth.enabled = false;
        }
    }

    protected override void Awake() {
        base.Awake();
        mesBackColor = GetComponent<MessageBackColor>();
        if (GameManager.Instance) {
            if (existDynamicBone) {
                dynamicBone = GetComponents<DynamicBone>();
                SetDynamicBoneEnabled(GameManager.Instance.save.config[GameManager.Save.configID_DynamicBone]);
            }
            if (cloth) {
                SetClothEnabled(GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation]);
            }
            headLookController = GetComponent<HeadLookController>();
            if (headLookController) {
                SetHeadLookControllerEnabled(GameManager.Instance.save.config[GameManager.Save.configID_FaceToEnemy]);
            }
        }
        if (animatorForBattle) {
            // SetFriendsStatus();
            consumeStamina = true;
            lightMutekiTime = lightStiffTime + 0.3f;
            heavyMutekiTime = heavyStiffTime + 0.3f;
            superHealHPBias = (maxHP > 100 ? maxHP * 0.01f : 1f);
            maxHPLevel = maxHP * 0.05f;
            attackMultiLevel = 0.1f;
            defenseMultiLevel = 0.1f;
            isAnimParamDetail = true;
            isAnimIdleEnabled = true;
            colorTypeDamage = damageColor_Friends;
            enableFriendsEffect = true;
            destroyOnDead = false;
            deadTimer = 5;
            reviveTimer = 3.5f;
            dodgeDistance = 4f;
            dodgeMutekiTime = 0.6f;
            dodgeDamageHealMax = dodgePower * 2f;
            attackingDodgeEnabled = true;
            attackedTimeRemain = -1f;
            knockRecovery = knockEndurance * 0.25f;
            lightKnockRecoveryRate = 0.5f;
            destinationUpdateInterval = 0.2f;
            fbStepConsiderRadius = true;
            damageReportEnabled = true;
            checkGroundedLayerMask = LayerMask.GetMask("Default", "Field", "EnemyCollision", "InvisibleWall", "SecondField", "ThirdField");
            defaultMultiHitInterval = new float[attackDetection.Length];
            for (int i = 0; i < defaultMultiHitInterval.Length; i++) {
                if (attackDetection[i]) {
                    defaultMultiHitInterval[i] = attackDetection[i].multiHitInterval;
                }
            }
            balloonSerifUI = Instantiate(CharacterDatabase.Instance.ui.balloonSerif, trans);
            balloonSerifUI.transform.localPosition = new Vector3(0f, cCon.height, 0f);
            bsCon = balloonSerifUI.GetComponentInChildren<BalloonSerifController>();
            bsCon.followTarget = trans;
            bsCon.offset = new Vector3(0f, cCon.height + 0.35f, 0f);
            bsCon.SetFrameColor(mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType);
            specialStepBoxCenterOffset = new Vector3(0f, cCon.center.y + 0.05f, 0f);
            specialStepBoxHalfExtents = new Vector3(0.08f, Mathf.Max((cCon.height - specialStepBoxCenterOffset.y) - 0.15f, 0.2f), 0.08f);
            specialStepBoxLayerMask = LayerMask.GetMask("E-DmgDetect");
            //Sandcat Center0.6 Height1.15
            //OLD SSCenter0.575 SSExtent0.425
            //NEW SSCenter0.65 SSExtent0.35
            ResetGuts();
            CheckStealth();
        }
    }

    protected override void Start() {
        base.Start();
        Level = GameManager.Instance.save.Level;
        SetNowHP(GetMaxHP());
        ChatKeyInit();
        if (playerTrans && trans != playerTrans) {
            trans.eulerAngles = new Vector3(0, playerTrans.eulerAngles.y, 0);
        }
        if (fCon) {
            SetFaceIndex();
            ResetFaciemBlink();
        }
        ChangeActionDistance(CharacterManager.Instance.isBossBattle);
    }

    /*
    void SetFriendsStatus() {
        if (friendsId >= 0) {
            nowHP = maxHP = CharacterDatabase.Instance.friends[friendsId].status.hp;
            nowST = maxST = CharacterDatabase.Instance.friends[friendsId].status.st;
            attackPower = CharacterDatabase.Instance.friends[friendsId].status.attack;
            defensePower = CharacterDatabase.Instance.friends[friendsId].status.defense;
            dodgeRemain = dodgePower = CharacterDatabase.Instance.friends[friendsId].status.dodge;
        }
    }
    */

    public void LevelUp(int newLevel) {
        int maxHPSave = GetMaxHP();
        Level = newLevel;
        int maxHPNew = GetMaxHP();
        if (maxHPNew > maxHPSave && nowHP > 0) {
            nowHP += maxHPNew - maxHPSave;
        }
        if (nowHP > maxHPNew) {
            nowHP = maxHPNew;
        }
    }

    public virtual void ResetGuts() {
        if (friendsId >= 0) {
            gutsRemain = gutsMax = 1f + CharacterDatabase.Instance.friends[friendsId].cost * 0.2f;
            dodgeRemain = dodgePower * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.FriendsDodge);
        }
        lightKnockCount = 0;
        heavyKnockCount = 0;
        specialMoveDuration = 0f;
        eventMutekiTimeRemain = 0f;
        battleTime = 0f;
        restTime = 0f;
        drowningRemain = 0f;
        drowningDamageRemain = 0;
        if (anim) {
            anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Drown], false);
            if (nowHP > 0) {
                anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Dead], false);
            }
        }
        RemoveRide();
    }

    /*
    public void ResetDodgeRemainForRaw() {
        if (dodgeRemain < dodgePower) {
            dodgeRemain = dodgePower;
        }
    }
    */

    public int GetGutsPercent() {
        if (gutsRemain <= 0f || gutsMax <= 0f) {
            return 0;
        } else if (gutsRemain >= gutsMax) {
            return 100;
        } else {
            return Mathf.Clamp(Mathf.RoundToInt(gutsRemain * 100f / gutsMax), 1, 99);
        }
    }

    protected override bool SetAnimatorDefaultParam() {
        if (anim) {
            if (battleAnimatorId < 0) {
                return base.SetAnimatorDefaultParam();
            } else {
                if (trans.parent == null) {
                    anim.runtimeAnimatorController = CharacterDatabase.Instance.GetAnimCon(battleAnimatorId);
                    return base.SetAnimatorDefaultParam();
                }
            }
        }
        return false;
    }

    public void SetForRevive() {
        isRevive = true;
    }

    protected virtual void SetFaceIndex() {
        if (fCon && !faceIndexInitialized) {
            faceIndex = new int[System.Enum.GetValues(typeof(FaceName)).Length];
            faceIndex[(int)FaceName.Idle] = fCon.GetFaceIndex(FaceName.Idle.ToString());
            faceIndex[(int)FaceName.Blink] = fCon.GetFaceIndex(FaceName.Blink.ToString());
            faceIndex[(int)FaceName.Idle1] = fCon.GetFaceIndex(FaceName.Idle1.ToString());
            faceIndex[(int)FaceName.Idle2] = fCon.GetFaceIndex(FaceName.Idle2.ToString());
            faceIndex[(int)FaceName.Attack] = fCon.GetFaceIndex(FaceName.Attack.ToString());
            faceIndex[(int)FaceName.Damage] = fCon.GetFaceIndex(FaceName.Damage.ToString());
            faceIndex[(int)FaceName.Dead] = fCon.GetFaceIndex(FaceName.Dead.ToString());
            faceIndex[(int)FaceName.Fear] = fCon.GetFaceIndex(FaceName.Fear.ToString());
            faceIndex[(int)FaceName.Jump] = fCon.GetFaceIndex(FaceName.Jump.ToString());
            faceIndex[(int)FaceName.Run] = fCon.GetFaceIndex(FaceName.Run.ToString());
            faceIndex[(int)FaceName.Refresh] = fCon.GetFaceIndex(FaceName.Refresh.ToString());
            faceIndex[(int)FaceName.Smile] = fCon.GetFaceIndex(FaceName.Smile.ToString());
            faceIndexInitialized = true;
        }
    }

    protected override void SetMapChip() {
        mapChipControl = Instantiate(MapDatabase.Instance.prefab[MapDatabase.friends], trans).GetComponent<MapChipControl>();
    }

    protected virtual void ChatKeyInit() {
        if (!chatKeyInitialized && !string.IsNullOrEmpty(talkName)) {
            string header = talkHeader + talkName;
            string headerAttack = header + "_ATTACK_";
            string headerDamage = header + "_DAMAGE_";
            chatKeyInitialized = true;
            chatKey_Appear = header + (isRevive ? "_REVIVE" : "_APPEAR");
            chatKey_Dead = header + "_DEAD";
            chatKey_Win = header + "_WIN";
            chatKey_Attack = new string[chatAttackCount];
            for (int i = 0; i < chatKey_Attack.Length; i++) {
                chatKey_Attack[i] = headerAttack + i.ToString("00");
            }
            chatKey_Damage = new string[chatDamageCount];
            for (int i = 0; i < chatKey_Damage.Length; i++) {
                chatKey_Damage[i] = headerDamage + i.ToString("00");
            }
        }
    }

    protected virtual void SetMultiHitBuff(bool flag) {
        if (flag) {
            for (int i = 0; i < attackDetection.Length; i++) {
                if (attackDetection[i] && !attackDetection[i].isProjectile && (attackDetection[i].multiHitInterval == 0 || attackDetection[i].multiHitInterval > multiHitBuffInterval)) {
                    attackDetection[i].multiHitInterval = multiHitBuffInterval;
                }
            }
        } else {
            for (int i = 0; i < attackDetection.Length; i++) {
                if (attackDetection[i] && attackDetection[i].multiHitInterval != defaultMultiHitInterval[i]) {
                    attackDetection[i].multiHitInterval = defaultMultiHitInterval[i];
                }
            }
        }
        isMultiBuff = flag;
    }

    protected override void Update_StatusJudge() {
        base.Update_StatusJudge();
        if (drowningRemain > 0f) {
            drowningRemain -= deltaTimeCache;
            if (drowningRemain <= 0f) {
                if (drowningDamageRemain > 0 && nowHP > 1) {
                    nowHP -= Mathf.Clamp(drowningDamageRemain, 0, nowHP - 1);
                }
                anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Drown], false);
                Vector2 circle = Random.insideUnitCircle * 0.5f;
                disableControlTimeRemain = 0.1f;
                Warp(lastLandingPosition + new Vector3(circle.x, 0f, circle.y), 0.1f, 0f);
                if (agent) {
                    agent.enabled = true;
                }
            }
        }
        if (GetIdling() && GetCanControl()) {
            idlingTimeForFriendsEffect += deltaTimeCache;
        } else {
            idlingTimeForFriendsEffect = 0f;
        }
        if (fieldBuffRemainTime[(int)FieldBuffType.Heal] > 0f) {
            fieldBuffContinuousHeal += deltaTimeCache;
            if (fieldBuffContinuousHeal >= 1f) {
                AddNowHP(Mathf.RoundToInt(GetMaxHP() * 0.015f), GetCenterPosition(), true, damageColor_Heal);
                CharacterManager.Instance.specialHealReported = 5;
                fieldBuffContinuousHeal -= 1f;
            }
        } else {
            fieldBuffContinuousHeal = 0f;
        }
        if (fieldBuffRemainTime[(int)FieldBuffType.Stamina] > 0f && deltaTimeCache > 0f) {
            float maxSTTemp = GetMaxST();
            nowST += maxSTTemp * (isPlayer ? 0.4f : 0.15f) * deltaTimeCache;
            if (nowST > maxSTTemp) {
                nowST = maxSTTemp;
            }
        }
        if (GameManager.Instance.minmiGolden) {
            float maxSTTemp = GetMaxST();
            nowST += maxSTTemp * 0.8f * deltaTimeCache;
            if (nowST > maxSTTemp) {
                nowST = maxSTTemp;
            }
        }
        if (nowHP > GetMaxHP()) {
            nowHP = GetMaxHP();
        }
        if (isSuperman) {
            if (attackedTimeRemain > 0f) {
                sParticleTimeOutRemain = 0.5f;
            } else if (sParticleTimeOutRemain > 0f) {
                sParticleTimeOutRemain -= deltaTimeCache;
                if (sParticleTimeOutRemain <= 0f) {
                    S_ParticleStopAll();
                }
            }
            if (IsDead) {
                SupermanEnd(false);
            }
        }
        if (mapChipControl) {
            bool toActive = (StageManager.Instance && StageManager.Instance.dungeonController && (StageManager.Instance.mapActivateFlag != 0 || (GameManager.Instance.save.config[GameManager.Save.configID_DisableAutoMapping] == 0 && StageManager.Instance.IsMappingFloor)));
            if (!isPlayer && toActive && StageManager.Instance.mapActivateFlag == 0 && IsSeparated()) {
                toActive = false;
            }
            if (mapChipControl.chipRenderer.enabled != toActive) {
                mapChipControl.chipRenderer.enabled = toActive;
            }
        }
        if (!isPlayer && rideTimeLimitEnabled && rideTarget && command == Command.Free && disableControlTimeRemain <= 0f) {
            RemoveRide(true);
        }
    }

    protected virtual bool StealthCondition() {
        bool answer = false;
        if (attackedTimeRemain <= -1 && CharacterManager.Instance) {
            /*
            bool isBossBattle = CharacterManager.Instance.isBossBattle;
            bool recentlyAttacked = (CharacterManager.Instance.GetElapsedTimeFromPlayerLastAttack() < 3f);
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Stealth)) {
                answer = true;
            } else if ((!isPlayer || !isBossBattle || !recentlyAttacked) && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.WaitStealth) != 0 && idlingTimeForFriendsEffect >= 0.5f) {
                answer = true;
            } else if ((!isPlayer || !isBossBattle || !recentlyAttacked) && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.JumpStealth) != 0 && state == State.Jump) {
                answer = true;
            }
            */
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Stealth)) {
                answer = true;
            }
        }
        return answer;
    }

    protected void CheckStealth() {
        if (StealthCondition()) {
            if (GetSearchTargetActive()) {
                SetSearchTargetActive(false);
            }
        } else {
            if (!GetSearchTargetActive()) {
                SetSearchTargetActive(true);
            }
        }
    }

    protected override void Update_Sick() {
        base.Update_Sick();
        if (state != State.Dead) {
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Antidote)) {
                if (GetSick(SickType.Poison)) {
                    SetSick(SickType.Poison, 0f);
                }
                if (GetSick(SickType.Acid)) {
                    SetSick(SickType.Acid, 0f);
                }
                if (GetSick(SickType.Slow)) {
                    SetSick(SickType.Slow, 0f);
                }
            }
            CheckStealth();
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Multi)) {
                if (!isMultiBuff) {
                    SetMultiHitBuff(true);
                }
            } else {
                if (isMultiBuff) {
                    SetMultiHitBuff(false);
                }
            }
        }
        bool passiveSneezeFlag = CharacterManager.Instance.IsPassiveSneeze;
        if (passiveSneezeFlag) {
            if (!passiveSneezeEffectInstance) {
                passiveSneezeEffectInstance = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.passiveSneeze], trans);
            }
        } else {
            if (passiveSneezeEffectInstance) {
                Destroy(passiveSneezeEffectInstance);
                passiveSneezeEffectInstance = null;
            }
        }
        CheckBuffEffect();
    }

    public override void SetSick(SickType sickType, float duration, AttackDetection attacker = null) {
        if (duration > 0f && eventMutekiTimeRemain > 0f) {
            return;
        }
        if (sickType == SickType.Fire && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Fire) != 0) {
            duration = 0f;
        }
        if (sickType == SickType.Mud && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Mud) != 0) {
            duration = 0f;
        }
        if (sickType == SickType.Ice && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Ice) != 0) {
            duration = 0f;
        }
        if ((sickType == SickType.Poison || sickType == SickType.Acid || sickType == SickType.Slow) && CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Antidote)) {
            duration = 0f;
        }
        base.SetSick(sickType, duration, attacker);
    }

    protected bool CheckWeapon() {
        bool changed = false;
        checkWeaponPreservedFlag = false;
        if (GameManager.Instance) {
            for (int i = 0; i < weapon.Length; i++) {
                bool flag = false;
                if (!weapon[i].isNegative) {
                    for (int j = 0; j < weapon[i].id.Length; j++) {
                        if (weapon[i].id[j] >= 0 && GameManager.Instance.save.GetEquip(weapon[i].id[j])) {
                            flag = true;
                            break;
                        }
                    }
                } else {
                    flag = true;
                    for (int j = 0; j < weapon[i].id.Length; j++) {
                        if (weapon[i].id[j] >= 0 && GameManager.Instance.save.GetEquip(weapon[i].id[j])) {
                            flag = false;
                            break;
                        }
                    }
                }
                if (!weapon[i].equiped && flag) {
                    changed = true;
                    weapon[i].equiped = true;
                    if (weapon[i].instance) {
                        Destroy(weapon[i].instance);
                        weapon[i].instance = null;
                    }
                    weapon[i].instance = Instantiate(weapon[i].prefab, weapon[i].pivot);
                    weapon[i].instance.transform.localPosition = vecZero;
                    weapon[i].instance.transform.localRotation = Quaternion.Euler(weapon[i].angle);
                    if (weapon[i].attackColliderIndex >= 0) {
                        attackDetection[weapon[i].attackColliderIndex] = weapon[i].instance.GetComponentInChildren<AttackDetection>();
                    }
                } else if (weapon[i].equiped && !flag) {
                    changed = true;
                    weapon[i].equiped = false;
                    if (weapon[i].instance) {
                        Destroy(weapon[i].instance);
                        weapon[i].instance = null;
                    }
                }
            }
        }
        return changed;
    }

    protected virtual void CheckBuffEffect() {
        if (EffectDatabase.Instance) {
            if (buffEffectInstance == null) {
                buffEffectInstance = new GameObject[CharacterManager.buffSlotMax];
            }
            for (int i = 0; i < CharacterManager.buffSlotMax; i++) {
                if (CharacterManager.Instance.GetBuff(i) && state != State.Dead) {
                    if (!buffEffectInstance[i]) {
                        buffEffectInstance[i] = Instantiate(CharacterManager.Instance.GetBuffEffect(i), CharacterManager.Instance.GetBuffIsCenter(i) && centerPivot ? centerPivot : trans);
                    }
                } else {
                    if (buffEffectInstance[i]) {
                        Destroy(buffEffectInstance[i]);
                    }
                }
            }
            bool fieldBuffEnabled = false;
            for (int i = 0; i < fieldBuffReserved.Length; i++) {
                if (fieldBuffReserved[i] > 0f) {
                    fieldBuffReserved[i] -= deltaTimeCache;
                    fieldBuffRemainTime[i] = Mathf.Clamp(fieldBuffRemainTime[i] + deltaTimeCache, 0.002f, 1f);
                } else {
                    fieldBuffRemainTime[i] = Mathf.Clamp(fieldBuffRemainTime[i] - deltaTimeCache * CharacterManager.Instance.riskyIncSqrt, 0f, 1f);
                }
                if (fieldBuffRemainTime[i] > 0f) {
                    fieldBuffEnabled = true;
                }
            }
            if (fieldBuffEnabled && !fieldBuffEffectInstance) {
                fieldBuffEffectInstance = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.fieldBuff], centerPivot ? centerPivot : trans);
            } else if (!fieldBuffEnabled && fieldBuffEffectInstance) {
                Destroy(fieldBuffEffectInstance);
                fieldBuffEffectInstance = null;
            }
        }
    }

    protected override void Update_Transition_Spawn() {
        if (stateTime > spawnStiffTime) {
            if (!isRestored) {
                AppearAction();
            } else {
                isRestored = false;
            }
            SetState(State.Wait);
        }
    }

    protected override void Start_Process_Wait() {
        base.Start_Process_Wait();
        stateTime = destinationUpdateInterval;
    }

    protected override void Rove() {
        if (command == Command.Free && disableControlTimeRemain <= 0f) {
            base.Rove();
        }
    }
    
    protected override void Update_Process_Wait() {
        base.Update_Process_Wait();
        if (agent && playerTrans && trans != playerTrans && stateTime >= destinationUpdateInterval && (command != Command.Free || escapeTrapTimeRemain > 0f)) {
            stateTime = 0;
            if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
                if (escapeTrapTimeRemain > 0f) {
                    destination = escapeTrapDestination;
                } else {
                    destination = playerTrans.position;
                }
                agent.SetDestination(destination);
            }
        }
    }

    protected override void Start_Process_Dead() {
        SetSpecialMove(knockDirection, heavyKnockMove, heavyKnockTime, EasingType.QuadOut);
        SetChat(chatKey_Dead, 50, 3f);
        CharacterManager.Instance.SetFriendsLost(friendsId, isSacrificeOnDead);
        if (attackerCB) {
            EnemyBase eBaseTemp = attackerCB.GetComponent<EnemyBase>();
            if (eBaseTemp) {
                CharacterManager.Instance.SetKillRecord(eBaseTemp);
            }
        }
        base.Start_Process_Dead();
    }

    protected override void Update_Process_Dead() {
        if (!isPlayer) {
            reviveTimer -= deltaTimeCache;
            if (reviveTimer <= 0f && GameManager.Instance.save.config[GameManager.Save.configID_FriendsAutomaticRevive] != 0 && PauseController.Instance && !PauseController.Instance.pauseGame && !PauseController.Instance.friendsDisabled) {
                PauseController.Instance.ReviveFriends(friendsId);
                reviveTimer = float.MaxValue;
            }
        }
        base.Update_Process_Dead();
    }

    protected override void DeadProcess() {
        CharacterManager.Instance.SelfErase(gameObject);
    }

    protected override void Update_MoveControl_ChildAgentSpeed() {
        float speedTemp = (canRun && !(command == Command.Free && target == null) ? GetMaxSpeed(false) : GetMaxSpeed(true));
        if (agent.speed != speedTemp) {
            agent.speed = speedTemp;
        }
    }

    protected virtual void ReleaseAttackDetectionsForce() {
        for (int i = 0; i < attackDetection.Length; i++) {
            if (attackDetection[i]) {
                attackDetection[i].DetectionEnd(false);
            }
        }
        anyAttackEnabled = false;
    }

    protected override void Update_TimeCount() {
        base.Update_TimeCount();
        if (isSuperman) {
            superHealDecimal += CharacterManager.Instance.GetPlayerMaxHP() * CharacterManager.Instance.GetSuperHealHyper() * CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.WRHeal) * superHealHPBias * superHealRate * deltaTimeCache;
            if (superHealDecimal >= 1) {
                AddNowHP((int)superHealDecimal, GetCenterPosition(), false);
                superHealDecimal -= (int)superHealDecimal;
            }
        }
        if (forceGroundedTimeRemain > 0f) {
            forceGroundedTimeRemain -= deltaTimeCache;
        }
        if (forceStopReservedTimeRemain > 0f) {
            forceStopReservedTimeRemain -= deltaTimeCache;
            if (forceStopReservedTimeRemain <= 0f) {
                ReleaseAttackDetectionsForce();
                if (throwingEnabled && throwing) {
                    throwing.ThrowCancelAll(true);
                }
            }
        }
        if (eventMutekiTimeRemain > 0f) {
            eventMutekiTimeRemain -= deltaTimeCache;
        }
        if (escapeTrapTimeRemain > 0f) {
            escapeTrapTimeRemain -= deltaTimeCache;
        }
        if (disadvantageTimeRemain > 0f) {
            disadvantageTimeRemain -= deltaTimeCache;
        }
        if (mutekiTimeRemain > 0f && GameManager.Instance.save.config[GameManager.Save.configID_DisableInvincibility] != 0) {
            mutekiTimeRemain = 0f;
        }
        if (CharacterManager.Instance.GetFriendsSeparated(friendsId)) {
            if (state != State.Wait) {
                separatedBattleTime += deltaTimeCache * separateFearRate;
            } else {
                separatedBattleTime -= deltaTimeCache;
            }
            separatedBattleTime = Mathf.Clamp(separatedBattleTime, 0f, 12f);
            if (separatedBattleTime >= 12f) {
                forceIgnoreFlag = true;
            } else if (separatedBattleTime <= 0f) {
                forceIgnoreFlag = false;
            }
        } else {
            separatedBattleTime = 0f;
            forceIgnoreFlag = false;
        }
        if (StageManager.Instance.IsHomeStage) {
            if (command == Command.Free) {
                if (target) {
                    battleTime += deltaTimeCache;
                    if (battleTime >= 30f) {
                        battleTime = 30f;
                        restTime = 20f;
                    }
                } else {
                    battleTime -= deltaTimeCache * 5f;
                    if (battleTime < 0f) {
                        battleTime = 0f;
                    }
                }
                if (restTime > 0f) {
                    restTime -= deltaTimeCache;
                }
            } else {
                battleTime = 0f;
                restTime = 0f;
            }
            if (searchArea.Length > 0 && searchArea[0] && searchArea[0].enabled != (restTime <= 0f)) {
                searchArea[0].enabled = (restTime <= 0f);
                if (searchArea[0].enabled == false) {
                    target = null;
                    targetTrans = null;
                    targetRadius = 0f;
                }
            }
        }
    }

    protected virtual void SetFace(int faceIndex) {
        fCon.SetFace(faceIndex, true);
    }

    protected override void Update_FaceControl() {
        base.Update_FaceControl();
        if (animParam.deadSpecial && state == State.Dead && stateTime > 2f) {
            fixFaceName = FaceName.Refresh;
            fixFaceTimeRemain = 0.2f;
        }
        AnimatorStateInfo animSI = anim.GetCurrentAnimatorStateInfo(0);
        if (fCon && faceIndex.Length > 0) {
            currentFaceIndex = fCon.CurrentFaceIndex;
            int hash = animSI.fullPathHash;
            if (hash == AnimHash.Instance.ID[(int)AnimHash.ParamName.StateFriendsIdle1]) {
                if (animSI.normalizedTime < 0.81f) {
                    if (currentFaceIndex != faceIndex[(int)FaceName.Idle1] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                        SetFace(faceIndex[(int)FaceName.Idle1]);
                    }
                } else if (currentFaceIndex != faceIndex[(int)FaceName.Idle] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                    SetFace(faceIndex[(int)FaceName.Idle]);
                }
            } else if (hash == AnimHash.Instance.ID[(int)AnimHash.ParamName.StateFriendsIdle2]) {
                if (animSI.normalizedTime < 1f) {
                    if (currentFaceIndex != faceIndex[(int)FaceName.Idle2] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                        SetFace(faceIndex[(int)FaceName.Idle2]);
                    }
                } else if (currentFaceIndex != faceIndex[(int)FaceName.Idle] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                    SetFace(faceIndex[(int)FaceName.Idle]);
                }
            } else if (hash == AnimHash.Instance.ID[(int)AnimHash.ParamName.StateFriendsRefresh]) {
                if (currentFaceIndex != faceIndex[(int)FaceName.Refresh] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                    SetFace(faceIndex[(int)FaceName.Refresh]);
                }
            } else if ((state == State.Damage || (!drownToleranceNow && drowningRemain > 0))) {
                if (currentFaceIndex != faceIndex[(int)FaceName.Damage]) {
                    SetFace(faceIndex[(int)FaceName.Damage]);
                }
            } else if (fixFaceTimeRemain > 0f) {
                if (currentFaceIndex != faceIndex[(int)fixFaceName]) {
                    SetFace(faceIndex[(int)fixFaceName]);
                }
            } else if (state == State.Dead) {
                if (currentFaceIndex != faceIndex[(int)FaceName.Dead]) {
                    SetFace(faceIndex[(int)FaceName.Dead]);
                }
            } else if (state == State.Attack) {
                if (currentFaceIndex != faceIndex[(int)FaceName.Attack]) {
                    SetFace(faceIndex[(int)FaceName.Attack]);
                }
            } else if ((state == State.Jump || state == State.Dodge || state == State.Climb) && !rideTarget) {
                if (currentFaceIndex != faceIndex[(int)FaceName.Jump] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                    SetFace(faceIndex[(int)FaceName.Jump]);
                }
            } else if (nowSpeed > GetMaxSpeed(true)) {
                if (currentFaceIndex != faceIndex[(int)FaceName.Run] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                    SetFace(faceIndex[(int)FaceName.Run]);
                }
            } else if (disadvantageTimeRemain > 0f || GetAnySick()) {
                if (currentFaceIndex != faceIndex[(int)FaceName.Refresh] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                    SetFace(faceIndex[(int)FaceName.Refresh]);
                }
            } else {
                if (currentFaceIndex != faceIndex[(int)FaceName.Idle] && currentFaceIndex != faceIndex[(int)FaceName.Blink]) {
                    SetFace(faceIndex[(int)FaceName.Idle]);
                }
            }
        }
        if (fixFaceTimeRemain > 0f) {
            fixFaceTimeRemain -= deltaTimeCache;
        }
    }

    protected virtual void STConsumeChild_Drown() {
        if (drowningRemain > 0f && !drownToleranceNow && eventMutekiTimeRemain <= 0f) {
            nowST -= Mathf.Clamp(GetMaxST(), 200f, 500f) * deltaTimeCache;
            if (drownIsLava) {
                drownLavaDamageDecimal += StageManager.Instance.GetSlipDamage(false) * (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Gravity) != 0 ? 1 : 2) * 5.07f * deltaTimeCache;
                if (drownLavaDamageDecimal >= 1f) {
                    int damageTemp = (int)drownLavaDamageDecimal;
                    if (damageTemp > drowningDamageRemain) {
                        damageTemp = drowningDamageRemain;
                    }
                    drowningDamageRemain -= damageTemp;
                    if (damageTemp > 0 && nowHP > 1) {
                        nowHP -= Mathf.Clamp(damageTemp, 0, nowHP - 1);
                    }
                    drownLavaDamageDecimal -= Mathf.Floor(drownLavaDamageDecimal);
                }
            }
        }
    }

    protected override void STControlChild_STConsume() {
        STConsumeChild_Drown();
        base.STControlChild_STConsume();
    }

    protected override void MoveControlChild_Gravity() {
        if (drowningRemain > 0f && !drownIsFalling) {
            if (drownHeightPivot) {
                if (drownHeightPivot.position.y > drownSurfaceHeight) {
                    move.y = Mathf.Clamp((drownHeightPivot.position.y - drownSurfaceHeight) * -9.81f - 3f, Mathf.Clamp(move.y, -9.81f, -1f), 0f);
                } else if (drownHeightPivot.position.y < drownSurfaceHeight - 0.01f) {
                    move.y = Mathf.Clamp((drownSurfaceHeight - drownHeightPivot.position.y) * 5f + 0.2f, 0.2f, 1f);
                } else {
                    move.y = 0f;
                }
            }
        } else if (gravityZeroTimeRemain > 0f) {
            move.y = 0f;
        } else {
            base.MoveControlChild_Gravity();
        }
    }

    protected override void Update_MoveControl() {
        Update_SpecialRotate();
        if (moveByFloorBody) {
            Update_MoveByFloor();
        }
        base.Update_MoveControl();
    }    

    protected override float GetDodgeHealRate() {
        return 0f;
    }

    protected override void DodgePowerDamageHeal(int damage) {
        if (dodgeRemain < dodgeDamageHealMax) {
            /*
            dodgeRemain += dodgePower * damage / Mathf.Max(GetMaxHPNoEffected(), 1f);
            */
            //Test
            dodgeRemain += dodgePower * damage * CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.FriendsDodge) / Mathf.Max(GetMaxHPNoEffected(), 1f);
            if (dodgeRemain > dodgeDamageHealMax) {
                dodgeRemain = dodgeDamageHealMax;
            }
        }
    }

    protected virtual void ShowGuts() {
        CharacterManager.Instance.ShowGuts(trans.position + cCon.center + vecDown * 1f, gutsRemain <= 0f);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        if (eventMutekiTimeRemain <= 0f) {
            int nowHPTemp = nowHP;
            if (attacker && damage > 0) {
                float damageTemp = damage;
                if (attacker.isBoss) {
                    damageTemp *= CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.DamageBoss, attacker.forceDefaultDifficulty);
                } else {
                    damageTemp *= CharacterManager.Instance.GetDifficultyEffect(CharacterManager.DifficultyEffect.DamageZako, attacker.forceDefaultDifficulty);
                }
                if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Defense)) {
                    damageTemp *= 0.5f;
                }
                damage = Mathf.Max(1, Mathf.RoundToInt(damageTemp));
            }
            if (gutsRemain > 0f && !CharacterManager.Instance.hellMode && GameManager.Instance.save.config[GameManager.Save.configID_DisableGuts] == 0) {
                if (damage >= nowHPTemp && nowHPTemp >= CharacterManager.Instance.GetGutsBorder()) {
                    int newDamage = nowHP - 1;
                    if (gutsMax < 1000f) {
                        gutsRemain -= (float)(damage - newDamage) / Mathf.Max(CharacterManager.Instance.GetPlayerMaxHPNoEffected(), 1);
                    }
                    ShowGuts();
                    damage = newDamage;
                }
            }
            base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        }
    }

    protected virtual void HeadLookEffectUpdate(float speed = 2f) {
        if (headLookEffect < headLookEffectTargetFigure) {
            headLookEffect += Time.deltaTime * speed;
            if (headLookEffect > headLookEffectTargetFigure) {
                headLookEffect = headLookEffectTargetFigure;
            }
        } else if (headLookEffect > headLookEffectTargetFigure) {
            headLookEffect -= Time.deltaTime * speed;
            if (headLookEffect < headLookEffectTargetFigure) {
                headLookEffect = headLookEffectTargetFigure;
            }
        }
        headLookController.effect = headLookEffect;
    }

    protected virtual void HeadLookUpdate() {
        if (headLookController) {
            if (targetTrans && GetCanControl()) {
                if (animParam.refresh) {
                    headLookEffectTargetFigure = 0.5f;
                } else {
                    headLookEffectTargetFigure = 1f;
                }
                headLookController.target = targetTrans.position;
            } else {
                headLookEffectTargetFigure = 0f;
            }
            HeadLookEffectUpdate();
        }
    }

    protected virtual void LateUpdate() {
        HeadLookUpdate();
    }

    public override bool CheckGrounded(float tolerance = 0.5f) {
        if (forceGroundedTimeRemain > 0f || rideTarget) {
            return true;
        } else if (drowningRemain > 0f) {
            return false;
        } else {
            return base.CheckGrounded(tolerance);
        }
    }

    public override void SetForItem() {
        if (headLookController) {
            headLookController.effect = 0;
            headLookController.enabled = false;
        }
        AgentLinkMover agentLinkMover = GetComponent<AgentLinkMover>();
        if (agentLinkMover) {
            agentLinkMover.enabled = false;
        }
        base.SetForItem();
    }

    public void SetForDictionary() {
        SetForItem();
        if (cloth) {
            SetClothEnabled(GameManager.Instance.save.config[GameManager.Save.configID_ClothSimulation]);
        }
        if (battleAnimatorId >= 0) {
            if (anim == null) {
                anim = GetComponent<Animator>();
            }
            if (anim) {
                anim.runtimeAnimatorController = CharacterDatabase.Instance.GetAnimCon(battleAnimatorId);
            }
        }
    }

    public void SetFaceSmile() {
        fixFaceName = FaceName.Smile;
        fixFaceTimeRemain = 5f;
    }

    public void SetFaceFear() {
        fixFaceName = FaceName.Fear;
        fixFaceTimeRemain = 5f;
    }

    public void SetFaceSpecial(FaceName newFace, float timer = 5f) {
        fixFaceName = newFace;
        fixFaceTimeRemain = timer;
    }

    public void SetFaceString(string newFace, float timer = 5f, bool soonNow = false) {
        if (!faceIndexInitialized) {
            SetFaceIndex();
        }
        int index = fCon.GetFaceIndex(newFace);
        if (index >= 0) {
            faceIndex[(int)FaceName.Other] = index;
            fixFaceName = FaceName.Other;
            fixFaceTimeRemain = timer;
            if (soonNow) {
                SetFace(faceIndex[(int)fixFaceName]);
            }
        }
    }

    public void SetDrowning(float surfaceHeight = 0f, bool isLava = false, bool isFalling = false) {
        if (state != State.Dead) {
            if (agent) {
                agent.enabled = false;
            }
            if (state == State.Attack) {
                SetState(State.Wait);
            }
            ResetTriggerOnDamage();
            if (move.y < -10f) {
                move.y = -10f;
            }
            drownIsLava = isLava;
            drownIsFalling = isFalling;
            drownLavaDamageDecimal = 0f;
            drowningRemain = 1.6f;
            mutekiTimeRemain = 1.6f;
            disableControlTimeRemain = 2f;
            gravityZeroTimeRemain = isFalling ? 0f : 2f;
            forceStopReservedTimeRemain = 0.5f;
            drownSurfaceHeight = surfaceHeight;
            drownToleranceNow = GetDrownTolerance(isLava, isFalling);
            if (drownIsLava && !drownToleranceNow && eventMutekiTimeRemain <= 0f) {
                drowningDamageRemain = StageManager.Instance.GetSlipDamage(false) * 8 * (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Gravity) != 0 ? 1 : 2);
            } else {
                drowningDamageRemain = 0;
            }
            anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.DrownTolerance], drownToleranceNow);
            anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.Drown], true);
        }
    }

    public bool GetDrowning() {
        return drowningRemain > 0f;
    }

    public void SetLastLandingPosition(Vector3 position) {
        lastLandingPosition = position;
        if (isPlayer) {
            CharacterManager.Instance.lastLandingPosition = lastLandingPosition;
        }
    }

    protected override void KnockLightProcess() {
        base.KnockLightProcess();
        if (chatKeyInitialized && GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] < 3) {
            int index = Random.Range(mesDmgLtMin, mesDmgLtMax + 1);
            if (index >= 0 && index < chatKey_Damage.Length) {
                SetChat(chatKey_Damage[index], 20);
            }
        }
    }

    protected override void KnockHeavyProcess() {
        base.KnockHeavyProcess();
        if (chatKeyInitialized && GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] < 3) {
            int index = Random.Range(mesDmgHvMin, mesDmgHvMax + 1);
            if (index >= 0 && index < chatKey_Damage.Length) {
                SetChat(chatKey_Damage[index], 30);
            }
        }
    }

    protected override void Attack() {
        base.Attack();
        if (chatKeyInitialized && GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] < 3) {
            int index = Random.Range(mesAtkMin, mesAtkMax + 1);
            if (index >= 0 && index < chatKey_Damage.Length) {
                SetChat(chatKey_Attack[index], 10);
            }
        }
    }

    public virtual void AppearAction() {
        SetChat(chatKey_Appear, 15);
    }

    public override void SpecialStep(float distanceToTarget, float stepTime = 0.25f, float maxDist = 4, float minDist = 0, float defaultDist = 0, bool ignoreY = true, bool considerTargetRadius = true, EasingType easingType = EasingType.SineInOut, bool directionAdjustEnabled = false) {
        float dist = defaultDist;
        Vector3 targetPos;
        if (targetTrans) {
            targetPos = targetTrans.position;
            if (ignoreY) {
                targetPos.y = trans.position.y;
            }
            if (considerTargetRadius && Physics.BoxCast(trans.position + specialStepBoxCenterOffset, specialStepBoxHalfExtents, (targetPos - trans.position).normalized, out raycastHit, Quaternion.Euler(trans.TransformDirection(vecForward)), maxDist + distanceToTarget, specialStepBoxLayerMask, QueryTriggerInteraction.Ignore)) {
                dist = Mathf.Clamp(raycastHit.distance - distanceToTarget, minDist, maxDist);
            } else {
                dist = Mathf.Clamp(Vector3.Distance(targetPos, trans.position) - distanceToTarget, minDist, maxDist);
            }
            if (considerTargetRadius && touchChecker.Length > 0) {
                for (int i = 0; i < touchChecker.Length; i++) {
                    if (touchChecker[i] && touchChecker[i].stayFlag) {
                        dist = minDist;
                        break;
                    }
                }
            }
        } else {
            targetPos = trans.position + trans.TransformDirection(vecForward);
        }
        if (dist > 0) {
            SetSpecialMove((targetPos - trans.position).normalized, dist, stepTime, easingType, null, directionAdjustEnabled);
        }
    }

    public override void ForceStopForEvent(float time) {
        base.ForceStopForEvent(time);
        forceGroundedTimeRemain = time;
        eventMutekiTimeRemain = time;
        forceStopReservedTimeRemain = 0.5f;
    }

    public override void ReleaseStopForEvent() {
        base.ReleaseStopForEvent();
        forceGroundedTimeRemain = 0f;
        eventMutekiTimeRemain = 0f;
        forceStopReservedTimeRemain = 0f;
    }

    public virtual void WinAction(bool gravityZero = false, float forceGroundedTimePlus = 0f) {
        if (state != State.Dead) {
            ForceStopForEvent(80f / 30f);
            SetFaceSmile();
            if (gravityZero) {
                gravityZeroTimeRemain = 80f / 30f;
            }
            if (forceGroundedTimePlus != 0f) {
                forceGroundedTimeRemain += forceGroundedTimePlus;
            }
            anim.SetTrigger(AnimHash.Instance.ID[(int)AnimHash.ParamName.Smile]);
            SetChat(chatKey_Win, 40, 5f, false);
        }
    }

    public void SetEventMutekiTime(float time) {
        eventMutekiTimeRemain = time;
    }

    protected void SetSpecialRotate(float angle, float duration, VoidEvent completeEvent = null) {
        specialRotateAngle = angle;
        specialRotateDuration = duration;
        specialRotateElapsedTime = 0f;
        specialRotateCompleteEvent = completeEvent;
        specialRotateLastPoint = 0f;
    }

    protected void Update_SpecialRotate() {
        if (specialRotateElapsedTime < specialRotateDuration && specialRotateDuration > 0f && deltaTimeMove > 0f) {
            specialRotateElapsedTime += deltaTimeMove;
            float nowPoint = Mathf.Clamp01(specialRotateElapsedTime / specialRotateDuration);
            trans.Rotate(vecUp, specialRotateAngle * (nowPoint - specialRotateLastPoint), Space.World);
            specialRotateLastPoint = nowPoint;
            if (specialRotateElapsedTime >= specialRotateDuration && specialRotateCompleteEvent != null) {
                specialRotateCompleteEvent.Invoke();
                specialRotateCompleteEvent = null;
            }
        }
    }

    protected void Update_MoveByFloor() {
        if (!rideTarget && moveByFloorBody && moveByFloorBody.floorRigidbody) {
            if (groundedFlag && state != State.Jump) {
                Vector3 moveVec = moveByFloorBody.floorRigidbody.GetPointVelocity(trans.position) * deltaTimeCache;
                if (moveByFloorBody.heightOffset != 0f) {
                    moveVec.y += moveByFloorBody.heightOffset * deltaTimeCache;
                }
                if (moveByFloorBody.centripetalOffset != 0f && moveByFloorBody.centerPivot) {
                    Vector3 posDiff = (moveByFloorBody.centerPivot.position - transform.position);
                    if (moveByFloorBody.centripetalIgnoreY) {
                        posDiff.y = 0f;
                    }
                    if (posDiff.sqrMagnitude > 0.0001f) {
                        moveVec += moveByFloorBody.centripetalOffset * deltaTimeCache * posDiff.normalized;
                    }
                }
                transform.position += moveVec;
            }
            if (moveByFloorBody.rotationEnabled) {
                float nowRot = moveByFloorBody.floorRigidbody.rotation.eulerAngles.y;
                if (groundedFlag && state != State.Jump) {
                    float difference = (nowRot - moveByFloorRotationSave);
                    if (difference != 0f) {
                        if (difference > 180f) {
                            difference -= 360f;
                        } else if (difference < -180f) {
                            difference += 360f;
                        }
                        transform.eulerAngles += vecUp * difference;
                    }
                }
                moveByFloorRotationSave = nowRot;
            }
        }
    }

    protected virtual void SetChat(string key, int priority = 1, float displayTime = 2f, bool lengthBias = true) {
        double intervalTemp = mesInterval;
        if (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] > 0 && CharacterManager.Instance) {
            intervalTemp = mesInterval * (1.0 + Mathf.Clamp(CharacterManager.Instance.friendsCountSave - 6, 0, 25) / 25.0) * (GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] == 1 ? 1.0 : 2.0);
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_Chat] != 0 && chatKeyInitialized && (GameManager.Instance.time >= mesTimeStamp + intervalTemp || priority > mesPrioritySave)) {
            chatString = TextManager.Get(key);
            if (!string.IsNullOrEmpty(chatString)) {
                if (lengthBias) {
                    displayTime += Mathf.Min(2.5f, chatString.Length * 0.03f * GameManager.Instance.save.charWeight);
                }
                mesPrioritySave = priority;
                mesTimeStamp = GameManager.Instance.time;
                bsCon.SetText(chatString, displayTime, true);
            }
        }
    }

    public virtual void SetFieldBuff(FieldBuffType type) {
        fieldBuffReserved[(int)type] = 0.05f;
    }

    public override void BlownAway(bool disableControl = true, float height = 5, bool nearGoal = false, string targetTag = "Respawn") {
        base.BlownAway(disableControl, height, nearGoal, targetTag);
        if (isPlayer) {
            CharacterManager.Instance.CheckFriendsSeparatedAll();
        } else {
            CharacterManager.Instance.CheckFriendsSeparated(friendsId);
        }
    }

    public virtual void ChangeActionDistance(bool isBossBattle) { }

    public override float GetAttack(bool ignoreMultiplier = false) {
        return base.GetAttack(ignoreMultiplier) * GameManager.Instance.GetLevelStatus(LevelForStatus) * (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Attack)) * (1f + CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Attack)) * (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Attack) ? 1.5f : 1) * (fieldBuffRemainTime[(int)FieldBuffType.Attack] > 0f ? 1.5f : 1f);
    }
    public override float GetAttackNoEffected() {
        return base.GetAttackNoEffected() * GameManager.Instance.GetLevelStatus(LevelForStatus) * (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Attack));
    }
    public override float GetDefense(bool ignoreMultiplier = false) {
        return base.GetDefense(ignoreMultiplier) * GameManager.Instance.GetLevelStatus(LevelForStatus) * (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Defense)) * (1f + CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Defense));
    }
    public override float GetDefenseNoEffected() {
        return base.GetDefenseNoEffected() * GameManager.Instance.GetLevelStatus(LevelForStatus) * (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Defense));
    }
    public override float GetKnock(bool ignoreMultiplier = false) {
        return base.GetKnock(ignoreMultiplier) * (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Knock)) * (1f + CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Knock)) * (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Knock) ? 2f : 1f);
    }
    public override float GetKnocked() {
        return base.GetKnocked() * (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Knock) ? 3f : 1f);
    }
    public override float GetLightKnockEndurance() {
        return base.GetLightKnockEndurance() + (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.LightKnockEndurance));
    }
    public override float GetHeavyKnockEndurance() {
        return base.GetHeavyKnockEndurance() + (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.LightKnockEndurance));
    }
    public override int GetMaxHP() {
        return CharacterManager.Instance.hellMode ? 1 : Mathf.RoundToInt(GetMaxHPNoEffected() * (1f + CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.MaxHP)));
    }
    public override int GetMaxHPNoEffected() {
        return CharacterManager.Instance.hellMode ? 1 : maxHP + Mathf.RoundToInt(maxHPLevel * LevelForStatus) + CharacterManager.Instance.GetMaxHPPlus() * maxHP / 10;
    }
    public override float GetMaxST() {
        return GetMaxSTNoEffected() * (1f + CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Stamina)) * (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Stamina) ? 3 : 1);
    }
    public override float GetMaxSTNoEffected() {
        return maxST + CharacterManager.Instance.GetMaxSTPlus() * maxST / 10;
    }
    protected override float GetSTHealRateChild_Normal() {
        return base.GetSTHealRateChild_Normal() * (idlingTimeForFriendsEffect >= 0.25f && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.IdlingSTHeal) != 0 ? 1.25f : 1f);
    }
    protected override float GetCost(CostType type) {
        float answer = base.GetCost(type);
        if (type == CostType.Attack || type == CostType.Skill) {
            answer *= (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.CostAttack));
        } else if (type == CostType.Jump) {
            // answer *= (1f +CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.CostJump));
        } else {
            answer *= (1f + CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.CostMove));
        }
        return answer;
    }

    public override float GetMaxSpeed(bool isWalk = false, bool ignoreSuperman = false, bool ignoreSick = false, bool ignoreConfig = false) {
        return
            (isWalk ? walkSpeed : maxSpeed)
            * (isSuperman && !ignoreSuperman ? superSpeedRate : 1f)
            * Mathf.Min(ignoreSick ? 1f : GetSickSpeedRate(), ignoreConfig ? 1f : Mathf.Clamp01(1f + (isWalk ? GameManager.Instance.save.config[GameManager.Save.configID_FriendsWalkingSpeed] : GameManager.Instance.save.config[GameManager.Save.configID_FriendsRunningSpeed]) * 0.01f))
            * (isWalk ? 1f : 1f + (groundedFlag ? CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Speed) : CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.SpeedInAir)) + CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Speed))
            * (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Speed) ? 2 : 1)
            * (!ignoreMegatonCoin && GameManager.Instance.megatonCoin ? GameManager.Instance.megatonCoinSpeedMul : 1f);
    }
    public override float GetAcceleration() {
        if (isPlayer) {
            return base.GetAcceleration();
        } else {
            return base.GetAcceleration() * (1f + (groundedFlag ? CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Speed) : CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.SpeedInAir)) + CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Speed)) * (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Speed) ? 2 : 1);
        }
    }
    public bool GetDrownTolerance(bool isLava = false, bool isFalling = false) {
        if (GameManager.Instance.minmiBlue) {
            return false;
        } else {
            if (isFalling) {
                return fallingTolerance;
            } else {
                return (!isLava && drownTolerance) || (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.DrownTolerance) != 0);
            }
        }
    }
    public bool SetClothEnabled(int param) {
        if (cloth) {
            bool flag = (isPlayer ? param >= 1 : param >= 2);
            cloth.enabled = flag;
            return true;
        }
        return false;
    }
    public void SetDynamicBoneEnabled(int param) {
        bool flag = (isPlayer ? param >= 1 : param >= 2);
        if (existDynamicBone) {
            for (int i = 0; i < dynamicBone.Length; i++) {
                dynamicBone[i].enabled = flag;
            }
        }
    }
    public void SetHeadLookControllerEnabled(int param) {
        if (headLookController) {
            bool flag = (isPlayer ? param >= 1 : param >= 2) && !isItem;
            headLookController.enabled = flag;
        }
    }
    public override void SupermanStart(bool effectEnable = true) {
        if (!isSuperman) {
            superHealDecimal = 0f;
            if (effectEnable) {
                if (isEnemy || isPlayer || GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] < 3) {
                    CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.friendsYKStart, friendsId, true, true, friendsId >= 0);
                    GameObject duringObj = CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.friendsYKDuring, friendsId, true, false, friendsId >= 0);
                    if (duringObj && friendsId >= 0) {
                        ActivateAndPositionToRaycastHit raycasterTemp = duringObj.GetComponent<ActivateAndPositionToRaycastHit>();
                        if (raycasterTemp) {
                            raycasterTemp.offset = new Vector3(0f, 0.046f + friendsId * 0.0001f, 0f);
                        }
                    }
                }
            }
        }
        base.SupermanStart();
    }
    public override void SupermanEnd(bool effectEnable = true) {
        if (isSuperman && effectEnable) {
            if (isEnemy || isPlayer || GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] < 3) {
                CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.friendsYKEnd, friendsId, true, true, false);
            }
        }
        base.SupermanEnd();
    }

    public virtual void HealForJustDodge(float amount) {
        /*
        float maxSTTemp = GetMaxST();
        dodgeRemain += (1f + dodgePower * 0.05f) * amount;
        if (dodgeRemain > dodgePower) {
            dodgeRemain = dodgePower;
        }
        */
        //Test
        float healTemp = (1f + dodgePower * 0.05f) * amount;
        if (dodgeRemain >= dodgePower) {
            dodgeRemain += healTemp * 0.5f;
        } else if (dodgeRemain + healTemp > dodgePower) {
            healTemp -= dodgePower - dodgeRemain;
            dodgeRemain = dodgePower;
            dodgeRemain += healTemp * 0.5f;
        } else {
            dodgeRemain += healTemp;
        }
        if (dodgeRemain > dodgeDamageHealMax) {
            dodgeRemain = dodgeDamageHealMax;
        }
    }

    public MessageBackColor GetMessageBackColor() {
        return mesBackColor;
    }

    public override void EscapeFromTrap(Vector3 position, SickType sickType, float radius) {
        if (agent && state == State.Wait && nowSpeed == 0f && escapeTrapTimeRemain <= 0f) {
            if (sickType == SickType.Fire && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Fire) != 0) {
                return;
            }
            if (sickType == SickType.Mud && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Mud) != 0) {
                return;
            }
            if (sickType == SickType.Ice && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Ice) != 0) {
                return;
            }
            escapeTrapTimeRemain = destinationUpdateInterval * 1.5f;
            escapeTrapDestination = GetEscapeDestination(position, 4f + radius);
        }
    }

    protected override bool IsSeparated() {
        if (friendsId >= 0) {
            return CharacterManager.Instance.GetFriendsSeparated(friendsId);
        }
        return false;
    }

    public override void SetMutekiTime(float num) {
        if (GameManager.Instance.save.config[GameManager.Save.configID_DisableInvincibility] == 0) {
            if (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Muteki) != 0) {
                num *= 1.2f;
            }
            base.SetMutekiTime(num);
        }
    }

    public override float GetDodgeRemain() {
        float remainTemp = dodgeRemain;
        if (CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.Dodge) != 0) {
            remainTemp += 0.5f;
        }
        return remainTemp;
    }

    public override bool DodgeChallenge() {
        if (CharacterManager.Instance.DefinitelyDodge()) {
            return true;
        }
        float remainTemp = GetDodgeRemain();
        return remainTemp > 0f && dodgePower > 0f && GetCanDodge() && Random.value <= remainTemp * CharacterManager.Instance.riskyDecSqrt * 0.1f;
    }

    public void SetScaffoldEffect(bool flag, GameObject effectPrefab = null) {
        if (flag) {
            if (!scaffoldEffectInstance && effectPrefab) {
                scaffoldEffectInstance = Instantiate(effectPrefab, trans);
            }
        } else {
            if (scaffoldEffectInstance) {
                Destroy(scaffoldEffectInstance);
            }
        }
    }

    protected virtual void ScrewStart() { }

    protected virtual void ScrewEnd() { }

    public void SetAnimDeadSpecial(bool flag) {
        anim.SetBool(AnimHash.Instance.ID[(int)AnimHash.ParamName.DeadSpecial], flag);
        animParam.deadSpecial = flag;
        deadTimer = flag ? float.MaxValue : 5;
        reviveTimer = flag ? float.MaxValue : 3.5f;
    }

    public void SetMovingFloor(MovingFloor movingFloor) {
        moveByFloorBody = movingFloor;
        moveByFloorRotationSave = moveByFloorBody.floorRigidbody.rotation.eulerAngles.y;
    }

    public void RemoveMovingFloor(MovingFloor movingFloor) {
        if (moveByFloorBody == movingFloor) {
            moveByFloorBody = null;
        }
    }

    public void DisadvantageDamage(int damageAmount) {
        if (nowHP > 1) {
            AddNowHP(-Mathf.Min(damageAmount, nowHP - 1), GetCenterPosition(), false, damageColor_Friends);
        }
        disadvantageTimeRemain = 0.4f;
    }

    public override void RemoveRide(bool replaceToReleasePoint = false) {
        if (fixFaceTimeRemain > 0f && (state == State.Damage || state == State.Dead || nowHP <= 0f)) {
            fixFaceTimeRemain = 0f;
        }
        base.RemoveRide(replaceToReleasePoint);
    }
    public override bool IsThrowCancelling {
        get {
            return false;
        }
    }

    public bool IsRotationEnabled() {
        return rideTarget == null && target == null && GetCanControl();
    }

    public void ResetFaciemBlink() {
        if (faciemBlink == null) {
            faciemBlink = GetComponent<FaciemBlink>();
        }
        if (faciemBlink) {
            bool flag = GameManager.Instance.save.config[GameManager.Save.configID_Blinking] != 0;
            if (faciemBlink.enabled != flag) {
                faciemBlink.enabled = flag;
                if (!flag && fCon.CurrentFaceIndex == faceIndex[(int)FaceName.Blink]) {
                    SetFace(faceIndex[(int)FaceName.Idle]);
                }
            }
        }
    }

}
