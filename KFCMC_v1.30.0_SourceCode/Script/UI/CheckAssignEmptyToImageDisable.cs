using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckAssignEmptyToImageDisable : MonoBehaviour {

    public enum CheckType { None, Kinovori, Dodge, PileAttack, SpinAttack, GatherFriends, WildRelease, JustDodge, WaveAttack, ScrewAttack, BoltAttack, Judgement };

    public Sprite assignEmptySprite;
    public Image[] images;
    public TMP_Text assignErrorText;
    public CheckType checkType;
    int configError = -1;
    int equipmentError = -1;
    int configTarget;
    int textState;

    private void Start() {
        switch (checkType) {
            case CheckType.None:
                break;
            case CheckType.Kinovori:
                if (GameManager.Instance.save.config[GameManager.Save.configID_TreeClimbingAction] == 0) {
                    configError = GameManager.Save.configID_TreeClimbingAction;
                    configTarget = 1;
                }
                break;
            case CheckType.Dodge:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Dodge) == 0) {
                    equipmentError = GameManager.equipmentID_Dodge;
                } else if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Escape) == 0) {
                    equipmentError = GameManager.equipmentID_Escape;
                }
                break;
            case CheckType.PileAttack:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Pile) == 0) {
                    equipmentError = GameManager.equipmentID_Pile;
                }
                break;
            case CheckType.SpinAttack:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Spin) == 0) {
                    equipmentError = GameManager.equipmentID_Spin;
                }
                break;
            case CheckType.GatherFriends:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Gather) == 0) {
                    equipmentError = GameManager.equipmentID_Gather;
                }
                break;
            case CheckType.WildRelease:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.WildRelease) == 0) {
                    equipmentError = GameManager.sandstarCondition;
                }
                break;
            case CheckType.JustDodge:
                if (GameManager.Instance.save.config[GameManager.Save.configID_DisableJustDodge] != 0) {
                    configError = GameManager.Save.configID_DisableJustDodge;
                    configTarget = 0;
                }
                break;
            case CheckType.WaveAttack:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Wave) == 0) {
                    equipmentError = GameManager.equipmentID_Wave;
                }
                break;
            case CheckType.ScrewAttack:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Screw) == 0) {
                    equipmentError = GameManager.equipmentID_Screw;
                }
                break;
            case CheckType.BoltAttack:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Bolt) == 0) {
                    equipmentError = GameManager.equipmentID_Bolt;
                }
                break;
            case CheckType.Judgement:
                if (CharacterManager.Instance.GetEquipEffect(CharacterManager.EquipEffect.Judgement) == 0) {
                    equipmentError = GameManager.equipmentID_Judgement;
                }
                break;
        }

        if (assignErrorText) {
            assignErrorText.enabled = false;
        }
    }

    private void LateUpdate() {
        bool assignFilled = true;
        bool toEnabled = false;
        for (int i = 0; i < images.Length; i++) {
            if (images[i].sprite == assignEmptySprite) {
                assignFilled = false;
                break;
            }
        }
        for (int i = 0; i < images.Length; i++) {
            if (images[i].enabled != assignFilled) {
                images[i].enabled = assignFilled;
            }
        }
        if (assignFilled == false) {
            toEnabled = true;
            if (textState != 1) {
                assignErrorText.text = TextManager.Get("CONTROL_ASSIGNERROR");
                textState = 1;
            }
        } else if (configError >= 0) {
            toEnabled = true;
            if (textState != 2) {
                assignErrorText.text = string.Format(TextManager.Get("CONTROL_CONFIGERROR"), PauseController.Instance.GetConfigNameWithIndex(configError), TextManager.Get(configTarget == 0 ? "CONFIG_OFF" : "CONFIG_ON"));
                textState = 2;
            }
        } else if (equipmentError >= 0) {
            toEnabled = true;
            if (textState != 3) {
                assignErrorText.text = string.Format(TextManager.Get("CONTROL_EQUIPMENTERROR"), TextManager.Get("ITEM_NAME_" + (200 + equipmentError)));
                textState = 3;
            }
        } else {
            toEnabled = false;
            textState = 0;
        }

        if (assignErrorText && assignErrorText.enabled != toEnabled) {
            assignErrorText.enabled = toEnabled;
        }
    }
}
