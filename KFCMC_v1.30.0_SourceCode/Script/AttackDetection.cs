using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class AttackDetection : MonoBehaviour {

    public enum ElementalAttribute { None, Bolt, Fire, Earth };
    public enum ReportType { None, Judgement, Hippo, PrairieDog, DarkWave, DarkInferno };
    protected const int receiverMax = 32;
    protected const int preservedMax = 8;
    protected const int notExistID = -1;
    protected static readonly Vector3 vecZero = Vector3.zero;
    protected static readonly Quaternion quaIden = Quaternion.identity;

    protected class DamageReceiver {
        public int characterId;
        public DamageDetection[] preservedDD;
        public float forgetTime;
        public int count;
        public DamageReceiver() {
            characterId = notExistID;
            preservedDD = new DamageDetection[preservedMax];
            forgetTime = 0f;
            count = 0;
        }
    }

    public GameObject attackEffect;
    public GameObject endEffect;
    public Vector3 offset;
    public bool effectParenting;
    public bool isEffectOnly;
    public string targetTag;
    public float attackRate = 1;
    public float knockRate = 1;
    public float damageRate = 1;
    public float multiHitInterval;
    public int multiHitMaxCount;
    public int relationIndex = -1;
    public XWeaponTrail[] xWeaponTrails;
    public ParticleSystem[] particles;
    public ParticleSystem[] s_particles;
    public float particleStopDelay;
    public float s_particleStopDelay;
    public bool independenceOnAwake;
    public bool ignoreParentMultiplier;
    public bool selectTopDamageRate;
    public bool penetrate;
    public bool isProjectile;
    public int overrideColorType = -1;
    public ReportType reportType;

    [System.NonSerialized]
    public bool attackEnabled;
    [System.NonSerialized]
    public CharacterBase parentCBase;
    [System.NonSerialized]
    public ElementalAttribute elementalAttribute;
    [System.NonSerialized]
    public bool unleashed;

    protected AttackDetection mySelf;
    protected Transform trans;
    protected Collider[] selfCollider;
    protected DamageDetection targetDD;
    protected DamageReceiver[] damageReceiver = new DamageReceiver[receiverMax];
    protected int[] hitReservedIndex = new int[receiverMax];
    protected int hitReservedCount;
    protected Vector3 closestPoint;
    protected Vector3 direction;
    protected float particleRemainTime;
    protected float s_particleRemainTime;
    protected float attackPowerSave;
    protected float knockPowerSave;
    protected int startProgress;
    protected bool checkParentSuperman;
    protected Collider[] targetColliders;
    protected bool[] checkFlags = new bool[preservedMax];
    protected float[] checkParams = new float[preservedMax];
    protected const float oneFrameSec = 1f / 60f;
    protected bool parentIsFriend;

    protected virtual void AwakeProcess() {
        trans = transform;
        mySelf = this;
        if (parentCBase == null) {
            parentCBase = GetComponentInParent<CharacterBase>();
        }
        selfCollider = GetComponents<Collider>();
        if (selfCollider.Length > 0) {
            for (int i = 0; i < selfCollider.Length; i++) {
                selfCollider[i].enabled = false;
            }
        }
        if (independenceOnAwake && parentCBase) {
            attackPowerSave = parentCBase.GetAttack(ignoreParentMultiplier);
            knockPowerSave = parentCBase.GetKnock(ignoreParentMultiplier);
        }
        for (int i = 0; i < receiverMax; i++) {
            damageReceiver[i] = new DamageReceiver();
            hitReservedIndex[i] = -1;
        }
        if (parentCBase && !parentCBase.isEnemy && !parentCBase.isPlayer) {
            parentIsFriend = true;
        }
        startProgress = 1;
    }

    protected virtual void StartProcess() {
        if (xWeaponTrails != null) {
            for (int i = 0; i < xWeaponTrails.Length; i++) {
                if (xWeaponTrails[i]) {
                    xWeaponTrails[i].Init();
                    xWeaponTrails[i].Deactivate();
                }
            }
        }
        if (particles != null && particles.Length > 0) {
            for (int i = 0; i < particles.Length; i++) {
                if (particles[i]) {
                    particles[i].Stop();
                    particles[i].Clear();
                }
            }
        }
        if (s_particles != null && s_particles.Length > 0) {
            for (int i = 0; i < s_particles.Length; i++) {
                if (s_particles[i]) {
                    s_particles[i].Stop();
                    s_particles[i].Clear();
                }
            }
        }
        startProgress = 2;
    }

    protected virtual void Awake() {
        if (startProgress == 0) {
            AwakeProcess();
        }
    }

    protected virtual void Start() {
        if (startProgress == 0) {
            AwakeProcess();
        }
        if (startProgress == 1) {
            StartProcess();
        }
    }

    protected virtual void Update() {
        UpdateReceiver();
        if (!attackEnabled) {
            if (particleStopDelay > 0f && particleRemainTime > 0f) {
                particleRemainTime -= Time.deltaTime;
                if (particleRemainTime <= 0) {
                    ParticlesStop();
                }
            }
            if (checkParentSuperman) {
                if (!parentCBase || !parentCBase.isSuperman) {
                    S_ParticlesStop();
                }
            }
            if (s_particleStopDelay > 0f && s_particleRemainTime > 0f) {
                s_particleRemainTime -= Time.deltaTime;
                if (s_particleRemainTime <= 0f) {
                    S_ParticlesStop();
                }
            }
        } else if (hitReservedCount > 0) {
            for (int i = 0; i < hitReservedCount; i++) {
                int index = hitReservedIndex[i];
                int answer = -2;
                if (index >= 0) {
                    for (int j = 0; j < preservedMax; j++) {
                        checkFlags[j] = (damageReceiver[index].preservedDD[j] != null);
                        if (checkFlags[j]) {
                            if (answer == -2) {
                                answer = j;
                            } else if (answer >= 0) {
                                answer = -1;
                            }
                        }
                    }
                    if (answer == -1) {
                        if (selectTopDamageRate) {
                            answer = SelectWithDamageRate(index, false);
                            if (answer == -1) {
                                answer = SelectWithDamageRate(index, true);
                                if (answer == -1) {
                                    answer = SelectWithDistance(index);
                                }
                            }
                        } else {
                            answer = SelectWithDistance(index);
                            if (answer == -1) {
                                answer = SelectWithDamageRate(index, false);
                                if (answer == -1) {
                                    answer = SelectWithDamageRate(index, true);
                                }
                            }
                        }
                    }
                    if (answer < 0) {
                        for (int j = 0; j < preservedMax; j++) {
                            if (checkFlags[j]) {
                                HitAttack(ref damageReceiver[index].preservedDD[j]);
                                break;
                            }
                        }
                    } else {
                        HitAttack(ref damageReceiver[index].preservedDD[answer]);
                    }
                    hitReservedIndex[i] = -1;
                }
            }
            hitReservedCount = 0;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!isEffectOnly && (parentCBase || independenceOnAwake) && other.CompareTag(targetTag)) {
            WorkEnter(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!isEffectOnly && (parentCBase || independenceOnAwake) && other.CompareTag(targetTag)) {
            WorkExit(other.gameObject);
        }
    }

   public virtual void WorkEnter(GameObject other) {
        if (attackEnabled) {
            targetDD = other.GetComponent<DamageDetection>();
            if (targetDD) {
                ConsiderHit(ref targetDD, true);
                if (relationIndex >= 0 && parentCBase) {
                    parentCBase.ConsiderHitAttackDetection(relationIndex, ref targetDD);
                }
            }
        }
    }

    public virtual void WorkExit(GameObject other) {
        targetDD = other.GetComponent<DamageDetection>();
        if (targetDD) {
            for (int i = 0; i < receiverMax; i++) {
                if (damageReceiver[i].characterId == targetDD.characterId) {
                    for (int j = 0; j < preservedMax; j++) {
                        if (damageReceiver[i].preservedDD[j] == targetDD) {
                            damageReceiver[i].preservedDD[j] = null;
                        }
                    }
                }
            }
        }
    }

    public virtual void ConsiderHit(ref DamageDetection targetDD, bool hitAttackEnabled) {
        int receiverBlank = -1;
        bool receiverExist = false;
        for (int i = 0; i < receiverMax; i++) {
            if (damageReceiver[i].characterId == notExistID) {
                if (receiverBlank < 0) {
                    receiverBlank = i;
                }
            } else if (damageReceiver[i].characterId == targetDD.characterId) {
                int preservedBlank = -1;
                bool preservedExist = false;
                receiverExist = true;
                for (int j = 0; j < preservedMax; j++) {
                    if (damageReceiver[i].preservedDD[j] == null) {
                        if (preservedBlank < 0) {
                            preservedBlank = j;
                        }
                    } else if (damageReceiver[i].preservedDD[j] == targetDD) {
                        preservedExist = true;
                        break;
                    }
                }
                if (!preservedExist && preservedBlank >= 0) {
                    damageReceiver[i].preservedDD[preservedBlank] = targetDD;
                }
            }
        }
        if (!receiverExist && receiverBlank >= 0) {
            if (hitAttackEnabled && hitReservedCount < hitReservedIndex.Length) {
                hitReservedIndex[hitReservedCount] = receiverBlank;
                hitReservedCount++;
            }
            damageReceiver[receiverBlank].characterId = targetDD.characterId;
            damageReceiver[receiverBlank].preservedDD[0] = targetDD;
            for (int i = 1; i < preservedMax; i++) {
                damageReceiver[receiverBlank].preservedDD[i] = null;
            }
            damageReceiver[receiverBlank].forgetTime = multiHitInterval;
            damageReceiver[receiverBlank].count = (multiHitInterval <= 0.01f ? 100 : 1);
        }
    }

    int SelectWithDistance(int index) {
        float minSqrDist = float.MaxValue;
        Vector3 pos = trans.position;
        int answer = -2;
        for (int i = 0; i < preservedMax; i++) {
            if (checkFlags[i]) {
                checkParams[i] = (GetClosestPoint(ref damageReceiver[index].preservedDD[i]) - pos).sqrMagnitude;
                if (checkParams[i] < minSqrDist) {
                    minSqrDist = checkParams[i];
                }
            } else {
                checkParams[i] = float.MaxValue;
            }
        }
        for (int i = 0; i < preservedMax; i++) {
            if (checkFlags[i]) {
                if (checkParams[i] > minSqrDist) {
                    checkFlags[i] = false;
                } else {
                    if (answer == -2) {
                        answer = i;
                    } else if (answer >= 0) {
                        answer = -1;
                    }
                }
            }
        }
        return answer;
    }

    int SelectWithDamageRate(int index, bool forKnocked = false) {
        float maxRate = float.MinValue;
        int answer = -2;
        for (int i = 0; i < preservedMax; i++) {
            if (checkFlags[i]) {
                checkParams[i] = forKnocked ? damageReceiver[index].preservedDD[i].knockedRate : damageReceiver[index].preservedDD[i].damageRate;
                if (checkParams[i] > maxRate) {
                    maxRate = checkParams[i];
                }
            } else {
                checkParams[i] = float.MinValue;
            }
        }
        for (int i = 0; i < preservedMax; i++) {
            if (checkFlags[i]) {
                if (checkParams[i] < maxRate) {
                    checkFlags[i] = false;
                } else {
                    if (answer == -2) {
                        answer = i;
                    } else if (answer >= 0) {
                        answer = -1;
                    }
                }
            }
        }
        return answer;
    }

    protected virtual void UpdateReceiver() {
        if (!isEffectOnly && multiHitInterval > 0.01f) {
            float deltaTimeCache = Time.deltaTime;
            for (int i = 0; i < receiverMax; i++) {
                if (damageReceiver[i].characterId != notExistID && (multiHitMaxCount <= 0 || damageReceiver[i].count < multiHitMaxCount)) {
                    damageReceiver[i].forgetTime -= deltaTimeCache;
                    if (damageReceiver[i].forgetTime <= 0f) {
                        bool ddExist = false;
                        for (int j = 0; j < preservedMax; j++) {
                            if (damageReceiver[i].preservedDD[j]) {
                                if (damageReceiver[i].preservedDD[j].gameObject.activeInHierarchy) {
                                    ddExist = true;
                                } else {
                                    damageReceiver[i].preservedDD[j] = null;
                                }
                            }
                        }
                        if (ddExist && hitReservedCount < hitReservedIndex.Length) {
                            hitReservedIndex[hitReservedCount] = i;
                            hitReservedCount++;
                            if (damageReceiver[i].forgetTime < -oneFrameSec) {
                                damageReceiver[i].forgetTime = -oneFrameSec;
                            }
                            damageReceiver[i].forgetTime += multiHitInterval;
                            damageReceiver[i].count++;
                        }
                    }
                }
            }
        }
    }

    protected virtual bool SendDamage(ref DamageDetection damageDetection, ref Vector3 closestPoint, ref Vector3 direction) {
        return damageDetection.ReceiveDamage(ref closestPoint, Mathf.RoundToInt(Mathf.Max(1f, CharacterBase.CalcDamage((independenceOnAwake ? attackPowerSave : parentCBase.GetAttack(ignoreParentMultiplier)) * attackRate * damageDetection.damageRate, damageDetection.GetDefence()) * damageRate)), (independenceOnAwake ? knockPowerSave : parentCBase.GetKnock(ignoreParentMultiplier)) * knockRate, ref direction, mySelf, penetrate, overrideColorType);
    }

    protected virtual Vector3 GetClosestPoint(ref DamageDetection targetDD) {
        if (targetDD.specialCollider != null) {
            return targetDD.specialCollider.ClosestPoint(trans.position);
        } else {
            targetColliders = targetDD.GetComponents<Collider>();
            if (targetColliders.Length == 1) {
                return targetColliders[0].ClosestPoint(trans.position);
            } else if (targetColliders.Length > 1) {
                float sqrDistMax = float.MaxValue;
                Vector3 answer = trans.position;
                for (int i = 0; i < targetColliders.Length; i++) {
                    Vector3 point = targetColliders[i].ClosestPoint(trans.position);
                    float sqrDistTemp = (point - trans.position).sqrMagnitude;
                    if (sqrDistTemp < sqrDistMax) {
                        sqrDistMax = sqrDistTemp;
                        answer = point;
                    }
                }
                return answer;
            }
        }
        return Vector3.Lerp(targetDD.transform.position, trans.position, 0.5f);
    }

    protected virtual void HitAttack(ref DamageDetection targetDD) {
        closestPoint = GetClosestPoint(ref targetDD);
        direction = targetDD.transform.position - trans.position;
        MyMath.NormalizeXZ(ref direction);
        SendDamage(ref targetDD, ref closestPoint, ref direction);
    }

    public virtual void DetectionStart(bool effectEnabled = true, bool weaponTrailsEnabled = true) {
        if (startProgress == 0) {
            AwakeProcess();
        }
        if (startProgress == 1) {
            StartProcess();
        }
        hitReservedCount = 0;
        attackEnabled = true;
        for (int i = 0; i < damageReceiver.Length; i++) {
            damageReceiver[i].characterId = notExistID;
        }
        if (selfCollider != null && selfCollider.Length > 0) {
            for (int i = 0; i < selfCollider.Length; i++) {
                if (selfCollider[i]) {
                    selfCollider[i].enabled = true;
                }
            }
        }
        if (effectEnabled && attackEffect) {
            Vector3 offsetTemp = vecZero;
            if (offset != vecZero && parentCBase) {
                offsetTemp = parentCBase.transform.TransformDirection(offset);
            }
            if (parentIsFriend && GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] != 0) {
                GameObject objTemp;
                if (effectParenting) {
                    objTemp = Instantiate(attackEffect, trans.position + offsetTemp, trans.rotation, trans);
                } else {
                    objTemp = Instantiate(attackEffect, trans.position + offsetTemp, quaIden);
                }
                AudioSource audioTemp = objTemp.GetComponent<AudioSource>();
                if (audioTemp) {
                    audioTemp.volume *= CharacterManager.Instance.GetObscureRateAudio();
                }
            } else {
                if (effectParenting) {
                    Instantiate(attackEffect, trans.position + offsetTemp, trans.rotation, trans);
                } else {
                    Instantiate(attackEffect, trans.position + offsetTemp, quaIden);
                }
            }
        }
        if (weaponTrailsEnabled && xWeaponTrails != null) {
            for (int i = 0; i < xWeaponTrails.Length; i++) {
                if (xWeaponTrails[i]) {
                    xWeaponTrails[i].Activate();
                }
            }
        }
        ParticlesPlay();
        S_ParticlesPlay();
    }

    public virtual void DetectionEnd(bool effectEnabled = true, bool weaponTrailsEnabled = true) {
        if (startProgress == 0) {
            AwakeProcess();
        }
        if (startProgress == 1) {
            StartProcess();
        }
        hitReservedCount = 0;
        attackEnabled = false;
        for (int i = 0; i < damageReceiver.Length; i++) {
            damageReceiver[i].characterId = notExistID;
        }
        if (selfCollider != null && selfCollider.Length > 0) {
            for (int i = 0; i < selfCollider.Length; i++) {
                if (selfCollider[i]) {
                    selfCollider[i].enabled = false;
                }
            }
        }
        if (effectEnabled && endEffect) {
            if (parentIsFriend && GameManager.Instance.save.config[GameManager.Save.configID_ObscureFriends] >= 2) {
                GameObject objTemp;
                if (effectParenting) {
                    objTemp = Instantiate(endEffect, trans.position, quaIden, trans);
                } else {
                    objTemp = Instantiate(endEffect, trans.position, quaIden);
                }
                AudioSource audioTemp = objTemp.GetComponent<AudioSource>();
                if (audioTemp) {
                    audioTemp.volume *= CharacterManager.Instance.GetObscureRateAudio();
                }
            } else {
                if (effectParenting) {
                    Instantiate(endEffect, trans.position, quaIden, trans);
                } else {
                    Instantiate(endEffect, trans.position, quaIden);
                }
            }
        }
        if (weaponTrailsEnabled && xWeaponTrails != null) {
            for (int i = 0; i < xWeaponTrails.Length; i++) {
                if (xWeaponTrails[i]) {
                    xWeaponTrails[i].StopSmoothly(0.1f);
                }
            }
        }
        if (particleStopDelay <= 0f) {
            ParticlesStop();
        } else {
            particleRemainTime = particleStopDelay;
        }
        if (s_particleStopDelay <= 0f) {
            S_ParticlesStop();
        } else {
            s_particleRemainTime = s_particleStopDelay;
        }
    }

    public void ActivateXWeaponTrails() {
        if (xWeaponTrails != null) {
            for (int i = 0; i < xWeaponTrails.Length; i++) {
                if (xWeaponTrails[i]) {
                    xWeaponTrails[i].Activate();
                }
            }
        }
    }

    public virtual void ParticlesPlay() {
        if (particles != null && particles.Length > 0) {
            for (int i = 0; i < particles.Length; i++) {
                if (particles[i]) {
                    particles[i].Play();
                }
            }
        }
    }

    public virtual void ParticlesStop() {
        if (particles != null && particles.Length > 0) {
            for (int i = 0; i < particles.Length; i++) {
                if (particles[i]) {
                    particles[i].Stop();
                }
            }
        }
    }

    public virtual void S_ParticlesPlay() {
        if (s_particles != null && s_particles.Length > 0 && parentCBase && parentCBase.isSuperman) {
            for (int i = 0; i < s_particles.Length; i++) {
                if (s_particles[i]) {
                    s_particles[i].Play();
                }
            }
            checkParentSuperman = true;
        }
    }

    public virtual void S_ParticlesStop() {
        if (s_particles != null && s_particles.Length > 0) {
            for (int i = 0; i < s_particles.Length; i++) {
                if (s_particles[i]) {
                    s_particles[i].Stop();
                }
            }
            s_particleRemainTime = -1f;
            checkParentSuperman = false;
        }
    }

    public virtual void XWeaponTrailsStop() {
        if (xWeaponTrails != null) {
            for (int i = 0; i < xWeaponTrails.Length; i++) {
                if (xWeaponTrails[i]) {
                    xWeaponTrails[i].Init();
                    xWeaponTrails[i].Deactivate();
                }
            }
        }
    }

    public void SetParentCharacterBase(CharacterBase cBase) {
        parentCBase = cBase;
        if (independenceOnAwake && parentCBase) {
            attackPowerSave = parentCBase.GetAttack(ignoreParentMultiplier);
            knockPowerSave = parentCBase.GetKnock(ignoreParentMultiplier);
        }
    }

    public CharacterBase GetParentCharacterBase() {
        return parentCBase;
    }

    protected virtual void OnEnable() {
        if (!isProjectile) {
            DetectionEnd(false, false);
            ParticlesStop();
            S_ParticlesStop();
            XWeaponTrailsStop();
            particleRemainTime = 0f;
            s_particleRemainTime = 0f;
        }
    }

}
