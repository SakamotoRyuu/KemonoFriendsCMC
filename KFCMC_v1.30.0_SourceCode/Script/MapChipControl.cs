using UnityEngine;

public class MapChipControl : MonoBehaviour {

    public Renderer chipRenderer;
    public bool setToDungeonController;
    public bool isStatic;
    public float radius;
    public bool temporaryFlag;

    private void Start() {
        if (setToDungeonController && StageManager.Instance && StageManager.Instance.dungeonController) {
            StageManager.Instance.dungeonController.SetAdditionalMapChip(this);
        }
    }

}
