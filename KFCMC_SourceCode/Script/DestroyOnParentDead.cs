using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnParentDead : MonoBehaviour
{

    CharacterBase cBase;

    void Awake()
    {
        cBase = GetComponentInParent<CharacterBase>();
    }

    void Update()
    {
        if (cBase == null || cBase.GetNowHP() <= 0)
        {
            Destroy(gameObject);
        }
    }
}
