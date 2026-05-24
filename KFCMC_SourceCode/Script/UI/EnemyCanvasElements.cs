using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCanvasElements : MonoBehaviour
{
    public GameObject BackNormal;
    public GameObject BackWRFill;
    public GameObject KnockRecovery;
    public GameObject KnockDiff;
    public GameObject KnockFill;
    public GameObject HpDiff;
    public GameObject HpFill;
    public GameObject WrFill;

    public void SetActiveAll(bool value)
    {
        BackNormal.SetActive(value);
        BackWRFill.SetActive(value);
        KnockRecovery.SetActive(value);
        KnockDiff.SetActive(value);
        KnockFill.SetActive(value);
        HpDiff.SetActive(value);
        HpFill.SetActive(value);
        WrFill.SetActive(value);
    }

}
