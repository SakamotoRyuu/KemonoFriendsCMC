using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustDodgeTrigger : MonoBehaviour {

    public GameObject effectPrefab;
    public CharacterManager.JustDodgeType justDodgeType;
    public PlayerController parentPlayer;
    bool sentFlag;

    private void OnTriggerEnter(Collider other) {
        if (!sentFlag && other.CompareTag("EnemyAttackDetection") && parentPlayer && parentPlayer.justDodgeIntervalRemain <= 0f) {
            sentFlag = true;
            parentPlayer.justDodgeIntervalRemain = 0.5f;
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
            CharacterManager.Instance.ShowJustDodge(justDodgeType, other.GetComponent<AttackDetection>());
            Destroy(gameObject);
        }
    }

}
