using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Talk : MonoBehaviour
{

    [System.Serializable]
    public class MessageData
    {
        public string textKey;
        public int faceIndex;
        public MessageBackColor backColor;
    }

    public float delayTime;
    public bool waitForSceneChange;
    public MessageData[] messageDatas;
    private int progress;
    private float elapsedTime;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= delayTime && (!waitForSceneChange || !SceneChange.Instance.GetIsProcessing) && progress < messageDatas.Length && MessageUI.Instance && MessageUI.Instance.GetMessageCount(MessageUI.panelType_Speech) == 0)
        {
            MessageData data = messageDatas[progress];
            string speechContent = TextManager.Get(data.textKey);
            MessageUI.Instance.SetMessageOptional(speechContent, data.backColor.color1, data.backColor.color2, data.backColor.twoToneType, data.faceIndex, -1, 1, 1, true);
            MessageUI.Instance.SetMessageLog(speechContent, data.faceIndex);
            progress++;
            if (progress >= messageDatas.Length)
            {
                enabled = false;
            }
        }
    }
}
