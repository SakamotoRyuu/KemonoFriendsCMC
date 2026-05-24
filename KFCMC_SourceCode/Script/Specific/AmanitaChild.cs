using System.Diagnostics;
using UnityEngine;
using UnityEngine.Analytics;

public class AmanitaChild : TriggerReceiverDamage
{

    public float instantiateTimer;
    public float destroyTimer;
    public int enemyID;
    public int level;
    public float knockEndurance;
    public GameObject spawnEffectPrefab;
    public Transform spawnEffectPivot;
    public BombReceiverForEnemy bombReceiver;

    private Rigidbody rigid;
    private float instantiateTimeRemain;
    private float destroyTimeRemain;
    private float knockRemain;
    private bool isInstantiateTimerStarted;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        knockRemain = knockEndurance;
        instantiateTimeRemain = instantiateTimer;
        destroyTimeRemain = destroyTimer;
    }

    void Update()
    {
        if (destroyTimeRemain > 0)
        {
            destroyTimeRemain -= Time.deltaTime;
            if (destroyTimeRemain <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                if (!isInstantiateTimerStarted && rigid.IsSleeping())
                {
                    isInstantiateTimerStarted = true;
                }
                if (isInstantiateTimerStarted && instantiateTimeRemain > 0)
                {
                    instantiateTimeRemain -= Time.deltaTime;
                    if (instantiateTimeRemain <= 0)
                    {
                        GameObject prefab = CharacterDatabase.Instance.GetEnemy(enemyID);
                        if (prefab != null)
                        {
                            EnemyBase eBase = Instantiate(prefab, transform.position, transform.rotation, transform.parent).GetComponent<EnemyBase>();
                            if (eBase != null)
                            {
                                eBase.enemyID = enemyID;
                                int variableLevel = 0;
                                if (StageManager.Instance && StageManager.Instance.dungeonController)
                                {
                                    variableLevel = StageManager.Instance.dungeonController.variableLevel;
                                }
                                eBase.SetLevel(level, false, true, variableLevel);
                            }
                            if (spawnEffectPrefab != null)
                            {
                                if (spawnEffectPivot != null)
                                {
                                    Instantiate(spawnEffectPrefab, spawnEffectPivot.position, spawnEffectPivot.rotation);
                                }
                                else
                                {
                                    Instantiate(spawnEffectPrefab, transform.position, transform.rotation);
                                }
                            }
                        }
                        Destroy(gameObject, 0.5f);
                        enabled = false;
                    }
                }
            }
        }
    }

    public override void ReceiveDamage(Vector3 effectPosition, int damage, float knockAmount, Vector3 knockVector, AttackDetection attackDetection = null, bool penetrate = false, int overrideColorType = -1)
    {
        knockRemain -= knockAmount;
        instantiateTimeRemain = instantiateTimer;
        destroyTimeRemain = destroyTimer;
        if (knockRemain <= 0)
        {
            if (bombReceiver)
            {
                bombReceiver.BootDeathEffect();
            }
            Destroy(gameObject);
        }
    }

}
