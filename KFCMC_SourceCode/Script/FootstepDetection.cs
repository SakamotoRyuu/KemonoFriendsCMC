using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FriendsBase;
using static Rewired.Controller;

public class FootstepDetection : MonoBehaviour
{

    public bool isAudioEnabled;
    public JudgeFootMaterial judgeFootMaterial;
    public Transform footprintNormal;
    public Transform footprintForward;
    public ParticleSystem footprintParticle;
    public float pitchRate = 1f;

    AudioSource audioSource;
    AudioClip audioClip;
    double preEnterTime = -1.0f;
    int materialType = -1;
    GameObject colliderObj;
    int preMatType = -2;
    float exitTimeRemain;
    Ray footprintRay;
    RaycastHit hit;
    LayerMask rayLayer;
    static readonly Vector3 footprintOffset = new Vector3(0f, 0.2f, 0f);

    private void Awake()
    {
        if (isAudioEnabled)
        {
            audioSource = GetComponent<AudioSource>();
        }
        footprintRay.direction = Vector3.down;
        rayLayer = LayerMask.GetMask("Field", "SecondField");
    }

    private void FixedUpdate()
    {
        if (exitTimeRemain > 0f)
        {
            exitTimeRemain -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (FootstepManager.Instance && !other.isTrigger)
        {
            if (judgeFootMaterial && judgeFootMaterial.materialType >= 0)
            {
                materialType = judgeFootMaterial.materialType;
            }
            else
            {
                materialType = FootstepManager.Instance.JudgeType(other.gameObject);
            }
            if (materialType >= 0)
            {
                if (GameManager.Instance.time >= preEnterTime + 0.125 && (materialType != preMatType || (colliderObj == null && exitTimeRemain <= 0f) || (colliderObj != null && colliderObj.activeInHierarchy == false)))
                {
                    preEnterTime = GameManager.Instance.time;
                    if (isAudioEnabled && audioSource)
                    {
                        float pitchTemp = FootstepManager.Instance.GetMaterialPitch(materialType) * pitchRate;
                        if (pitchTemp < 1f)
                        {
                            pitchTemp = 1f;
                        }
                        if (audioSource.pitch != pitchTemp)
                        {
                            audioSource.pitch = pitchTemp;
                        }
                        audioClip = FootstepManager.Instance.GetPlayClip(materialType);
                        if (audioClip)
                        {
                            audioSource.PlayOneShot(audioClip);
                        }
                    }
                    if (footprintParticle && materialType == FootstepManager.FootprintMaterialType)
                    {
                        EmitFootprintParticle();
                    }
                }
                colliderObj = other.gameObject;
                preMatType = materialType;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (FootstepManager.Instance && colliderObj != null && other.gameObject == colliderObj)
        {
            colliderObj = null;
            exitTimeRemain = 0.03f;
        }
    }

    void EmitFootprintParticle()
    {
        footprintRay.origin = footprintParticle.transform.position + footprintOffset;
        if (Physics.Raycast(footprintRay, out hit, 0.4f, rayLayer, QueryTriggerInteraction.Ignore))
        {
            var ep = new ParticleSystem.EmitParams();
            ep.position = hit.point + (hit.normal * 0.005f);
            Vector3 eulerNormal = Quaternion.FromToRotation(Vector3.forward, hit.normal).eulerAngles;
            footprintNormal.eulerAngles = eulerNormal;
            Vector3 eulerForward = Vector3.zero;
            eulerForward.z = transform.eulerAngles.y;
            footprintForward.localEulerAngles = eulerForward;
            ep.rotation3D = footprintParticle.transform.eulerAngles;
            footprintParticle.Emit(ep, 1);
        }
    }

}
