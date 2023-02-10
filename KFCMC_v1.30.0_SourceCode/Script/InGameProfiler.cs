using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using System.Text;
using TMPro;

public class InGameProfiler : UIBehaviour {    

    public Canvas profilerCanvas;
    public TMP_Text displayText;

    private StringBuilder sb = new StringBuilder();
    private string m_os_info;
    private string m_cpu_info;
    private string m_gpu_info;
    private string m_resolution_info;
    private string m_audio_info;
    private int m_frame_counts;
    private double m_start_time;
    private float m_fps;
    private const uint mega = 1024 * 1024;
    static readonly string[] stateNames = new string[] { "Spawn", "Wait", "Chase", "Escape", "Dodge", "Attack", "Damage", "Dead", "Jump", "Climb" };

    public void SetProfilerEnabled(bool flag) {
        m_frame_counts = 0;
        m_start_time = GameManager.Instance.unscaledTime;
        profilerCanvas.enabled = flag;
        enabled = flag;
        if (flag) {
            SetText();
        }
    }

    protected override void Start() {
        base.Start();

        m_os_info = string.Format("OS: {0}", SystemInfo.operatingSystem);
        m_cpu_info = string.Format("CPU: {0} / {1}cores", SystemInfo.processorType, SystemInfo.processorCount);
        m_gpu_info = string.Format("GPU: {0} / {1}MB API: {2}", SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize, SystemInfo.graphicsDeviceType);

        AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
        OnAudioConfigurationChanged(true);
        OnRectTransformDimensionsChange();

        m_start_time = GameManager.Instance.time;
    }

    public void SetText() {
        if (displayText != null) {
            int profileType = 0;
            if (GameManager.Instance) {
                profileType = GameManager.Instance.save.config[GameManager.Save.configID_SystemInformation];
            }
            switch (profileType) {
                case 0:
                case 1:
                    sb.Clear();
                    sb.Append(m_os_info);
                    sb.AppendLine();
                    sb.Append(m_cpu_info);
                    sb.AppendLine();
                    sb.Append(m_gpu_info);
                    sb.AppendLine();
                    sb.Append(m_resolution_info);
                    sb.AppendLine();
                    sb.Append(m_audio_info);
                    sb.AppendLine();
                    sb.AppendFormat("Memory: {0:###0.0} / {1}.0MB GCCount: {2}", Profiler.usedHeapSizeLong / (float)mega, SystemInfo.systemMemorySize, System.GC.CollectionCount(0));
                    sb.AppendLine();
                    sb.AppendFormat("Performance: {0:#0.0}fps", m_fps);
                    if (StageManager.Instance) {
                        sb.AppendLine();
                        sb.AppendFormat("Stage: {0} / Floor: {1}", StageManager.Instance.stageNumber, StageManager.Instance.floorNumber + 1);
                    }
                    if (BGM.Instance && Ambient.Instance && LightingDatabase.Instance) {
                        sb.AppendLine();
                        sb.AppendFormat("Music: {0} / Ambient: {1} / Sky: {2}", BGM.Instance.GetPlayingIndex() + 1, Ambient.Instance.GetPlayingIndex() + 1, LightingDatabase.Instance.nowLightingNumber);
                    }
                    displayText.text = sb.ToString();
                    break;
                case 2:
                    if (CharacterManager.Instance && CharacterManager.Instance.pCon) {
                        Vector3 position = CharacterManager.Instance.pCon.transform.position;
                        float rotation = CharacterManager.Instance.pCon.transform.localEulerAngles.y;
                        sb.Clear();
                        sb.Append("State: ").Append(stateNames[CharacterManager.Instance.pCon.GetStateInt]).AppendLine();
                        sb.AppendFormat("Position: {0:0.00}, {1:0.00}, {2:0.00}", position.x, position.y, position.z).AppendLine();
                        sb.AppendFormat("Rotation Y-Axis: {0:0.00}", rotation).AppendLine();
                        sb.AppendFormat("Knock Light: {0:F1} / {1:F1}", CharacterManager.Instance.pCon.GetKnockRemainLight, CharacterManager.Instance.pCon.GetLightKnockEndurance()).AppendLine();
                        sb.AppendFormat("Knock Heavy: {0:F1} / {1:F1}", CharacterManager.Instance.pCon.GetKnockRemainHeavy, CharacterManager.Instance.pCon.GetHeavyKnockEndurance()).AppendLine();
                        sb.AppendFormat("Last Knocked: {0:F1}", CharacterManager.Instance.pCon.GetLastKnocked).AppendLine();
                        sb.AppendFormat("Engage in Attack: {0:F2} / {1:F2}", CharacterManager.Instance.pCon.GetAttackStiffRemain, CharacterManager.Instance.pCon.GetAttackStiffTime).AppendLine();
                        sb.AppendFormat("Attack Interval: {0:F2} / {1:F2}", CharacterManager.Instance.pCon.GetAttackedTimeRemain, CharacterManager.Instance.pCon.GetAttackIntervalSave).AppendLine();
                        displayText.text = sb.ToString();
                    } else {
                        displayText.text = "";
                    }
                    break;
                case 3:
                    if (CharacterManager.Instance && CharacterManager.Instance.playerTargetEnemy) {
                        Vector3 position = CharacterManager.Instance.playerTargetEnemy.transform.position;
                        float rotation = CharacterManager.Instance.playerTargetEnemy.transform.localEulerAngles.y;
                        sb.Clear();
                        sb.Append("Enemy: ").Append(TextManager.Get("CELLIEN_NAME_" + CharacterManager.Instance.playerTargetEnemy.enemyID.ToString("00"))).AppendLine();
                        sb.Append("State: ").Append(stateNames[CharacterManager.Instance.playerTargetEnemy.GetStateInt]).AppendLine();
                        sb.AppendFormat("Position: {0:0.00}, {1:0.00}, {2:0.00}", position.x, position.y, position.z).AppendLine();
                        sb.AppendFormat("Rotation Y-Axis: {0:0.00}", rotation).AppendLine();
                        sb.AppendFormat("Knock Light: {0:F1} / {1:F1}", CharacterManager.Instance.playerTargetEnemy.GetKnockRemainLight, CharacterManager.Instance.playerTargetEnemy.GetLightKnockEndurance()).AppendLine();
                        sb.AppendFormat("Knock Heavy: {0:F1} / {1:F1}", CharacterManager.Instance.playerTargetEnemy.GetKnockRemainHeavy, CharacterManager.Instance.playerTargetEnemy.GetHeavyKnockEndurance()).AppendLine();
                        sb.AppendFormat("Engage in Attack: {0:F2} / {1:F2}", CharacterManager.Instance.playerTargetEnemy.GetAttackStiffRemain, CharacterManager.Instance.playerTargetEnemy.GetAttackStiffTime).AppendLine();
                        sb.AppendFormat("Attack Interval: {0:F2} / {1:F2}", CharacterManager.Instance.playerTargetEnemy.GetAttackedTimeRemain, CharacterManager.Instance.playerTargetEnemy.GetAttackIntervalSave).AppendLine();
                        displayText.text = sb.ToString();
                    } else {
                        displayText.text = "";
                    }
                    break;
            }
        }
    }
    
    void Update() {
        if (GameManager.Instance.save.config[GameManager.Save.configID_SystemInformation] >= 2) {
            SetText();
        } else { 
            ++m_frame_counts;
            double currentTime = GameManager.Instance.unscaledTime;
            double diffTime = currentTime - m_start_time;
            if (diffTime >= 1.0f) {
                m_fps = (float)(m_frame_counts / diffTime);
                m_start_time = currentTime;
                m_frame_counts = 0;
                SetText();
            }
        }
    }

    void OnAudioConfigurationChanged(bool deviceWasChanged) {
        int bufferLength, numBuffers;
        AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
        AudioConfiguration config = AudioSettings.GetConfiguration();
        m_audio_info = string.Format("Audio: {0:#,#}Hz {1} {2}samples {3}buffers", config.sampleRate, config.speakerMode.ToString(), config.dspBufferSize, numBuffers);
    }

    protected override void OnRectTransformDimensionsChange() {
        Resolution reso = Screen.currentResolution;
        m_resolution_info = string.Format("Resolution: {0} x {1} RefreshRate: {2}Hz", reso.width, reso.height, reso.refreshRate);
    }

}