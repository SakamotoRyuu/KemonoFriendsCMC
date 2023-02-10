using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class AttackDetectionPlayer : AttackDetection {

    bool longFlag = false;
    CapsuleCollider selfCapCol;
    GameObject giraffeBeamInstance;
    Vector3 defCenter;
    float defRadius;
    float defHeight;
    protected bool defSelect;
    protected int defDirection;
    bool isPlayer = false;

    public bool overrideLongEnabled;
    [System.Serializable]
    public class OverrideLongSettings {
        public Vector3 longCenter;
        public float longRadius;
        public float longHeight;
        public int longDirection;
        public GameObject giraffeBeam;
    }
    public OverrideLongSettings overrideLongSettings;
    public bool forHyper;

    Vector3 longCenter = new Vector3(0, -2.4f, 0);
    const float longRadius = 1.2f;
    const float longHeight = 7.2f;
    
    protected virtual void SetColliderSize(bool toLong) {
        if (selfCapCol) {
            if (toLong) {
                if (overrideLongEnabled) {
                    selfCapCol.center = overrideLongSettings.longCenter;
                    selfCapCol.radius = overrideLongSettings.longRadius;
                    selfCapCol.height = overrideLongSettings.longHeight;
                    selfCapCol.direction = overrideLongSettings.longDirection;
                } else {
                    selfCapCol.center = longCenter;
                    selfCapCol.radius = longRadius;
                    selfCapCol.height = longHeight;
                }
                selectTopDamageRate = true;
            } else {
                selfCapCol.center = defCenter;
                selfCapCol.radius = defRadius;
                selfCapCol.height = defHeight;
                selfCapCol.direction = defDirection;
                selectTopDamageRate = defSelect;
            }
        }
    }

    protected virtual void CheckBuff() {
        if (CharacterManager.Instance) {
            if (CharacterManager.Instance.GetBuff(CharacterManager.BuffType.Long) || (forHyper && parentCBase && parentCBase.isSuperman)) {
                if (!longFlag) {
                    SetColliderSize(true);
                    if (!giraffeBeamInstance && EffectDatabase.Instance) {
                        if (overrideLongEnabled) {
                            giraffeBeamInstance = Instantiate(overrideLongSettings.giraffeBeam, transform);
                        } else {
                            giraffeBeamInstance = Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.giraffeBeam], transform);
                        }
                    }
                    longFlag = true;
                }
            } else {
                if (longFlag) {
                    SetColliderSize(false);
                    if (giraffeBeamInstance) {
                        Destroy(giraffeBeamInstance);
                        giraffeBeamInstance = null;
                    }
                    longFlag = false;
                }
            }
        }
    }

    protected override bool SendDamage(ref DamageDetection damageDetection, ref Vector3 closestPoint, ref Vector3 direction) {
        if (base.SendDamage(ref damageDetection, ref closestPoint, ref direction)) {
            if (isPlayer) {
                parentCBase.HitAttackAdditiveProcess();
            }
            return true;
        }
        return false;
    }

    protected override void Awake() {
        base.Awake();
        defSelect = selectTopDamageRate;
    }

    protected override void Start () {
        base.Start();
        selfCapCol = GetComponent<CapsuleCollider>();
        if (selfCapCol) {
            defCenter = selfCapCol.center;
            defRadius = selfCapCol.radius;
            defHeight = selfCapCol.height;
            defDirection = selfCapCol.direction;
        }
        isPlayer = parentCBase.isPlayer;
    }
	
	protected override void Update () {
        base.Update();
        CheckBuff();
	}

}
