using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableComponentOnRigidbodyNotKinematic : MonoBehaviour
{

    public Rigidbody checkTargetRigidbody;
    public MonoBehaviour enableTargetComponent;

    private void Update()
    {
        if (enableTargetComponent && !enableTargetComponent.enabled && checkTargetRigidbody && !checkTargetRigidbody.isKinematic)
        {
            enableTargetComponent.enabled = true;
            enabled = false;
        }
    }

}
