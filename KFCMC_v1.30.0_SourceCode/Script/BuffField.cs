using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffField : MonoBehaviour {

    public string targetTag = "Player";
    public FriendsBase.FieldBuffType type;

    FriendsBase fBaseTemp;
    int playerEntering;
    int[] friendsEntering = new int[GameManager.friendsMax];
    const string registerHateTag = "Enemy";
    const int parentFriendsID = 23;
    const float hateConditionDistance = 10f;

    private void Start() {
        FriendsBase parentFriendsBase = CharacterManager.Instance.GetFriendsBase(parentFriendsID);
        if (parentFriendsBase) {
            Transform parentTrans = parentFriendsBase.transform;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(registerHateTag);
            EnemyBase enemyBaseTemp;
            float amount = CharacterManager.Instance.GetNormalKnockAmount();
            for (int i = 0; i < enemies.Length; i++) {
                enemyBaseTemp = enemies[i].GetComponent<EnemyBase>();
                if (enemyBaseTemp) {
                    if (enemyBaseTemp.isBoss) {
                        enemyBaseTemp.RegisterTargetHate(parentFriendsBase, amount * 4f);
                    } else if ((enemies[i].transform.position - parentTrans.position).sqrMagnitude <= hateConditionDistance * hateConditionDistance) {
                        enemyBaseTemp.RegisterTargetHate(parentFriendsBase, amount);
                    }
                }
            }
        }
    }

    private void Update() {
        if (CharacterManager.Instance) {
            if (playerEntering >= 2 && CharacterManager.Instance.pCon) {
                CharacterManager.Instance.pCon.SetFieldBuff(type);
            }
            for (int i = 0; i < friendsEntering.Length; i++) {
                if (friendsEntering[i] >= 2 && CharacterManager.Instance.friends[i].fBase) {
                    CharacterManager.Instance.friends[i].fBase.SetFieldBuff(type);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (CharacterManager.Instance && other.CompareTag(targetTag)) {
            fBaseTemp = other.GetComponent<FriendsBase>();
            if (fBaseTemp) {
                if (fBaseTemp.isPlayer) {
                    if (playerEntering == 0) {
                        CharacterManager.Instance.ShowFieldBuff(fBaseTemp.transform.position, type, true);
                    }
                    playerEntering = 2;
                } else if (fBaseTemp.friendsId >= 0 && fBaseTemp.friendsId < friendsEntering.Length && fBaseTemp.friendsId != parentFriendsID) {
                    if (friendsEntering[fBaseTemp.friendsId] == 0) {
                        CharacterManager.Instance.ShowFieldBuff(fBaseTemp.transform.position, type, false);
                    }
                    friendsEntering[fBaseTemp.friendsId] = 2;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (CharacterManager.Instance && other.CompareTag(targetTag)) {
            fBaseTemp = other.GetComponent<FriendsBase>();
            if (fBaseTemp) {
                if (fBaseTemp.isPlayer) {
                    playerEntering = 1;
                } else if (fBaseTemp.friendsId >= 0 && fBaseTemp.friendsId < friendsEntering.Length && fBaseTemp.friendsId != parentFriendsID) {
                    friendsEntering[fBaseTemp.friendsId] = 1;
                }
            }
        }
    }

}
