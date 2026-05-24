using UnityEngine;

public class SpecialMapChipControl : MonoBehaviour {

    public Renderer[] chipRenderers;
    bool nowActive;

    private void Update() {
        bool toActive = (StageManager.Instance && StageManager.Instance.mapActivateFlag != 0);
        if (nowActive != toActive) {
            nowActive = toActive;
            for (int i = 0; i < chipRenderers.Length; i++) {
                if (chipRenderers[i]) {
                    chipRenderers[i].enabled = toActive;
                }
            }
        }
    }

}
