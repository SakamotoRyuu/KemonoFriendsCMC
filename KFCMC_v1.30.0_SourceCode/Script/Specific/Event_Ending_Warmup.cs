using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Ending_Warmup : MonoBehaviour
{

    public GameObject blackCurtain;
    public GameObject timelineObject;
    public Event_Ending_CallFriends[] callFriends;

    private int frameWait = 1;

    private void Awake() {
        if (blackCurtain) {
            blackCurtain.SetActive(true);
        }
        if (timelineObject) {
            timelineObject.SetActive(false);
        }
        for (int i = 0; i < callFriends.Length; i++) {
            if (callFriends[i]) {
                callFriends[i].CallBody();
            }
        }
    }
    
    void Update() {
        if (frameWait == 0) {
            if (blackCurtain) {
                blackCurtain.SetActive(false);
            }
            if (timelineObject) {
                timelineObject.SetActive(true);
            }
            Destroy(gameObject);
        } else {
            frameWait--;
        }
    }
}
