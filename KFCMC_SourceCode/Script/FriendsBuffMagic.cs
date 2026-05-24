using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsBuffMagic : MonoBehaviour {

    public enum MagicType { IbisSong, MargayVoice, WolfHowling, HomoSapiensVoice, AustralopithecusShout };

    public MagicType magicType;
    public bool toFriends;
    public bool toEnemy;
    public CharacterBase parent;
    public GameObject decoyObject;
    public float delayForEnemy;
    public float delayForFriends;
    public float bossConditionPlus;
    public LayerMask wallLayerMask;

    FriendsBase[] fBase;
    EnemyBase[] eBase;
    bool[] fTreated;
    bool[] eTreated;
    Transform trans;
    GameObject targetObj;
    CharacterBase targetCBase;
    int enemyTreatCount;

    float elapsedTime;
    bool hasAffected;
    protected const int damageColor_Heal = 1;

    private void Awake() {
        trans = transform;
        parent = GetComponentInParent<CharacterBase>();
        GameObject[] stTemp = parent.GetSearchTarget();
        if (stTemp.Length > 0) {
            decoyObject = stTemp[0];
        }
    }

    private void Start() {
        if (toEnemy) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            eBase = new EnemyBase[enemies.Length];
            for (int i = 0; i < enemies.Length; i++) {
                eBase[i] = enemies[i].GetComponent<EnemyBase>();
            }
            eTreated = new bool[enemies.Length];
        }
        if (toFriends) {
            GameObject[] friends = GameObject.FindGameObjectsWithTag("Player");
            fBase = new FriendsBase[friends.Length];
            for (int i = 0; i < friends.Length; i++) {
                fBase[i] = friends[i].GetComponent<FriendsBase>();
            }
            fTreated = new bool[friends.Length];
        }
        if (magicType == MagicType.IbisSong) {
            CharacterManager.Instance.specialHealReported = 10;
        }
        if (magicType == MagicType.AustralopithecusShout && parent) {
            targetObj = parent.GetNowTarget();
            if (targetObj) {
                targetCBase = targetObj.GetComponentInParent<CharacterBase>();
            }
        }
    }

    private void Update() {
        float condDist = trans.localScale.x * 0.5f;
        elapsedTime += Time.deltaTime;
        if (toEnemy && elapsedTime >= delayForEnemy) {
            for (int i = 0; i < eBase.Length; i++) {
                if (eBase[i] != null && !eTreated[i]) {
                    float condDistTemp = condDist;
                    float sqrDistTemp = (eBase[i].transform.position - trans.position).sqrMagnitude;
                    if (bossConditionPlus != 0f && eBase[i].isBoss) {
                        condDistTemp += bossConditionPlus;
                    }
                    if (sqrDistTemp < condDistTemp * condDistTemp) {
                        eTreated[i] = true;
                        if (!hasAffected)
                        {
                            hasAffected = true;
                            if (parent && parent.isPlayer && CharacterManager.Instance)
                            {
                                CharacterManager.Instance.JustDodgeAmountPlusNonAttack();
                            }
                        }
                        switch (magicType) {
                            case MagicType.IbisSong:
                                if (parent && parent.GetNowHP() > 0 && decoyObject) {
                                    eBase[i].MakeAngry(decoyObject, parent, parent && parent.isPlayer);
                                }
                                break;
                            case MagicType.MargayVoice:
                                if (parent && parent.GetNowHP() > 0) {
                                    eBase[i].Confuse(parent && parent.isPlayer);
                                }
                                break;
                            case MagicType.WolfHowling:
                                if (parent && parent.GetNowHP() > 0) {
                                    eBase[i].ReceiveWolfHowling(parent && parent.isPlayer);
                                }
                                break;
                            case MagicType.HomoSapiensVoice:
                                if (parent) {
                                    eBase[i].SetHomoParent(parent);
                                }
                                break;
                            case MagicType.AustralopithecusShout:
                                if (parent && parent != eBase[i] && (enemyTreatCount < 4 || sqrDistTemp < 2500f)) {
                                    eBase[i].SetConcentratedAttack(parent.transform.position, targetObj, targetCBase);
                                    enemyTreatCount++;
                                }
                                break;
                        }
                    }
                }
            }
        }
        if (toFriends && elapsedTime >= delayForFriends && parent && parent.GetNowHP() > 0) {
            int healAmount = CharacterManager.Instance.GetIbisSongHeal();
            if (healAmount > 0) {
                for (int i = 0; i < fBase.Length; i++) {
                    if (fBase[i] != null && !fTreated[i] && (fBase[i].transform.position - trans.position).sqrMagnitude <= condDist * condDist) {
                        fTreated[i] = true;
                        switch (magicType) {
                            case MagicType.IbisSong:
                                if (fBase[i].gameObject != parent.gameObject) {
                                    fBase[i].AddNowHP(healAmount, fBase[i].GetCenterPosition(), true, damageColor_Heal);
                                }
                                break;
                        }
                        if (!hasAffected)
                        {
                            hasAffected = true;
                            if (parent && parent.isPlayer && CharacterManager.Instance)
                            {
                                CharacterManager.Instance.JustDodgeAmountPlusNonAttack();
                                CharacterManager.Instance.PlayerAttackStamp();
                            }
                        }
                    }
                }
            }
        }
    }
}