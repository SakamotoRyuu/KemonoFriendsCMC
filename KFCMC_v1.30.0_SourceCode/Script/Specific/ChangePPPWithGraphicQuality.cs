using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class ChangePPPWithGraphicQuality : MonoBehaviour
{

    public PostProcessingBehaviour ppBehaviour;
    public PostProcessingProfile lqProfile;

    private void Awake() {
        if (QualitySettings.GetQualityLevel() <= 0) {
            ppBehaviour.profile = lqProfile;
        }
    }

}
