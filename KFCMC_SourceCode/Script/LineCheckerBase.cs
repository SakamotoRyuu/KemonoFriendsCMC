using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCheckerBase : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform point;
    public Vector3 offset;

    protected bool reaching;

    public virtual bool Reaching => reaching;

}
