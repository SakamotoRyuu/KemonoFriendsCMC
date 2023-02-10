using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetectionContainer : DamageDetectionNotCharacter {

    public int rank = 0;
    public GameObject balloonPrefab;
    public int enemyMin = 1;
    public int enemyMax = 3;

    enum Type { Item, Gold, Enemy };
    Type itemType;
    int[] itemArray;
    int goldNum;
    int enemyNum;
    GameObject balloonInstance;
    ContainerBalloonController balloonCon;

    const int rankMax = 5;
    const float condDist = 20f;
    const float balloonHeight = 1.5f;
    const int healStarID = 352;
    const int sandstarBlockID = 354;
    static readonly int[] moneyBase = new int[] { 30, 55, 100, 180, 300 };

    protected override void Awake() {
        base.Awake();
        trans = transform;
    }

    protected override void Start() {
        base.Start();
        SetMapChip();
        int randMin = 0;
        if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.dungeonController.chestSettings.notCallEnemy) {
            randMin = 8;
        }
        int rate = Random.Range(randMin, 100);
        if (rate < 8) {
            itemType = Type.Enemy;
            enemyNum = Random.Range(enemyMin, enemyMax + 1);
        } else if (rate < 38) {
            if ((StageManager.Instance && StageManager.Instance.IsRandomStage) || (GameManager.Instance && GameManager.Instance.IsPlayerAnother)) {
                itemType = Type.Item;
                itemArray = new int[2] { Random.Range(0, 2) == 0 ? healStarID : sandstarBlockID, -1 };
            } else {
                itemType = Type.Gold;
                goldNum = (int)(moneyBase[rank] * Random.Range(0.75f, 1.25f));
            }
        } else {
            itemType = Type.Item;
            itemArray = ContainerDatabase.Instance.GetIDArray(rank);
            int replaceLevel = (StageManager.Instance && StageManager.Instance.dungeonController ? StageManager.Instance.dungeonController.itemReplaceLevel : 0);
            if (replaceLevel > 0) {
                for (int i = 0; i < itemArray.Length; i++) {
                    for (int j = 0; j < ItemDatabase.replaceBeforeID.Length; j++) {
                        if (itemArray[i] == ItemDatabase.replaceBeforeID[j]) {
                            itemArray[i] = ItemDatabase.replaceAfterID[Mathf.Clamp(replaceLevel - 1, 0, ItemDatabase.replaceRows - 1), Random.Range(0, ItemDatabase.replaceColumns)];
                            break;
                        }
                    }
                }
            }
            if (CharacterManager.Instance) {
                for (int i = 0; i < itemArray.Length; i++) {
                    CharacterManager.Instance.CheckJaparimanShortage(itemArray[i]);
                }
            }
        }
        StartCoroutine(UpdateBalloon());
    }

    protected virtual bool CheckBalloonCondition() {
        return (CharacterManager.Instance && CharacterManager.Instance.GetFriendsEffect(CharacterManager.FriendsEffect.KeenNose) != 0 && CharacterManager.Instance.playerTrans && (CharacterManager.Instance.playerTrans.position - trans.position).sqrMagnitude <= condDist * condDist);
    }

    IEnumerator UpdateBalloon() {
        var wait = new WaitForSeconds(0.25f);
        for (; ; ) {
            if (CheckBalloonCondition()) {
                if (balloonInstance && balloonCon) {
                    balloonCon.Activate(true);
                } else {
                    balloonInstance = Instantiate(balloonPrefab, trans.position + new Vector3(0f, balloonHeight, 0f), trans.rotation, trans);
                    balloonCon = balloonInstance.GetComponent<ContainerBalloonController>();
                    if (balloonCon) {
                        int itemID = (itemType == Type.Enemy ? -2 : itemType == Type.Gold ? -1 : itemArray.Length > 0 ? itemArray[0] : -3);
                        if (itemID >= -2) {
                            balloonCon.SetBalloon(itemID);
                        }
                        balloonCon.Activate(true);
                    }
                }
            } else {
                if (balloonInstance && balloonCon) {
                    balloonCon.Activate(false);
                }
            }
            yield return wait;
        }
    }

    protected virtual void SetMapChip() {
        Instantiate(MapDatabase.Instance.prefab[MapDatabase.item], transform);
    }

    public override void WorkDamage(int damage, float knockAmount, Vector3 knockVector) {
        if (itemType == Type.Enemy) {
            if (StageManager.Instance && StageManager.Instance.dungeonController) {
                StageManager.Instance.dungeonController.SpawnEnemyPos(transform.position, 0.5f, enemyNum);
                if (TrophyManager.Instance && !TrophyManager.Instance.IsTrophyHad(TrophyManager.t_GrayWolf) && CheckBalloonCondition()) {
                    TrophyManager.Instance.CheckTrophy(TrophyManager.t_GrayWolf, true);
                }
            }
        } else if (itemType == Type.Gold) {
            ItemDatabase.Instance.GiveGold(goldNum, transform);
        } else {
            ItemDatabase.Instance.GiveItem(itemArray, transform);
        }
        Destroy(transform.parent.gameObject);
    }

}