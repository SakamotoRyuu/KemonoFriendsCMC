using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPlayerSelect : ConsoleBase
{

    int playerIndexSave;

    protected override void CancelOnGameover()
    {
        if (PauseController.Instance)
        {
            PauseController.Instance.PausePlayerSelect(false);
        }
        state = 0;
    }

    protected override void ConsoleStart()
    {
        if (PauseController.Instance && CharacterManager.Instance)
        {
            playerIndexSave = CharacterManager.Instance.playerIndex;
            PauseController.Instance.PausePlayerSelect(true);
        }
    }

    protected override void ConsoleUpdate()
    {
        base.ConsoleUpdate();
    }

    protected override void ConsoleEnd()
    {
        if (CharacterManager.Instance && playerIndexSave != CharacterManager.Instance.playerIndex)
        {
            CharacterManager.Instance.EmitEffect((int)EffectDatabase.id.sacrifice, -1, true, true, false);
            CharacterManager.Instance.pCon.ForceStopForEvent(1f);
            coolTime = 1f;
        }
    }

}
