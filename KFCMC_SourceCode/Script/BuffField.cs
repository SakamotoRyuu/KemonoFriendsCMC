using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffField : MonoBehaviour {

    public string targetTag = "Player";
    public FriendsBase.FieldBuffType type;

    FriendsBase fBaseTemp;
    int playerEntering;
    int[] friendsEntering = new int[GameManager.friendsMax];
    CharacterBase parentCBase;
    bool parentIsPlayer;

    private void Awake()
    {
        parentCBase = GetComponentInParent<CharacterBase>();
        if (parentCBase && parentCBase.isPlayer && CharacterManager.Instance)
        {
            CharacterManager.Instance.JustDodgeAmountPlusNonAttack(0.5f);
            CharacterManager.Instance.PlayerAttackStamp();
            parentIsPlayer = true;
        }
    }

    private void Update() {
        if (CharacterManager.Instance) {
            if (playerEntering >= 2 && CharacterManager.Instance.pCon && CharacterManager.Instance.pCon != parentCBase) {
                CharacterManager.Instance.pCon.SetFieldBuff(type, parentIsPlayer);
            }
            for (int i = 0; i < friendsEntering.Length; i++) {
                if (friendsEntering[i] >= 2 && CharacterManager.Instance.friends[i].fBase && CharacterManager.Instance.friends[i].fBase != parentCBase) {
                    CharacterManager.Instance.friends[i].fBase.SetFieldBuff(type, parentIsPlayer);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CharacterManager.Instance && other.CompareTag(targetTag))
        {
            fBaseTemp = other.GetComponent<FriendsBase>();
            if (fBaseTemp && fBaseTemp != parentCBase)
            {
                if (fBaseTemp.isPlayer)
                {
                    if (playerEntering == 0)
                    {
                        CharacterManager.Instance.ShowFieldBuff(fBaseTemp.transform.position, type, true);
                    }
                    playerEntering = 2;
                }
                else if (fBaseTemp.friendsId >= 0 && fBaseTemp.friendsId < friendsEntering.Length)
                {
                    if (friendsEntering[fBaseTemp.friendsId] == 0)
                    {
                        CharacterManager.Instance.ShowFieldBuff(fBaseTemp.transform.position, type, false);
                    }
                    friendsEntering[fBaseTemp.friendsId] = 2;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CharacterManager.Instance && other.CompareTag(targetTag))
        {
            fBaseTemp = other.GetComponent<FriendsBase>();
            if (fBaseTemp && fBaseTemp != parentCBase)
            {
                if (fBaseTemp.isPlayer)
                {
                    playerEntering = 1;
                }
                else if (fBaseTemp.friendsId >= 0 && fBaseTemp.friendsId < friendsEntering.Length)
                {
                    friendsEntering[fBaseTemp.friendsId] = 1;
                }
            }
        }
    }

}
