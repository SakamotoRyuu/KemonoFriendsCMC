using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCharacter_Tsuchinoko : ItemCharacter {

    float comboTimeRemain;
    int comboCount;
    int[] randomTalkArray = new int[] { 2, 3, 4, 5, 6, 7, 8, 9 };

    protected override void HomeUpdate() {
        base.HomeUpdate();
        if (!isHomeAp) {
            if (comboTimeRemain > 0) {
                comboTimeRemain -= Time.deltaTime;
            }
            if (comboCount >= 1 && comboTimeRemain <= 0) {
                if (comboCount >= 3) {
                    MessageUI.Instance.DestroyMessage();
                    homeTalkIndex = 10;
                    homeTalkOption = sb.Clear().AppendFormat("_{0:00}", homeTalkIndex).ToString();
                    string speechContent = TextManager.Get(sb.Clear().Append(talkHeader).Append(talkName).Append(homeTalk).Append(homeTalkOption).ToString());
                    lookTimeRemain = MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length);
                    MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex, lookTimeRemain, 1, 1, true);
                    MessageUI.Instance.SetMessageLog(speechContent, faceIndex);
                    Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.talk], trans.position, Quaternion.identity);
                    homeTalkIndex = 0;
                    comboCount = 0;
                } else {
                    homeTalkOption = sb.Clear().AppendFormat("_{0:00}", homeTalkIndex).ToString();
                    string speechContent = TextManager.Get(sb.Clear().Append(talkHeader).Append(talkName).Append(homeTalk).Append(homeTalkOption).ToString());
                    lookTimeRemain = MessageUI.Instance.GetSpeechAppropriateTime(speechContent.Length);
                    MessageUI.Instance.SetMessageOptional(speechContent, mesBackColor.color1, mesBackColor.color2, mesBackColor.twoToneType, faceIndex, lookTimeRemain, 1, 1, true);
                    MessageUI.Instance.SetMessageLog(speechContent, faceIndex);
                    Instantiate(EffectDatabase.Instance.prefab[(int)EffectDatabase.id.talk], trans.position, Quaternion.identity);
                    homeTalkIndex = randomTalkArray[comboCount];
                    comboTimeRemain = 1;
                    comboCount++;
                }
            }
        }
    }

    protected override void SetHomeTalkIndex() {
        if (isHomeAp) {
            base.SetHomeTalkIndex();
        } else {
            if (homeTalkIndex == 0) {
                homeTalkIndex = 1;
            } else if (homeTalkIndex == 1) {
                for (int i = randomTalkArray.Length - 1; i > 0; i--) {
                    int r = Random.Range(0, i + 1);
                    int temp = randomTalkArray[i];
                    randomTalkArray[i] = randomTalkArray[r];
                    randomTalkArray[r] = temp;
                }
                homeTalkIndex = randomTalkArray[0];
                comboTimeRemain = 1;
                comboCount = 1;
            }
        }
    }

}
