using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Alexandrium : EnemyBase
{

    public LaserOption[] laserOption;
    public RaycastToAdjustCapsuleCollider[] raycaster;
    public CapsuleCollider wallBreaker;
    public Vector2 wallBreakerCenterRange;
    public Vector2 wallBreakerHeightRange;

    int attackSave = -1;
    bool laserEnabled = false;
    int laserIndexMin;
    int laserIndexMax;
    int laserIndexReady;
    float strongLaserInterval;
    bool isLaserStrong;
    int noticeRemain;
    float strongLaserEmittingTime;
    const int EffectStrongLaserNotice = 0;
    const int AttackIndexStrongLaser = 3;
    const float StrongLaserIntervalMax = 12;
    static readonly float[] lockonSpeedArray = new float[] { 2.5f, 2.5f, 3.25f, 4.225f, 5.5f };

    protected override void Awake()
    {
        base.Awake();
        attackedTimeRemainOnRestoreKnockDown = 0.5f;
        attackLockonDefaultSpeed = lockonSpeedArray[0];
        LaserCancel();
    }

    protected override void SetLevelModifier()
    {
        attackLockonDefaultSpeed = lockonSpeedArray[Mathf.Clamp(level, 0, lockonSpeedArray.Length - 1)];
        strongLaserInterval = StrongLaserIntervalMax;
    }

    protected override void Update_StatusJudge()
    {
        base.Update_StatusJudge();
        if (state != State.Attack && laserEnabled)
        {
            LaserCancel();
        }
        if (target && strongLaserInterval > 0)
        {
            strongLaserInterval -= deltaTimeCache;
        }
        if (state == State.Attack && isLaserStrong && noticeRemain > 0 && stateTime >= (84f / 60f - 0.1f - (noticeRemain - 1) * 0.4f))
        {
            noticeRemain--;
            EmitEffect(EffectStrongLaserNotice);
        }
        if (state == State.Attack && attackDetection[AttackIndexStrongLaser].attackEnabled)
        {
            strongLaserEmittingTime += deltaTimeCache;
            wallBreaker.center = Vector3.forward * Mathf.Lerp(wallBreakerCenterRange.x, wallBreakerCenterRange.y, Mathf.Clamp(strongLaserEmittingTime * 10f, 0, 1));
            wallBreaker.height = Mathf.Lerp(wallBreakerHeightRange.x, wallBreakerHeightRange.y, Mathf.Clamp(strongLaserEmittingTime * 10f, 0, 1));
            if (!wallBreaker.enabled)
            {
                wallBreaker.enabled = true;
            }
        }
        else
        {
            if (wallBreaker.enabled)
            {
                wallBreaker.enabled = false;
            }
        }
    }

    void LaserCancel()
    {
        laserEnabled = false;
        for (int i = 0; i < laserOption.Length; i++)
        {
            if (laserOption[i])
            {
                laserOption[i].CancelLaser();
            }
        }
        for (int i = 0; i < raycaster.Length; i++)
        {
            if (raycaster[i])
            {
                raycaster[i].Deactivate();
            }
        }
    }

    void LaserReady()
    {
        laserEnabled = true;
        AttackStart(laserIndexReady);
        for (int i = laserIndexMin; i < laserOption.Length && i <= laserIndexMax; i++)
        {
            if (laserOption[i])
            {
                laserOption[i].LightFlickeringChargeStart();
            }
        }
        for (int i = laserIndexMin; i < raycaster.Length && i <= laserIndexMax; i++)
        {
            if (raycaster[i])
            {
                raycaster[i].hitEffectEnabled = false;
                raycaster[i].Activate();
            }
        }
    }

    void LaserStart()
    {
        laserEnabled = true;
        AttackEnd(laserIndexReady);
        for (int i = laserIndexMin; i < laserOption.Length && i <= laserIndexMax; i++)
        {
            if (laserOption[i])
            {
                laserOption[i].LightFlickeringChargeEnd();
            }
        }
        for (int i = 0; i < raycaster.Length && i <= laserIndexMax; i++)
        {
            if (raycaster[i])
            {
                raycaster[i].hitEffectEnabled = true;
                raycaster[i].Activate();
            }
        }
    }

    void LaserEnd()
    {
        laserEnabled = false;
        for (int i = 0; i < laserOption.Length; i++)
        {
            if (laserOption[i])
            {
                laserOption[i].LightFlickeringBlastEnd();
            }
        }
        for (int i = 0; i < raycaster.Length; i++)
        {
            if (raycaster[i])
            {
                raycaster[i].Deactivate();
            }
        }
    }

    int GetAttackIndex()
    {
        return Random.Range(0, level >= 4 ? 6 : level == 3 ? 5 : level == 2 ? 4 : 3);
    }

    void AttackStartForLaser()
    {
        for (int i = laserIndexMin; i < attackDetection.Length && i <= laserIndexMax; i++)
        {
            AttackStart(i);
        }
    }

    void AttackEndForLaser()
    {
        for (int i = laserIndexMin; i < attackDetection.Length && i <= laserIndexMax; i++)
        {
            AttackEnd(i);
        }
    }

    protected override void Attack()
    {
        int attackRandom = GetAttackIndex();
        int laserLevel;
        isLaserStrong = false;
        noticeRemain = 3;
        if (attackRandom == attackSave)
        {
            attackRandom = GetAttackIndex();
        }
        if (level >= 5 && attackRandom <= 3 && strongLaserInterval <= 0)
        {
            isLaserStrong = true;
            laserLevel = 4;
            laserIndexMin = 3;
            laserIndexMax = 3;
            laserIndexReady = 5;
            strongLaserInterval = StrongLaserIntervalMax;
        }
        else
        {
            laserIndexReady = 4;
            if (attackRandom <= 3)
            {
                laserLevel = 1;
                laserIndexMin = 0;
                laserIndexMax = 0;
            }
            else if (attackRandom == 4)
            {
                laserLevel = 2;
                laserIndexMin = 0;
                laserIndexMax = 1;
            }
            else
            {
                laserLevel = 3;
                laserIndexMin = 0;
                laserIndexMax = 2;
            }
        }
        if (isLaserStrong)
        {
            float spRate = 60f / 84f;
            AttackBase(attackRandom, 1.4f, 50f, 0, 150f / 60f / spRate, 150f / 60f / spRate + GetAttackInterval(1.5f), 0f, spRate);
        }
        else
        {
            AttackBase(attackRandom, 1f, 0.8f, 0, 150f / 60f, 150f / 60f + GetAttackInterval(1.5f, laserLevel * -1), 0f);
        }
        if (level >= 3)
        {
            SuperarmorStart();
        }
        attackSave = attackRandom;
    }
}
