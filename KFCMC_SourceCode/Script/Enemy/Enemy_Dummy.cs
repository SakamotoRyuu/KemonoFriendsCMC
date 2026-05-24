using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class Enemy_Dummy : EnemyBase {

    public Canvas infoCanvas;
    public TMP_Text infoText;
    public TMP_Text infoTextSmall;
    public GameObject deadEvent;

    double lastDamagedTime;
    double startTime;
    int totalDamage;
    float totalKnock;
    StringBuilder sb = new StringBuilder();
    const int stealedItemID = 94;
    const int rewardMinmiShiftNum = 0;

    protected override void Start() {
        base.Start();
        lastDamagedTime = GameManager.Instance.time - 100.0;
        infoCanvas.enabled = false;
        forceDefaultDifficulty = true;
    }

    protected override void Update() {
        base.Update();
        if (GameManager.Instance.time >= lastDamagedTime + 8.0 && infoCanvas.enabled) {
            infoCanvas.enabled = false;
            totalDamage = 0;
            totalKnock = 0f;
        }
        if (infoCanvas.enabled) {
            CommonLockon();
        }
    }

    protected override void LoadEnemyCanvas() {
        base.LoadEnemyCanvas();
        if (enemyCanvas) {
            enemyCanvas.gameObject.SetActive(false);
            enemyCanvasLoaded = false;
        }
    }

    protected override void Start_Process_Dead() {
        if (deadEvent) {
            Instantiate(deadEvent, trans.position, Quaternion.identity);
        }
        base.Start_Process_Dead();
    }

    protected override void DeadProcess() {
        expForce = true;
        if (isSuperman || level >= 4) {
            GiveItem(stealedItemID);
            GameManager.Instance.save.minmi |= (1 << rewardMinmiShiftNum);
        }
        base.DeadProcess();
    }

    public override int GetMaxHP() {
        int rate = 1;
        if (GameManager.Instance.save.difficulty == 3) {
            rate = 2;
        } else if (GameManager.Instance.save.difficulty >= 4) {
            rate = 5;
        }
        return maxHP * rate;
    }

    public override int GetStealItemID() {
        if (stealedCount < stealedMax) {
            stealedCount++;
            GameManager.Instance.save.minmi |= (1 << rewardMinmiShiftNum);
            return stealedItemID;
        }
        return -1;
    }

    private void LateUpdate() {
        if (state != State.Dead) {
            nowHP = GetMaxHP();
        }
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        float dps = 0;
        float knockPS = 0f;
        nowHP = GetMaxHP();
        lastDamagedTime = GameManager.Instance.time;
        totalDamage += damage;
        totalKnock += knockAmount;
        if (!infoCanvas.enabled) {
            infoCanvas.enabled = true;
            startTime = lastDamagedTime - 1.0;
        }
        float timeDiff = (float)(lastDamagedTime - startTime);
        dps = totalDamage / timeDiff;
        knockPS = totalKnock / timeDiff;
        infoText.text = sb.Clear().Append(totalDamage).AppendLine().AppendFormat("{0:0.0}", dps).Append("/s").ToString();
        infoTextSmall.text = sb.Clear().AppendFormat("{0:0.0}", knockAmount).AppendLine().AppendFormat("{0:0.0}", knockPS).Append("/s").ToString();
    }

}
