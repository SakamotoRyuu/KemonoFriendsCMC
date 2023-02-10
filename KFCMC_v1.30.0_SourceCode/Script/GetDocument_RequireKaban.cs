using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDocument_RequireKaban : GetDocument
{

    public Renderer changeRenderer;
    public int changeIndex;
    public Material defaultMaterial;
    public Material emissionMaterial;
    public AudioSource startSfx;
    public AudioSource pageSfx;
    public bool setSecretEnabled;
    public GameManager.SecretType secretType;

    private bool existKaban = false;
    private bool existSave = false;

    protected override bool ActivateCondition_Balloon() {
        if (existKaban) {
            return base.ActivateCondition_Balloon();
        } else {
            return false;
        }
    }

    protected override bool ActivateCondition_Get() {
        if (existKaban) {
            return base.ActivateCondition_Get();
        } else {
            return false;
        }
    }

    protected override void PlaySE() {
        if (startSfx) {
            startSfx.Play();
        } else {
            base.PlaySE();
        }
    }

    protected override void SetDocument() {
        base.SetDocument();
        if (setSecretEnabled) {
            GameManager.Instance.SetSecret(secretType);
        }
        if (PauseController.Instance) {
            PauseController.Instance.SetSpecialPageSFX(pageSfx);
        }
    }

    protected override void Update() {
        if (CharacterManager.Instance) {
            existKaban = CharacterManager.Instance.GetFriendsExist(1, true);
            if (existKaban != existSave) {
                existSave = existKaban;
                Material[] mats = changeRenderer.materials;
                if (existKaban) {
                    mats[changeIndex] = emissionMaterial;
                    mats[changeIndex].renderQueue = defaultMaterial.renderQueue;
                } else {
                    mats[changeIndex] = defaultMaterial;
                }
                changeRenderer.materials = mats;
            }
        }
        base.Update();
    }


}
