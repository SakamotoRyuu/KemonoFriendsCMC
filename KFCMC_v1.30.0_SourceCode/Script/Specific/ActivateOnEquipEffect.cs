using UnityEngine;

public class ActivateOnEquipEffect : MonoBehaviour {

    public CharacterManager.EquipEffect equipEffectType;
    public GameObject activateTarget;
    public float activateConditionNum;

    void Update() {
        if (CharacterManager.Instance && PauseController.Instance && !PauseController.Instance.pauseGame) {
            bool activateFlag = (CharacterManager.Instance.GetEquipEffect(equipEffectType) == activateConditionNum);
            if (activateTarget && activateTarget.activeSelf != activateFlag) {
                activateTarget.SetActive(activateFlag);
            }
        }
    }
}
