using UnityEngine;
using Mebiustos.MMD4MecanimFaciem;

public class SetFaceOnEnable : MonoBehaviour {

    public FaciemController faciemController;
    public string faceName;

    private void OnEnable() {
        if (faciemController) {
            faciemController.SetFace(faceName);
        }
    }

}
