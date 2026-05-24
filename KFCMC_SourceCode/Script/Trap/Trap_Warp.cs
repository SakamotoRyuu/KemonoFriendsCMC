using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Warp : TrapBase {

    public GameObject effectPrefab;
    public bool isStrong;
    public GameObject weakPrefab;    

    protected override void WorkEnter(int index) {
        if (CharacterManager.Instance && !GameManager.Instance.megatonCoin) {
            cBaseList[index].BlownAway();
            if (effectPrefab) {
                GameObject effect1 = Instantiate(effectPrefab, cBaseList[index].transform);
                GameObject effect2 = Instantiate(effectPrefab, transform.position, Quaternion.identity);
                AudioSource aSrc;
                if (cBaseList[index].isPlayer) {
                    aSrc = effect2.GetComponent<AudioSource>();
                    if (isStrong) {
                        isStrong = false;
                        StartCoroutine(WeakenTrap());
                    }
                } else {
                    aSrc = effect1.GetComponent<AudioSource>();
                }
                if (aSrc) {
                    aSrc.enabled = false;
                }
            }
        }
    }

    IEnumerator WeakenTrap() {
        yield return new WaitForSeconds(1f);
        if (weakPrefab) {
            Instantiate(weakPrefab, transform.position, transform.rotation, transform.parent);
            Destroy(gameObject);
        }
    }

}
