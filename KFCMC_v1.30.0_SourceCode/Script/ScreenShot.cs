using UnityEngine;
using Rewired;
using System.Text;
using System.Collections;

public class ScreenShot : MonoBehaviour {

    Player playerInput;
    StringBuilder sb = new StringBuilder();
    int count = 0;
    string filenameSave = "";
    string filename = "";
    bool excluding;
    int quality = 100;

    private void Start() {
        if (GameManager.Instance) {
            playerInput = GameManager.Instance.playerInput;
        } else {
            playerInput = ReInput.players.GetPlayer(0);
        }
    }
    
    IEnumerator CaptureJPG() {

        yield return new WaitForEndOfFrame();

        var texture = ScreenCapture.CaptureScreenshotAsTexture();        
        byte[] fileData = texture.EncodeToJPG(quality);
        System.IO.File.WriteAllBytes(filename, fileData);        
        Destroy(texture);
        
    }

    void Capture() {
        if (PauseController.Instance && PauseController.Instance.IsPhotoPausing) {
            excluding = true;
            PauseController.Instance.ExcludeFromPhoto(true);
        }
        if (!System.IO.Directory.Exists(Application.dataPath + "/ScreenShot")) {
            System.IO.Directory.CreateDirectory(Application.dataPath + "/ScreenShot");
        }
        sb.Clear();
        sb.Append(Application.dataPath).Append("/ScreenShot/").Append(System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        if (sb.ToString() == filenameSave) {
            count++;
            sb.Append(" (").Append(count).Append(")");
        } else {
            count = 0;
            filenameSave = sb.ToString();
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_ScreenShotFileFormat] == 0) {
            sb.Append(".png");
            filename = sb.ToString();
            ScreenCapture.CaptureScreenshot(filename);
        } else {
            sb.Append(".jpg");
            filename = sb.ToString();
            quality = GameManager.Instance.save.config[GameManager.Save.configID_ScreenShotFileFormat] == 1 ? 100 : GameManager.Instance.save.config[GameManager.Save.configID_ScreenShotFileFormat] == 2 ? 90 : 75;
            StartCoroutine(CaptureJPG());
        }
        if (UISE.Instance) {
            UISE.Instance.Play(UISE.SoundName.camera);
        }
    }

    private void LateUpdate() {
        if (excluding && PauseController.Instance) {
            excluding = false;
            PauseController.Instance.ExcludeFromPhoto(false);
        }
        if (playerInput.GetButtonDown(RewiredConsts.Action.Screen_Capture)) {
            if (PauseController.Instance && PauseController.Instance.GetCanPhotoMode) {
                PauseController.Instance.PausePhoto(true);
            } else {
                Capture();
            }
        }
    }

}
