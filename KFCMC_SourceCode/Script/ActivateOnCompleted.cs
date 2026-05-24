using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnCompleted : ActivateOnProgress
{

    public bool requireFriendsCompleted;
    public bool requireMinmiCompleted;
    public bool requireDocumentCompleted;
    public bool requireDefeatSinWRCompleted;

    protected override bool CheckCondition()
    {
        return base.CheckCondition()
            && (!requireFriendsCompleted || GameManager.Instance.save.GetRescueNow() >= GameManager.rescueMax)
            && (!requireMinmiCompleted || GameManager.Instance.GetMinmiCompleted())
            && (!requireDocumentCompleted || GameManager.Instance.GetDocumentCompleted())
            && (!requireDefeatSinWRCompleted || GameManager.Instance.GetDefeatSinWRComplete());
    }

}
