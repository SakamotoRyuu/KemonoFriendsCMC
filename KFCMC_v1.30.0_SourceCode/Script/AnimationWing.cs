using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationWing : MonoBehaviour {

    public Vector3 targetRotation;
    public bool forLocalEuler;

    MMD4MecanimBone bone;
    Animator anim;
    Transform trans;
    Quaternion boneRotation;
    int hash;

	// Use this for initialization
	void Start () {
        anim = GetComponentInParent<Animator>();
        bone = GetComponent<MMD4MecanimBone>();
        trans = transform;
        if (AnimHash.Instance) {
            hash = AnimHash.Instance.ID[(int)AnimHash.ParamName.Curve];
        }
    }
	
	void Update () {
		if (anim && hash != 0) {
            if (forLocalEuler) {
                Vector3 eulerTemp = targetRotation * anim.GetFloat(hash);
                trans.localEulerAngles = eulerTemp;
            } else {
                boneRotation = Quaternion.Euler(targetRotation * anim.GetFloat(hash));
                if (bone) {
                    bone.userRotation = boneRotation;
                }
                trans.localRotation = boneRotation;
            }
        }
	}
}
