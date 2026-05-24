using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class ChangeImageColorSyncVideo : MonoBehaviour
{

    [System.Serializable]
    public class ColorTimeSet {
        public Color color;
        public double time;
    }

    [System.Serializable]
    public class SyncImageSet {
        public Image image;
        public ColorTimeSet[] sequence;
    }

    [System.Serializable]
    public class SpriteTimeSet {
        public Sprite[] spriteLang;
        public double time;
    }


    public VideoPlayer videoPlayer;
    public double nonVideoLoopTime;
    public SyncImageSet[] syncImageSets;
    public Image animatedImage;
    public SpriteTimeSet[] spriteTimeSets;

    double videoTime;
    int pointerSave = -2;

    private void LateUpdate() {
        if (videoPlayer) {
            videoTime = videoPlayer.time;
        } else {
            videoTime += Time.unscaledDeltaTime;
            if (videoTime >= nonVideoLoopTime) {
                videoTime -= nonVideoLoopTime;
            }
        }
        for (int i = 0; i < syncImageSets.Length; i++) {
            for (int s = syncImageSets[i].sequence.Length - 1; s >= 0; s--) {
                if (syncImageSets[i].sequence[s].time <= videoTime) {
                    syncImageSets[i].image.color = syncImageSets[i].sequence[s].color;
                    break;
                }
            }
        }
        if (animatedImage && GameManager.Instance) {
            int lang = GameManager.Instance.save.language;
            if (lang < 0 || lang >= GameManager.languageMax) {
                lang = GameManager.languageEnglish;
            }
            int pointer = -1;
            for (int i = spriteTimeSets.Length - 1; i >= 0; i--) {
                if (videoTime >= spriteTimeSets[i].time) {
                    pointer = i;
                    break;
                }
            }
            if (pointer != pointerSave) {
                pointerSave = pointer;
                if (pointer >= 0 && spriteTimeSets[pointer].spriteLang.Length > 0) {
                    animatedImage.sprite = spriteTimeSets[pointer].spriteLang[Mathf.Clamp(lang, 0, spriteTimeSets[pointer].spriteLang.Length - 1)];
                }
            }
        }

    }

}
