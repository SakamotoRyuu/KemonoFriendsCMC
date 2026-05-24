using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_SetPlayerJumping : MonoBehaviour
{

    public float height;
    public float moveVelocity;

    void Start() {
        if (CharacterManager.Instance.pCon) {
            CharacterManager.Instance.pCon.Event_PlayerJumping(height, moveVelocity);
        }
    }
    
}
