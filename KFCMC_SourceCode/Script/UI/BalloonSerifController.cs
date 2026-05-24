using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BalloonSerifController : MonoBehaviour {

    public Canvas canvas;
    public RectTransform panel;
    public RectTransform frame;
    public RectTransform backSimple;
    public RectTransform backLeft;
    public RectTransform backRight;
    public Sprite[] leftSprite;
    public Sprite[] rightSprite;
    public TMP_Text uiText;
    public Transform followTarget;
    public Vector3 offset;
    public float scaleTime = 0.1f;
    public AudioSource aSrc;
    public VisibleChecker visibleChecker;
    
    protected float duration;
    protected float displayTime = 2.5f;
    protected float distRateSave = -1;
    protected Image frameImage;
    protected Image backSimpleImage;
    protected Image backLeftImage;
    protected Image backRightImage;
    protected Camera mainCamera;
    protected Transform camT;
    protected const float scaleStart = 0.1f;
    protected const float scaleMinDist = 6f;
    protected const float scaleMaxDist = 40f;
    protected const float distRateMin = 0.25f;

    protected Vector2 frameSizeOffset = new Vector2(40f, 30f);
    protected Vector2 frameSizeRate = new Vector2(1f, 1.2f);
    protected Vector2 frameSizeMax = new Vector2(340f, 140f);
    protected static readonly Vector3 vecOne = Vector3.one;    

    public void SetText(string text, float displayTime = 3, bool twoTone = true, bool sfx = true) {
        uiText.text = text;
        Vector2 size = new Vector2(Mathf.Min(uiText.preferredWidth * frameSizeRate.x + frameSizeOffset.x, frameSizeMax.x), Mathf.Min(uiText.preferredHeight * frameSizeRate.y + frameSizeOffset.y, frameSizeMax.y));
        frame.sizeDelta = size;
        backSimple.sizeDelta = size;
        backLeft.sizeDelta = size;
        backRight.sizeDelta = size;
        backSimpleImage.enabled = !twoTone;
        backLeftImage.enabled = twoTone;
        backRightImage.enabled = twoTone;
        panel.localScale = Vector3.one * scaleStart;
        duration = 0;
        distRateSave = -1;
        this.displayTime = displayTime;
        if (sfx && aSrc && CharacterManager.Instance && CharacterManager.Instance.messageSoundCount == 0) {
            aSrc.Play();
            CharacterManager.Instance.messageSoundCount = 1;
        }
    }

    public void SetFrameColor(Color color1, Color color2, int twoToneType, float frameColorAlpha = 0.7f, float backColorAlpha = 0.5f) {
        frameImage = frame.GetComponent<Image>();
        backSimpleImage = backSimple.GetComponent<Image>();
        backLeftImage = backLeft.GetComponent<Image>();
        backRightImage = backRight.GetComponent<Image>();
        if (twoToneType >= 0 && twoToneType < leftSprite.Length && twoToneType < rightSprite.Length) {
            backLeftImage.sprite = leftSprite[twoToneType];
            backRightImage.sprite = rightSprite[twoToneType];
        }
        Color colorFrameTemp = color1;
        Color color1Temp = color1;
        Color color2Temp = color2;
        colorFrameTemp.a = frameColorAlpha;
        color1Temp.a = color2Temp.a = backColorAlpha;
        frameImage.color = colorFrameTemp;
        backSimpleImage.color = backLeftImage.color = color1Temp;
        backRightImage.color = color2Temp;
    }

    protected virtual float GetDistanceRate() {
        if (camT) {
            float distTemp = Vector3.Distance(camT.position, followTarget.position + offset);
            if (distTemp < scaleMinDist) {
                return 1f;
            } else if (distTemp > scaleMaxDist) {
                return distRateMin;
            } else {
                return Mathf.Lerp(1f, distRateMin, (distTemp - scaleMinDist) / (scaleMaxDist - scaleMinDist));
            }
        } else {
            return 1f;
        }
    }    

    protected void Awake() {
        duration = 0;
        displayTime = 0;
        canvas.enabled = false;
    }

    protected void Start() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetMainCameraTransform(ref camT);
            CameraManager.Instance.SetMainCamera(ref mainCamera);
        } else {
            Camera camTemp = Camera.main;
            if (camTemp) {
                mainCamera = camTemp;
                camT = camTemp.transform;
            }
        }
    }

    protected virtual void UpdateScale() { 
        float distRate = GetDistanceRate();
        if (duration < scaleTime) {
            panel.localScale = distRate * ((1f - scaleStart) * (duration / scaleTime) + scaleStart) * vecOne;
        } else {
            if (distRateSave != distRate) {
                panel.localScale = distRate * vecOne;
                distRateSave = distRate;
            }
        }
    }

    protected virtual void CheckVisible() {
        if (duration < displayTime && visibleChecker.isVisible) {
            if (!canvas.enabled) {
                canvas.enabled = true;
            }
            panel.position = RectTransformUtility.WorldToScreenPoint(mainCamera, followTarget.position + offset);
        } else {
            if (canvas.enabled) {
                canvas.enabled = false;
            }
        }
    }

    protected virtual void Update() {
        if (mainCamera && duration < displayTime) {
            if (displayTime < 1000) {
                duration += Time.deltaTime;
            }
            CheckVisible();
            UpdateScale();
        }
    }    

}
