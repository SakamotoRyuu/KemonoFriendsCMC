using System.Collections;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class ScreenShotTemp : MonoBehaviour {

    StringBuilder sb = new StringBuilder();

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
        if (!System.IO.Directory.Exists(Application.dataPath + "/ScreenShot"))
        {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/ScreenShot");
        }
        sb.Clear();
        sb.Append(Application.dataPath).Append("/ScreenShot/").Append(System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")).Append(".png");
        File.WriteAllBytes(sb.ToString(), bytes);
        Destroy(texAlpha);
        texAlpha = null;
    }

}
