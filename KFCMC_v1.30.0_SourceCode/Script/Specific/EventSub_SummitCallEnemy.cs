using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSub_SummitCallEnemy : MonoBehaviour
{

    public int myNumber;
    public Event_Summit eventParent;
    public GameObject eventSubSisin;
    public GameObject callEnemyEffect;
    public GameObject breakEffect;
    public GameObject mapChip;
    
    int state;
    float elapsedTime;    

    private void OnTriggerEnter(Collider other) {
        if (state == 0 && eventParent.enemyZeroFlag && other.CompareTag("ItemGetter")) {
            Instantiate(callEnemyEffect, transform.position, transform.rotation);
            eventParent.CallEnemy(myNumber);
            if (mapChip) {
                mapChip.SetActive(false);
            }
            state = 1;
        }
    }

    private void Update() {
        if (state == 1){
            if (eventParent.enemyZeroFlag) {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 0.5f) {
                    state = 2;
                }
            } else {
                elapsedTime = 0f;
            }
        } else if (state == 2) {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
            eventSubSisin.SetActive(true);
            Destroy(gameObject);
            state = 3;
        }
    }

}
