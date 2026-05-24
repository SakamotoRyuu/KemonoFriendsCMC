using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpecificEnemiesFarPoint : MonoBehaviour {

    public Transform[] pivots;
    public float radius;
    public int enemyID;
    public int level;
    public int count;
    public float rayHeight;
    public float rayLength;
    public LayerMask rayLayer;
    public bool lookatPlayer;
    public bool dropExpDisabled;
    public bool enableMinmiRed;
    public bool enableMinmiBlack;
    public bool defeatAllCheckTrophy;

    static readonly Vector3 vecUp = Vector3.up;
    static readonly Vector3 vecDown = Vector3.down;
    bool summonedFlag;

    void Start() {
        if (StageManager.Instance.dungeonController && CharacterManager.Instance.playerTrans) {
            if (lookatPlayer && CharacterManager.Instance.pCon) {
                GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemyObjs.Length > 0) {
                    for (int i = 0; i < enemyObjs.Length; i++) {
                        EnemyBase eBase = enemyObjs[i].GetComponent<EnemyBase>();
                        if (eBase) {
                            eBase.RegisterTargetHate(CharacterManager.Instance.pCon, 10f);
                        }
                    }
                }
            }
            float maxDist = -1f;
            int index = -1;
            if (pivots.Length >= 2) {
                for (int i = 0; i < pivots.Length; i++) {
                    if (pivots[i]) {
                        float distTemp = (pivots[i].position - CharacterManager.Instance.playerTrans.position).sqrMagnitude;
                        if (distTemp > maxDist) {
                            maxDist = distTemp;
                            index = i;
                        }
                    }
                }
            } else if (pivots.Length == 1 && pivots[0]) {
                index = 0;
            }
            if (index >= 0) {
                if (enableMinmiBlack && GameManager.Instance.minmiBlack) {
                    count *= 2;
                }
                for (int i = 0; i < count; i++) {
                    Vector3 summonPos = pivots[index].position;
                    if (radius > 0f) {
                        Vector2 randTemp = Random.insideUnitCircle * radius;
                        summonPos.x += randTemp.x;
                        summonPos.z += randTemp.y;
                    }
                    if (rayHeight != 0f && rayLength > 0f) {
                        Ray ray = new Ray(summonPos + vecUp * rayHeight, vecDown);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, rayLength, rayLayer, QueryTriggerInteraction.Ignore)) {
                            summonPos = hit.point;
                        }
                    }
                    int levelBias = (enableMinmiRed && GameManager.Instance.minmiRed ? 1 : 0);
                    EnemyBase eBase = StageManager.Instance.dungeonController.SummonSpecificEnemy(enemyID, Mathf.Clamp(level + levelBias, 0, 4), summonPos);
                    if (eBase) {
                        summonedFlag = true;
                        if (lookatPlayer) {
                            eBase.LookAtIgnoreY(CharacterManager.Instance.playerTrans.position);
                            if (CharacterManager.Instance.pCon) {
                                eBase.RegisterTargetHate(CharacterManager.Instance.pCon, 40f);
                            }
                        }
                        if (dropExpDisabled) {
                            eBase.dropExpDisabled = true;
                        }
                    }
                }
            }
        }
        if (!defeatAllCheckTrophy) {
            enabled = false;
        }
    }

    void Update() {
        if (defeatAllCheckTrophy && summonedFlag && StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.EnemyCount() <= 0) {
            defeatAllCheckTrophy = false;
            if (TrophyManager.Instance) {
                TrophyManager.Instance.CheckTrophy_DefeatGraphCollapse();
            }
            enabled = false;
        }
    }

}
