using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAreaAlwaysTargetingPlayer : SearchArea
{
    protected override void Update()
    {
        if (CharacterManager.Instance)
        {
            SetNowTarget(CharacterManager.Instance.playerSearchTarget.gameObject);
        }
    }
}
