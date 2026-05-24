using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCheckerWithCameraOrPlayer : LineCheckerWithCamera
{

    public float surelyReachDistanceForPlayerNotReaching;
    public float surelyReachDistanceForPlayerOnReaching;
    public bool ignoreYForPlayer;

    protected bool reachingForPlayer;

    public override bool Reaching => reaching || reachingForPlayer;

    protected override void CheckReaching()
    {
        base.CheckReaching();
        Vector3 targetPos = point.position + offset;
        Vector3 playerPos = CharacterManager.Instance.playerSearchArea.transform.position;
        if (ignoreYForPlayer)
        {
            targetPos.y = playerPos.y = 0;
        }
        float sqrDist = (targetPos - playerPos).sqrMagnitude;
        if (!reachingForPlayer && sqrDist <= surelyReachDistanceForPlayerNotReaching * surelyReachDistanceForPlayerNotReaching)
        {
            reachingForPlayer = true;
        }
        else if (reachingForPlayer && sqrDist <= surelyReachDistanceForPlayerOnReaching * surelyReachDistanceForPlayerOnReaching)
        {
            reachingForPlayer = true;
        }
        else if (sqrDist >= unreachableDistance * unreachableDistance)
        {
            reachingForPlayer = false;
        }
        else
        {
            reachingForPlayer = !Physics.Linecast(targetPos, playerPos, layerMask);
        }
    }

    public override void AddDistance(float amount)
    {
        base.AddDistance(amount);
        surelyReachDistanceForPlayerNotReaching += amount;
        surelyReachDistanceForPlayerOnReaching += amount;
    }

}
