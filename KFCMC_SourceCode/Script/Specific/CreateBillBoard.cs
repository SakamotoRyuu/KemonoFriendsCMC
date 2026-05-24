using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CreateBillBoard : MonoBehaviour {

    public Transform rotationPivot;
    public string filename;
    public bool Alpha;

    bool captured;
    int captureCount;
    float elapsedTime;
    StringBuilder sb = new StringBuilder();

    // Update is called once per frame
    void Update() {
        if (!captured) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 1f) {
                captured = true;
                captureCount = 1;
            }
        } else if (captureCount <= 8) {
            switch (captureCount) {
                case 1:
                    rotationPivot.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case 2:
                    rotationPivot.localEulerAngles = new Vector3(0, -45, 0);
                    break;
                case 3:
                    rotationPivot.localEulerAngles = new Vector3(0, -90, 0);
                    break;
                case 4:
                    rotationPivot.localEulerAngles = new Vector3(0, -135, 0);
                    break;
                case 5:
                    rotationPivot.localEulerAngles = new Vector3(0, 180, 0);
                    break;
                case 6:
                    rotationPivot.localEulerAngles = new Vector3(0, 135, 0);
                    break;
                case 7:
                    rotationPivot.localEulerAngles = new Vector3(0, 90, 0);
                    break;
                case 8:
                    rotationPivot.localEulerAngles = new Vector3(0, 45, 0);
                    break;
            }
            StartCoroutine(CaptureWithAlpha());
            captureCount++;
        }
    }

    void Capture(int count) {
        /*
        if (!Directory.Exists(Application.dataPath + "/ScreenShot")) {
            Directory.CreateDirectory(Application.dataPath + "/ScreenShot");
        }
        sb.Clear();
        sb.Append(Application.dataPath).Append("/ScreenShot/").Append(filename).Append(count).Append(".png");
        ScreenCapture.CaptureScreenshot(sb.ToString());
        */
    }

    IEnumerator CaptureWithAlpha() {
        yield return new WaitForEndOfFrame();
        
        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        var width = tex.width;
        var height = tex.height;
        var texNoAlpha = new Texture2D(width, height, TextureFormat.RGB24, false);
        var texAlpha = new Texture2D(width, height, TextureFormat.ARGB32, false);

        if (Alpha) {
            // Read screen contents into the texture
            texAlpha.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texAlpha.Apply();
        } else {
            // Read screen contents into the texture
            texNoAlpha.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texNoAlpha.Apply();
        }

        var bytes = texAlpha.EncodeToPNG(); ;
        if (!Alpha) {
            bytes = texNoAlpha.EncodeToPNG();
        }
        Destroy(tex);
        sb.Clear(); 
        sb.Append(Application.streamingAssetsPath).Append("/").Append(filename).Append(captureCount - 1).Append(".png");
        Debug.Log(sb.ToString());
        File.WriteAllBytes(sb.ToString(), bytes);
    }

}
