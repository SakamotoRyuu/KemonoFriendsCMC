using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWithPlayerHeight : MonoBehaviour {

    public GameObject activateTargetObj;
    public Transform standardTrans;
    public float offset;
    public bool activateOnPlayerGreaterThanStandard = false;
    
    Transform playerTrans;
    
	void Start () {
        playerTrans = CharacterManager.Instance.playerTrans;
	}
	
	void Update () {
        if (playerTrans) {
            bool toActive = ((activateOnPlayerGreaterThanStandard && playerTrans.position.y > standardTrans.position.y + offset) || (!activateOnPlayerGreaterThanStandard && playerTrans.position.y < standardTrans.position.y + offset));
            if (activateTargetObj.activeSelf != toActive) {
                activateTargetObj.SetActive(toActive);
            }
        }
	}
}
