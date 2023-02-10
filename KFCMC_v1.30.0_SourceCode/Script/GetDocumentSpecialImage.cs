using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDocumentSpecialImage : GetDocument {

    public Sprite specialImage;
    public int specialPageNum;

    protected override void SetDocument() {
        PauseController.Instance.SetDocumentSpecialImage(specialImage, specialPageNum, background, backgroundSize, sb.Clear().Append(textHeader).Append(id.ToString("00")).Append("_").ToString(), numPages, darkFilter);
    }

    public void SetDocumentExternal() {
        SetDocument();
    }

}
