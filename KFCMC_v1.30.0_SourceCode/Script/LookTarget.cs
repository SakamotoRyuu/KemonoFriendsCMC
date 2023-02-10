using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTarget : MonoBehaviour {

    public Vector3 anglesOffset;
    public CharacterBase cBase;

    Transform trans;
    static readonly Vector3 vecZero = Vector3.zero;
    
	void Start () {
        trans = transform;
		if (cBase == null) {
            cBase = GetComponentInParent<CharacterBase>();
        }
	}
	
	void Update () {
		if (cBase != null && cBase.target != null) {
            trans.LookAt(cBase.target.transform);
            if (anglesOffset != vecZero) {
                trans.eulerAngles += anglesOffset;
            }
        }
	}
}
