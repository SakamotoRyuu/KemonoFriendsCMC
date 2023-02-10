using UnityEngine;

public class ActivateOnSpecificStage : MonoBehaviour {

    public int specificStageNumber;
    public GameObject[] activateTarget;
    public GameObject[] deactivateTarget;

    void Start() {
        if (StageManager.Instance) {
            bool isSpecificStage = (StageManager.Instance.stageNumber == specificStageNumber);
            for (int i = 0; i < activateTarget.Length; i++) {
                if (activateTarget[i] && activateTarget[i].activeSelf != isSpecificStage) {
                    activateTarget[i].SetActive(isSpecificStage);
                }
            }
            for (int i = 0; i < deactivateTarget.Length; i++) {
                if (deactivateTarget[i] && deactivateTarget[i].activeSelf != !isSpecificStage) {
                    deactivateTarget[i].SetActive(!isSpecificStage);
                }
            }
        }
    }
}
