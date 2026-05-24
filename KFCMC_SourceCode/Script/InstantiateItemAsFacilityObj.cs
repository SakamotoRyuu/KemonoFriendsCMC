using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateItemAsFacilityObj : MonoBehaviour
{
    public int itemID;
    public int facilityID;

    void Start()
    {
        GameObject prefab = ItemDatabase.Instance.GetItemPrefab(itemID);
        if (prefab)
        {
            GameObject objTemp = Instantiate(prefab, transform);
            if (PauseController.Instance)
            {
                PauseController.Instance.SetFacilityObj(facilityID, objTemp);
            }
        }
    }
}
