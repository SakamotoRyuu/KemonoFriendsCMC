using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mebiustos.MMD4MecanimFaciem;

public class Event_Ending_CallFriends : MonoBehaviour {

    public bool isNPC;
    public bool isLB;
    public bool isSecret;
    public bool notCheckSave;
    public int characterIndex;
    public int bonusIndex;
    public RuntimeAnimatorController runtimeAnimatorController;
    public string faceName;

    private GameObject characterObj;

    bool CheckCondition() {
        if (notCheckSave) {
            return true;
        } else if (isNPC) {
            return GameManager.Instance.save.inventoryNFS[bonusIndex] != 0;
        } else if (isLB) {
            return GameManager.Instance.save.luckyBeast[bonusIndex] != 0;
        } else if (isSecret) {
            return GameManager.Instance.GetSecret(GameManager.SecretType.SingularityLB);
        } else {
            return GameManager.Instance.save.friends[characterIndex] != 0;
        }
    }

    void Start() {
        CallBody();
    }

    public void CallBody() {
        if (characterObj == null && CharacterDatabase.Instance && CheckCondition()) {
            characterObj = Instantiate(isNPC ? CharacterDatabase.Instance.GetNPC(characterIndex) : CharacterDatabase.Instance.GetFriends(characterIndex), transform);
            characterObj.layer = LayerMask.NameToLayer("Item");
            if (runtimeAnimatorController != null) {
                Animator anim = characterObj.GetComponent<Animator>();
                if (anim) {
                    anim.runtimeAnimatorController = runtimeAnimatorController;
                }
            }
            if (!string.IsNullOrEmpty(faceName)) {
                FaciemController fCon = characterObj.GetComponent<FaciemController>();
                if (fCon) {
                    fCon.SetFace(faceName);
                }
            }
            if (!isNPC) {
                FriendsBase fBase = characterObj.GetComponent<FriendsBase>();
                if (fBase) {
                    fBase.SetForItem();
                    fBase.SetClothEnabled(0);
                }
            }
            HeadLookController headLookController = characterObj.GetComponent<HeadLookController>();
            if (headLookController) {
                headLookController.enabled = false;
            }
            DynamicBone[] dynamicBones = characterObj.GetComponents<DynamicBone>();
            bool boneFlag = false;
            if (GameManager.Instance && GameManager.Instance.save.config[GameManager.Save.configID_DynamicBone] != 0){
                boneFlag = true;
            }
            for (int i = 0; i < dynamicBones.Length; i++) {
                dynamicBones[i].enabled = boneFlag;
            }
        }
    }

}
