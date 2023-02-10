using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class QuakeOneShot : MonoBehaviour {

    public float amplitude = 10f;
    public float frequency = 4f;
    public float attackTime = 0f;
    public float sustainTime = 0f;
    public float decayTime = 1f;
    public float impactRadius = 1f;
    public float dissipationDistance = 100f;
    public bool cancelQuakeOnDisable;
    public CinemachineImpulseSource impulseSource;

    private void Start() {
        if (CameraManager.Instance) {
            CameraManager.Instance.SetQuake(transform.position, amplitude, frequency, attackTime, sustainTime, decayTime, impactRadius, dissipationDistance);
        } else if (impulseSource){
            impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitude;
            impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequency;
            impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = attackTime;
            impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = sustainTime;
            impulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = decayTime;
            impulseSource.m_ImpulseDefinition.m_ImpactRadius = impactRadius;
            impulseSource.m_ImpulseDefinition.m_DissipationDistance = dissipationDistance;
            impulseSource.GenerateImpulseAt(transform.position, Vector3.one);
        }
    }

    private void OnDisable() {
        if (cancelQuakeOnDisable) {
            if (CameraManager.Instance) {
                CameraManager.Instance.CancelQuake();
            } else if (CinemachineImpulseManager.Instance != null) {
                CinemachineImpulseManager.Instance.Clear();
            }
        }
    }

}
