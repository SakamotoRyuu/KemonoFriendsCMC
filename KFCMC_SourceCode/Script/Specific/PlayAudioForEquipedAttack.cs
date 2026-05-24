using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioForEquipedAttack : MonoBehaviour {

    public AudioSource aSrc;
    public int conditionEquipID;

    private void Awake() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_ShowArms] == 1 && GameManager.Instance.save.equip[conditionEquipID] != 0) {
            aSrc.Play();
        }
    }

}
