using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_BigDogInside : EnemyBaseBoss {

    public int[] summonID;
    public int[] summonLevel;
    public Vector3[] summonPos;
    public GameObject[] nucleotide;
    public LineRenderer[] bands;
    public Material bandRedMat;

    GameObject[] enemyObjs;
    GameObject[] respawnObjs;
    List<EnemyBase> eBaseList = new List<EnemyBase>(16);
    Vector3 smoothVelocity;
    int nucleotidePointer;
    int summonPointer;
    int bandPointer;
    int destroyEnemyPointer;
    float elapsedTime;
    int eventState;

    protected override void Awake() {
        base.Awake();
        eBaseList.Clear();
        deadTimer = 5f;
        attackedTimeRemainOnDamage = 100f;
        attackWaitingLockonRotSpeed = 0f;
        attackLockonDefaultSpeed = 0f;
        sandstarRawKnockEndurance = 1000000;
        sandstarRawKnockEnduranceLight = 1000000;
        sandstarRawLockonSpeed = 0f;
        killByCriticalOnly = false;
        agent.enabled = false;
        if (StageManager.Instance && !StageManager.Instance.IsHomeStage) {
            for (int i = 0; i < summonID.Length; i++) {
                GameObject tempEnemy = Instantiate(CharacterDatabase.Instance.GetEnemy(summonID[i]));
                if (tempEnemy) {
                    Destroy(tempEnemy);
                }
            }
        }
    }

    protected override void Update_StatusJudge() {
        int listLength = eBaseList.Count;
        for (int i = listLength - 1; i >= 0; i--) {
            if (eBaseList[i] == null) {
                eBaseList.RemoveAt(i);
            } else {
                eBaseList[i].RegisterTargetHate(CharacterManager.Instance.pCon, 10f);
            }
        }
        slidingFlag = true;
        gravityZeroTimeRemain = 100f;
        attackedTimeRemain = 100f;
        base.Update_StatusJudge();
    }

    protected override void Update_MoveControl() {
        move = vecZero;
    }

    protected override void Update_Targeting() {
        base.Update_Targeting();
        Vector3 targetSearchPos = trans.position;
        if (targetTrans) {
            targetSearchPos.y = targetTrans.position.y + 1f;
        } else {
            targetSearchPos.y = trans.position.y + 1.5f;
        }
        targetSearchPos.y = Mathf.Clamp(targetSearchPos.y, trans.position.y + 1.5f, trans.position.y + 20f);
        searchTarget[0].transform.position = Vector3.SmoothDamp(searchTarget[0].transform.position, targetSearchPos, ref smoothVelocity, 0.5f);
    }

    protected override void DamageCommonProcess() {
        base.DamageCommonProcess();
        if (actDistNum == 0) {
            BattleStart();
        }
    }

    protected override void BattleStart() {
        CharacterManager.Instance.SetBossHP(this);
        CharacterManager.Instance.BossTimeStart();
        actDistNum = 1;
    }

    protected override void BattleEnd() {
        CharacterManager.Instance.SetBossHP(null);
    }

    public override void TakeDamage(int damage, Vector3 position, float knockAmount, Vector3 knockVector, CharacterBase attacker = null, int colorType = 0, bool penetrate = false) {
        base.TakeDamage(damage, position, knockAmount, knockVector, attacker, colorType, penetrate);
        if (nowHP > 0) {
            bool effectEmitted = false;
            int levelBias = GameManager.Instance.minmiRed ? 1 : 0;
            while (summonPointer < summonID.Length && nowHP <= Mathf.RoundToInt(GetMaxHP() * ((float)(summonID.Length + 1 - summonPointer) / (summonID.Length + 2)))) {
                int countMax = GameManager.Instance.minmiBlack ? 4 : 1;
                for (int i = 0; i < countMax; i++) {
                    eBaseList.Add(StageManager.Instance.dungeonController.SummonSpecificEnemy(summonID[summonPointer], sandstarRawEnabled ? 4 : Mathf.Clamp(summonLevel[summonPointer] + levelBias, 0, 4), trans.position + summonPos[(summonPointer + i) % summonPos.Length]));
                }
                summonPointer++;
                if (!effectEmitted) {
                    EmitEffect(0);
                    effectEmitted = true;
                }
            }
        }
        while (bandPointer < bands.Length && nowHP <= Mathf.RoundToInt(GetMaxHP() * ((float)(bands.Length + 3 - bandPointer - 1) / (bands.Length + 3)))) {
            if (bands[bandPointer] != null) {
                bands[bandPointer].material = bandRedMat;
            }
            bandPointer++;
        }
    }

    protected override void Start_Process_Dead() {
        base.Start_Process_Dead();
        enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        EmitEffect(1);
        CountDefeat();
        if (CharacterManager.Instance.pCon) {
            CharacterManager.Instance.pCon.SetEventMutekiTime(5f);
        }
        Ambient.Instance.StopFade(2f);
    }

    protected override void Update_Process_Dead() {
        base.Update_Process_Dead();
        elapsedTime += Time.deltaTime;
        if (eventState == 0) {
            if (nucleotidePointer < nucleotide.Length && elapsedTime >= (nucleotidePointer + 1) * 0.05f) {
                LineRenderer[] lineRenderers = nucleotide[nucleotidePointer].GetComponentsInChildren<LineRenderer>();
                if (lineRenderers.Length > 0) {
                    for (int i = 0; i < lineRenderers.Length; i++) {
                        lineRenderers[i].enabled = false;
                    }
                }
                Rigidbody[] rigidbodies = nucleotide[nucleotidePointer].GetComponentsInChildren<Rigidbody>();
                if (rigidbodies.Length > 0) {
                    for (int i = 0; i < rigidbodies.Length; i++) {
                        Vector3 randVel = Random.insideUnitSphere;
                        randVel.x *= 3f;
                        randVel.z *= 3f;
                        rigidbodies[i].isKinematic = false;
                        rigidbodies[i].useGravity = true;
                        rigidbodies[i].velocity = randVel;
                        rigidbodies[i].angularVelocity = Random.insideUnitSphere * 5f;
                    }
                }
                if (nucleotidePointer < bands.Length) {
                    bands[nucleotidePointer].enabled = false;
                }
                nucleotidePointer++;
            }
            if (enemyObjs.Length > 0 && destroyEnemyPointer < enemyObjs.Length && elapsedTime >= (destroyEnemyPointer + 1) * (1.5f / enemyObjs.Length)) {
                if (enemyObjs[destroyEnemyPointer] != null && enemyObjs[destroyEnemyPointer] != this.gameObject) {
                    EnemyBase enemyBaseTemp = enemyObjs[destroyEnemyPointer].GetComponent<EnemyBase>();
                    if (enemyBaseTemp && enemyBaseTemp.GetNowHP() > 0) {
                        enemyBaseTemp.ForceDeath();
                    }
                }
                destroyEnemyPointer++;
            }
            if (elapsedTime >= 3f) {
                eventState = 1;
                elapsedTime = 0f;
            }
        } else if (eventState == 1) {
            PauseController.Instance.pauseEnabled = false;
            PauseController.Instance.SetBlackCurtain(elapsedTime * 1f, true);
            if (elapsedTime >= 1.1f) {
                bool check = false;
                GameObject[] controllerObjs = GameObject.FindGameObjectsWithTag("GameController");
                for (int i = 0; i < controllerObjs.Length; i++) {
                    Event_BigDog eventBigDog = controllerObjs[i].GetComponent<Event_BigDog>();
                    if (eventBigDog != null) {
                        check = true;
                        eventBigDog.DefeatInside(sandstarRawEnabled);
                        break;
                    }
                }
                if (!check) {
                    PauseController.Instance.pauseEnabled = true;
                    PauseController.Instance.SetBlackCurtain(0, false);
                    PauseController.Instance.ReturnLibraryExternal();
                }
                eventState = 2;
                elapsedTime = 0f;
            }
        }
    }

}
