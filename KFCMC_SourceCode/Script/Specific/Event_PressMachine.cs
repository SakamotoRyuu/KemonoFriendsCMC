using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Event_PressMachine : MonoBehaviour {

    public GameObject[] prefab;
    public Transform[] pivot;
    public GameObject pressMachine;

    float stayTime;
    int progress;
    const string targetTag = "EnemyDamageDetection";

    private void OnTriggerEnter(Collider other) {
        if (progress <= 0 && other.CompareTag(targetTag)) {
            Enemy_Euglena euglena = other.GetComponentInParent<Enemy_Euglena>();
            if (euglena) {
                NavMeshAgent euglenaAgent = euglena.GetComponent<NavMeshAgent>();
                if (euglenaAgent) {
                    euglenaAgent.enabled = false;
                }
            }
            progress = 1;
        }
    }

    private void Update() {
        if (progress >= 1) {
            stayTime += Time.deltaTime;
            if (progress == 1 && stayTime >= 1.5f) {
                Instantiate(prefab[0], pivot[0]);
                progress = 2;
            }
            if (progress == 2 && stayTime >= 4.5f) {
                pressMachine.SetActive(true);
                Instantiate(prefab[1], pivot[1]);
                progress = 3;
            }
            if (progress == 3 && stayTime >= 5.0f) {
                Instantiate(prefab[2], pivot[2]);
                if (CharacterManager.Instance) {
                    CharacterManager.Instance.EveryFriendsFear();
                }
                progress = 4;
            }
            if (progress == 4 && stayTime >= 6.0f) {
                if (CharacterManager.Instance && CharacterManager.Instance.GetFriendsExist(1, true)) {
                    CharacterManager.Instance.SetSpecialChat("EVENT_KABAN_10", 1, -1);
                }
                progress = 5;
            }
            if (progress == 5 && stayTime >= 7.0f) {
                TrophyManager.Instance.CheckTrophy(TrophyManager.t_PressMachine, true);
                progress = 6;
            }
        }
    }

}
