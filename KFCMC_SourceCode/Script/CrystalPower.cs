using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPower : MonoBehaviour {

    public enum CrystalType {
        Red, Blue, Violet, GreenS, GreenM, GreenL, Crimson, White
    }

    public CrystalType crystalType;
    public float scaleTime = 1f;
    public float maxRadius = 40f;
    public bool destroyOnComplete = true;

    EnemyBase[] eBase;
    Transform[] eTrans;
    BombReceiverForEnemy[] bombReceiver;
    bool[] treated;
    float elapsedTime;
    float scaleSpeed;
    Transform trans;
    bool bombReceiverExist;
    int t_Count;

    private void Start() {
        trans = transform;
        if (scaleTime > 0f) {
            scaleSpeed = maxRadius / scaleTime;
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        eBase = new EnemyBase[enemies.Length];
        eTrans = new Transform[enemies.Length];
        treated = new bool[enemies.Length];
        for (int i = 0; i < enemies.Length; i++) {
            eBase[i] = enemies[i].GetComponent<EnemyBase>();
            eTrans[i] = enemies[i].transform;
        }
        if (crystalType == CrystalType.GreenS || crystalType == CrystalType.GreenM || crystalType == CrystalType.GreenL) {
            GameObject[] bombReceiverObj = GameObject.FindGameObjectsWithTag("BombReceiver");
            if (bombReceiverObj.Length > 0) {
                bombReceiverExist = true;
                bombReceiver = new BombReceiverForEnemy[bombReceiverObj.Length];
                for (int i = 0; i < bombReceiver.Length; i++) {
                    bombReceiver[i] = bombReceiverObj[i].GetComponent<BombReceiverForEnemy>();
                }
            }
        }
    }

    private void Update() {
        elapsedTime += Time.deltaTime;
        float condDist = Mathf.Min(elapsedTime * scaleSpeed, maxRadius);
        float condSqrDist = condDist * condDist;
        for (int i = 0; i < eBase.Length; i++) {
            if (eBase[i] && eBase[i].enabled && !treated[i] && (eTrans[i].position - trans.position).sqrMagnitude <= condSqrDist) {
                treated[i] = true;
                switch (crystalType) {
                    case CrystalType.Red:
                        if (!eBase[i].isBoss) {
                            eBase[i].LevelUp();
                            eBase[i].SupermanEnd();
                            t_Count++;
                            if (t_Count == 3) {
                                TrophyManager.Instance.CheckTrophy(TrophyManager.t_RedCrystal, true);
                            }
                        }
                        break;
                    case CrystalType.Blue:
                        if (!eBase[i].isBoss) {
                            eBase[i].LevelDown();
                            eBase[i].SupermanEnd();
                        }
                        break;
                    case CrystalType.Violet:
                        eBase[i].SetSick(CharacterBase.SickType.Slow, 40);
                        break;
                    case CrystalType.GreenS:
                        eBase[i].TakeDamageFixKnock(Mathf.RoundToInt(1000 + GameManager.Instance.GetLevelStatusNow() * 120f), eBase[i].GetCenterPosition(), 1500, (eTrans[i].position - trans.position).normalized, eBase[i].GetTargetExist() ? CharacterManager.Instance.pCon : null, 3, true);
                        eBase[i].ResetSickToleranceAll();
                        break;
                    case CrystalType.GreenM:
                        eBase[i].TakeDamageFixKnock(Mathf.RoundToInt(1000 + GameManager.Instance.GetLevelStatusNow() * 120f) * 2, eBase[i].GetCenterPosition(), 3000, (eTrans[i].position - trans.position).normalized, eBase[i].GetTargetExist() ? CharacterManager.Instance.pCon : null, 3, true);
                        eBase[i].ResetSickToleranceAll();
                        break;
                    case CrystalType.GreenL:
                        eBase[i].TakeDamageFixKnock(Mathf.RoundToInt(1000 + GameManager.Instance.GetLevelStatusNow() * 120f) * 4, eBase[i].GetCenterPosition(), 50000, (eTrans[i].position - trans.position).normalized, eBase[i].GetTargetExist() ? CharacterManager.Instance.pCon : null, 3, true);
                        eBase[i].ResetSickToleranceAll();
                        break;
                    case CrystalType.Crimson:
                        if (!eBase[i].isBoss && !eBase[i].isSuperman) {
                            eBase[i].ReserveSuperman();
                            t_Count++;
                            if (t_Count == 3) {
                                TrophyManager.Instance.CheckTrophy(TrophyManager.t_CrimsonCrystal, true);
                            }
                        }
                        break;
                    case CrystalType.White:
                        if (!eBase[i].isBoss) {
                            eBase[i].SupermanEnd();
                            eBase[i].SetLevel(0, true, false, 1);
                        }
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
        if (destroyOnComplete && elapsedTime >= scaleTime + 0.05f) {
            Destroy(gameObject);
        }
    }
}