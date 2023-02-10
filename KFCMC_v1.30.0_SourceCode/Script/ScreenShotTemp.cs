using System.Collections;
using UnityEngine;
using System.IO;
using System;

public class ScreenShotTemp : MonoBehaviour {
    
    void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift)) {
            ScreenCapture.CaptureScreenshot(sb.Clear().Append(Application.dataPath).Append("/ScreenShot/").Append(System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")).Append(".png").ToString());
        }
        */
        if (Input.GetKeyDown(KeyCode.G) && Input.GetKey(KeyCode.LeftShift)) {
            StartCoroutine(CaptureWithAlpha());
        }
    }

    IEnumerator CaptureWithAlpha() {
        yield return new WaitForEndOfFrame();

        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        var width = tex.width;
        var height = tex.height;
        var texAlpha = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Read screen contents into the texture
        texAlpha.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texAlpha.Apply();

        // Encode texture into PNG
        var bytes = texAlpha.EncodeToPNG();
        Destroy(tex);
        File.WriteAllBytes(Application.streamingAssetsPath + "/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png", bytes);
    }

}
