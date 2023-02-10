using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnFriendsDead : MonoBehaviour {

    public int friendsID;
    public bool askLive;
    
	// Update is called once per frame
	void Update () {
		if (!CharacterManager.Instance.GetFriendsExist(friendsID, askLive)) {
            Destroy(gameObject);
        }
	}
}
