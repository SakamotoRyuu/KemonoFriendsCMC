using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMoney : MonoBehaviour {

    public int num;
    public Collider col;
    
    private const float delay = 0.3f;

    private void Start() {
        SetMapChip();
        StartCoroutine(EnablizeCollider());
    }

    protected virtual void SetMapChip() {
        if (StageManager.Instance && StageManager.Instance.dungeonController && StageManager.Instance.IsMappingFloor && StageManager.Instance.dungeonController.usedGoldGraph && (GameManager.Instance.save.config[GameManager.Save.configID_DisableAutoMapping] == 0 || StageManager.Instance.mapActivateFlag >= 1)) {
            GameObject mapChipInstance = Instantiate(MapDatabase.Instance.prefab[MapDatabase.gold], transform);
            mapChipInstance.GetComponent<MapChipControl>().chipRenderer.enabled = true;
            if (GameManager.Instance.save.config[GameManager.Save.configID_DisableAutoMapping] != 0 && StageManager.Instance.mapActivateFlag == 1) {
                mapChipInstance.GetComponent<MapChipControl>().temporaryFlag = true;
            }
        } else {
            Instantiate(MapDatabase.Instance.prefab[MapDatabase.gold], transform);
        }
    }

    IEnumerator EnablizeCollider() {
        yield return new WaitForSeconds(delay);
        if (col) {
            col.enabled = true;
        }
    }
}
