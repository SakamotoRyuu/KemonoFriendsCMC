using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDocumentFragment : GetDocument {

    public GameObject findEffectPrefab;
    public GameObject activateTarget;
    public int specificID = -1;

    bool isHome;

    protected override void Start() {
        base.Start();
        isHome = (StageManager.Instance && StageManager.Instance.IsHomeStage);
    }

    protected override void FriendsComment() {
        if (MessageUI.Instance && CharacterManager.Instance) {
            if (CharacterManager.Instance.GetFriendsExist(talkConditionID, true)) {
                sb.Clear().Append(talkHeader).Append(id.ToString("00"));
                if (StageManager.Instance.IsHomeStage && (GameManager.Instance.save.document[id] & GameManager.documentFragmentCondition) == GameManager.documentFragmentCondition) {
                    sb.Append("_COMPLETE");
                }
                MessageUI.Instance.SetMessageOptional(TextManager.Get(sb.ToString()), MessageUI.Instance.preparedColors[0], MessageUI.Instance.preparedColors[1], 0, faceIndex, -1, 1, 1, true);
                MessageUI.Instance.SetMessageLog(TextManager.Get(sb.ToString()), faceIndex);
            }
            if (CharacterManager.Instance.GetFriendsExist(talkConditionIDEX, true)) {
                sb.Clear().Append(talkHeaderEX).Append(id.ToString("00"));
                if (StageManager.Instance.IsHomeStage && (GameManager.Instance.save.document[id] & GameManager.documentFragmentCondition) == GameManager.documentFragmentCondition) {
                    sb.Append("_COMPLETE");
                }
                MessageUI.Instance.SetMessageOptional(TextManager.Get(sb.ToString()), MessageUI.Instance.preparedColors[2], MessageUI.Instance.preparedColors[3], 0, faceIndexEX, -1, 1, 1, true);
                MessageUI.Instance.SetMessageLog(TextManager.Get(sb.ToString()), faceIndexEX);
            }
        }
    }

    protected override void SaveDocument() {
        int idTemp = (specificID >= 1 ? specificID : stageId);
        if (idTemp >= 1) {
            int calcTemp = GameManager.Instance.save.document[id];
            calcTemp = calcTemp | (1 << (idTemp - 1));
            GameManager.Instance.save.document[id] = calcTemp;
        }
    }

    protected override void SetDocument() {
        if (isHome) {
            PauseController.Instance.SetDocumentFragment(background, backgroundSize, sb.Clear().Append(textHeader).Append(id.ToString("00")).Append("_").ToString(), numPages, darkFilter, id);
        } else {
            PauseController.Instance.SetDocumentFragment(background, backgroundSize, sb.Clear().Append(textHeader).Append(id.ToString("00")).Append("_").ToString(), 2, darkFilter, id, specificID);
        }
    }
    
}
