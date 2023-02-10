/// 作者 yagero 様
/// https://gist.github.com/yagero/d4b377a4fa318b01f6b6e9c5a533141b

using UnityEngine;
using UnityEngine.PostProcessing;
using System.Collections.Generic;

/// <summary>
/// Use this component to dynamically create a PostProcessingBehaviour and instantiate a PostProcessingProfile on a Camera
/// This allows you to dynamically modify at runtime the PostProcessingProfile, without modifying the asset.
/// This component keeps track of the Profile and Instances. This means that if 2 different camera use the same Profile, they will use the same Instance.
/// </summary>
[RequireComponent(typeof(Camera))]
public class InstantiatePostProcessingProfile : SingletonMonoBehaviour<InstantiatePostProcessingProfile> {
    [SerializeField]
    PostProcessingProfile m_Profile = null;
    PostProcessingBehaviour ppb;
    PostProcessingProfile profile;

    [System.NonSerialized]
    public bool isChanged;
    
    static Dictionary<PostProcessingProfile, PostProcessingProfile> ms_RefToInstance = new Dictionary<PostProcessingProfile, PostProcessingProfile>();
    static PostProcessingProfile AssignProfile(PostProcessingProfile reference) {
        if (!reference)
            return null;

        // keep track of the profile and instances: only 1 instance is created per profile
        // (event if multiple cameras share 1 profile)
        if (!ms_RefToInstance.ContainsKey(reference)) {
            var profileInstance = Object.Instantiate(reference);
            //QualitySettingsAdjustments(profileInstance);
            ms_RefToInstance.Add(reference, profileInstance);
        }

        return ms_RefToInstance[reference];
    }

    T GetOrAddComponent<T>() where T : Component {
        var component = gameObject.GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();
        return component;
    }

    protected override void Awake() {
        if (m_Profile) {
            ppb = GetOrAddComponent<PostProcessingBehaviour>();
            ppb.profile = AssignProfile(m_Profile);
            profile = ppb.profile;
        }
    }

    private void Start() {
        QualitySettingsAdjustments();
    }

    public void QualitySettingsAdjustments() {
        isChanged = true;
        if (GameManager.Instance.save.config[GameManager.Save.configID_Antialiasing] == 0) {
           profile.antialiasing.enabled = false;
        } else {
            var am_settings = profile.antialiasing.settings;
            if (GameManager.Instance.save.config[GameManager.Save.configID_Antialiasing] == 1) {
                am_settings.method = AntialiasingModel.Method.Fxaa;
            } else {
                am_settings.method = AntialiasingModel.Method.Taa;
            }
            profile.antialiasing.settings = am_settings;
            profile.antialiasing.enabled = true;
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_AmbientOcclusion] == 0) {
            profile.ambientOcclusion.enabled = false;
        } else {
            var aom_settings = profile.ambientOcclusion.settings;
            if (GameManager.Instance.save.config[GameManager.Save.configID_AmbientOcclusion] == 1) {
                aom_settings.sampleCount = AmbientOcclusionModel.SampleCount.Lowest;
            } else {
                aom_settings.sampleCount = AmbientOcclusionModel.SampleCount.Low;
            }
            profile.ambientOcclusion.settings = aom_settings;
            profile.ambientOcclusion.enabled = true;
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] == 0) {
            profile.depthOfField.enabled = false;
        } else {
            var dofm_settings = profile.depthOfField.settings;
            if (GameManager.Instance.save.config[GameManager.Save.configID_DepthOfField] == 1) {
                dofm_settings.kernelSize = DepthOfFieldModel.KernelSize.Small;
            } else {
                dofm_settings.kernelSize = DepthOfFieldModel.KernelSize.Medium;
            }
            profile.depthOfField.settings = dofm_settings;
            profile.depthOfField.enabled = true;
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_MotionBlur] == 0) {
            profile.motionBlur.enabled = false;
        } else {
            var mbm_settings = profile.motionBlur.settings;
            if (GameManager.Instance.save.config[GameManager.Save.configID_MotionBlur] == 1) {
                mbm_settings.sampleCount = 6;
            } else {
                mbm_settings.sampleCount = 10;
            }
            profile.motionBlur.settings = mbm_settings;
            profile.motionBlur.enabled = true;
        }        
        if (GameManager.Instance.save.config[GameManager.Save.configID_Bloom] == 0) {
            profile.bloom.enabled = false;
        } else {
            var bloom_settings = profile.bloom.settings;
            bloom_settings.bloom.radius = Mathf.Clamp(2.5f + GameManager.Instance.save.config[GameManager.Save.configID_Bloom] * 1.5f, 0f, 7f);
            profile.bloom.settings = bloom_settings;
            profile.bloom.enabled = true;
        }
        if (GameManager.Instance.save.config[GameManager.Save.configID_ColorGrading] == 0) {
            profile.colorGrading.enabled = false;
        } else {
            var color_settings = profile.colorGrading.settings;
            color_settings.basic.postExposure = GameManager.Instance.save.config[GameManager.Save.configID_Brightness] * 0.1f;
            profile.colorGrading.settings = color_settings;
            profile.colorGrading.enabled = true;
        }
    }

    public PostProcessingProfile GetProfile() {
        return profile;
    }

}
