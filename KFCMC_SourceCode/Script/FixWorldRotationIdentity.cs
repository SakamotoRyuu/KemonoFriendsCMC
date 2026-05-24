using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixWorldRotationIdentity : MonoBehaviour
{

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

}
