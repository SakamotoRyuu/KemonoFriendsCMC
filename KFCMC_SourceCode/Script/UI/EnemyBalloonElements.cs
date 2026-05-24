using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBalloonElements : MonoBehaviour
{

    public GameObject PaperPlane;
    public GameObject IbisSong;
    public GameObject MargayVoice;

    public void SetActiveAll(bool value)
    {
        PaperPlane.SetActive(value);
        IbisSong.SetActive(value);
        MargayVoice.SetActive(value);
    }
}
