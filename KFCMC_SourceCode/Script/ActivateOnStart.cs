using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnStart : MonoBehaviour
{
    public GameObject[] activateObjs;
    public bool isDeactivate;

    void Start()
    {
        for (int i = 0; i < activateObjs.Length; i++)
        {
            if (activateObjs[i])
            {
                activateObjs[i].SetActive(!isDeactivate);
            }
        }
    }

}
