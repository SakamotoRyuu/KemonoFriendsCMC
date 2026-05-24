using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalForCaveOfTribulations : MonoBehaviour
{
    public GameObject healDisabledObj;
    public GameObject healEnabledObj;
    public GameObject effectPrefab;
    public Transform effectPivot;
    bool mapChipEnabledSave;

    public bool SetHealEnabled()
    {
        if (healDisabledObj && healDisabledObj.activeSelf)
        {
            MapChipControl mapCon = healDisabledObj.GetComponentInChildren<MapChipControl>();
            if (mapCon && mapCon.chipRenderer && mapCon.chipRenderer.enabled)
            {
                mapChipEnabledSave = true;
                enabled = true;
            }
            healDisabledObj.SetActive(false);
        }
        if (healEnabledObj && !healEnabledObj.activeSelf)
        {
            healEnabledObj.SetActive(true);
            if (effectPrefab && effectPivot)
            {
                Instantiate(effectPrefab, effectPivot.position, effectPivot.rotation);
            }
            return true;
        }
        return false;
    }

    void Update()
    {
        if (mapChipEnabledSave)
        {
            MapChipControl mapCon = healEnabledObj.GetComponentInChildren<MapChipControl>();
            if (mapCon && mapCon.chipRenderer)
            {
                mapCon.chipRenderer.enabled = true;
            }
            mapChipEnabledSave = false;
        }
        enabled = false;
    }
}
