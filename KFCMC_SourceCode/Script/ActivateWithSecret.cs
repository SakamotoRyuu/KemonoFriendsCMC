using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWithSecret : MonoBehaviour
{

    public GameObject activateTarget;
    public GameManager.SecretType secretType;

    void Start()
    {
        if (activateTarget && GameManager.Instance && GameManager.Instance.GetSecret(secretType))
        {
            activateTarget.SetActive(true);
        }
    }

}
