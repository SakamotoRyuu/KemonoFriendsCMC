using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPower : MonoBehaviour {

    public enum BombType {
        Normal, Poison, Acid
    };

    public BombType bombType;
    public float scaleTime = 1f;
    public float maxRadius = 20f;
    public float damageRate = 1f;
    public bool destroyOnComplete = true;
    public bool damageFriends;
    public float maxRadiusForFriends = 40f;

    EnemyBase[] eBase;
    FriendsBase[] fBase;
    bool[] enemyTreated;
    bool[] friendsTreated;
    float elapsedTime;
    float scaleSpeed;
    Transform trans;
    int damageAmount;
    float knockAmount;
    BombReceiverForEnemy[] bombReceiver;
    bool bombReceiverExist;
    int t_defeatCount;

    private void Start() {
        trans = transform;
        if (scaleTime > 0) {
            scaleSpeed = maxRadius / scaleTime;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        eBase = new EnemyBase[enemies.Length];
        for (int i = 0; i < enemies.Length; i++) {
            eBase[i] = enemies[i].GetComponent<EnemyBase>();
        }
        enemyTreated = new bool[enemies.Length];

        GameObject[] bombReceiverObj = GameObject.FindGameObjectsWithTag("BombReceiver");
        if (bombReceiverObj.Length > 0) {
            bombReceiverExist = true;
            bombReceiver = new BombReceiverForEnemy[bombReceiverObj.Length];
            for (int i = 0; i < bombReceiver.Length; i++) {
                bombReceiver[i] = bombReceiverObj[i].GetComponent<BombReceiverForEnemy>();
            }
        }

        GameObject[] friends = GameObject.FindGameObjectsWithTag("Player");
        fBase = new FriendsBase[friends.Length];
        for (int i = 0; i < friends.Length; i++) {
            fBase[i] = friends[i].GetComponent<FriendsBase>();
        }
        friendsTreated = new bool[friends.Length];
        damageAmount = Mathf.RoundToInt(GameManager.Instance.GetLevelStatusNow() * 320f * damageRate);
        if (bombType != BombType.Normal) {
            damageAmount = damageAmount * 3 / 4;
        }
        knockAmount = (GameManager.Instance.GetLevelNow + 50) * 20f * damageRate;
        if (CharacterManager.Instance) {
            CharacterManager.Instance.bossResult.useItemCount++;
            if (GameManager.Instance.save.stageReport.Length >= GameManager.stageReportMax) {
                GameManager.Instance.save.stageReport[GameManager.stageReport_ItemCount]++;
            }
        }
    }

    private void Update() {
        elapsedTime += Time.deltaTime;
        float condDist = Mathf.Clamp(elapsedTime * scaleSpeed, 0f, maxRadius);
        float condSqrDist = condDist * condDist;
        for (int i = 0; i < eBase.Length; i++) {
            if (eBase[i] && eBase[i].enabled && !enemyTreated[i] && (eBase[i].transform.position - trans.position).sqrMagnitude <= condSqrDist) {
                enemyTreated[i] = true;
                eBase[i].TakeDamageFixKnock(damageAmount, eBase[i].GetCenterPosition(), knockAmount, (eBase[i].transform.position - trans.position).normalized, eBase[i].GetTargetExist() ? CharacterManager.Instance.pCon : null, 3, true);
                if (!TrophyManager.Instance.IsTrophyHad(TrophyManager.t_Bomb) && eBase[i].GetNowHP() <= 0) {
                    t_defeatCount++;
                    if (t_defeatCount >= 20) {
                        TrophyManager.Instance.CheckTrophy(TrophyManager.t_Bomb, true);
                    }
                }
                switch (bombType) {
                    case BombType.Normal:
                        eBase[i].ResetSickToleranceAll();
                        break;
                    case BombType.Poison:
                        eBase[i].ResetSickTolerance(CharacterBase.SickType.Poison);
                        eBase[i].SetSick(CharacterBase.SickType.Poison, 40);
                        eBase[i].ResetSickToleranceAll();
                        break;
                    case BombType.Acid:
                        eBase[i].ResetSickTolerance(CharacterBase.SickType.Acid);
                        eBase[i].SetSick(CharacterBase.SickType.Acid, 40);
                        eBase[i].ResetSickToleranceAll();
                        break;
                }
            }
        }
        if (bombReceiverExist) {
            for (int i = 0; i < bombReceiver.Length; i++) {
                if (bombReceiver[i] && (bombReceiver[i].transform.position - trans.position).sqrMagnitude <= condSqrDist) {
                    bombReceiver[i].DestroyWithEffect();
                }
            }
        }
        if (damageFriends) {
            condDist = Mathf.Clamp(elapsedTime * scaleSpeed, 0f, maxRadiusForFriends);
            condSqrDist = condDist * condDist;
            for (int i = 0; i < fBase.Length; i++) {
                if (fBase[i] && fBase[i].enabled && !friendsTreated[i] && (fBase[i].transform.position - trans.position).sqrMagnitude <= condSqrDist) {
                    friendsTreated[i] = true;
                    int nowHP = fBase[i].GetNowHP();
                    int damage = 0;
                    switch (bombType) {
                        case BombType.Normal:
                            damage = Mathf.Min(Mathf.RoundToInt((200 + StageManager.Instance.GetSlipDamage(false) * 10) * damageRate), nowHP - 1);
                            fBase[i].TakeDamage(damage, fBase[i].GetCenterPosition(), 400 * damageRate, (fBase[i].transform.position - trans.position).normalized, null, 0, true);
                            break;
                        case BombType.Poison:
                            damage = Mathf.Min(Mathf.RoundToInt((200 + StageManager.Instance.GetSlipDamage(false) * 10) * 0.75f * damageRate), nowHP - 1);
                            fBase[i].TakeDamage(damage, fBase[i].GetCenterPosition(), 400 * damageRate, (fBase[i].transform.position - trans.position).normalized, null, 0, true);
                            fBase[i].SetSick(CharacterBase.SickType.Poison, 20);
                            break;
                        case BombType.Acid:
                            damage = Mathf.Min(Mathf.RoundToInt((200 + StageManager.Instance.GetSlipDamage(false) * 10) * 0.75f * damageRate), nowHP - 1);
                            fBase[i].TakeDamage(damage, fBase[i].GetCenterPosition(), 400 * damageRate, (fBase[i].transform.position - trans.position).normalized, null, 0, true);
                            fBase[i].SetSick(CharacterBase.SickType.Acid, 20);
                            break;
                    }
                }
            }
        }
        if (destroyOnComplete && elapsedTime >= scaleTime + 0.05f) {
            Destroy(gameObject);
        }
    }
}