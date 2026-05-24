using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationCanvasOverlayEnemyHP : UIOperationCanvasOverlay
{
    protected override void LateUpdate()
    {
        if (GameManager.Instance.save.config[GameManager.Save.configID_ShowEnemyHp] == 0 && canvas.enabled)
        {
            canvas.enabled = false;
        }
        else
        {
            base.LateUpdate();
        }
    }
}
