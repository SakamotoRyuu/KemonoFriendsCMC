using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Effects;

public class PaperPlane : MonoBehaviour {

    public bool lookForward;
    public float lookForwardVelocityThreshold = 0.5f;
    public float decoyVelocityThreshold = 0.1f;
    public float specialGravity = 0.1f;
    public GameObject[] activateObjects;
    public ParticleSystem[] particles;
    public GameObject normalLooks;
    public GameObject emissionLooks;
    public GameObject nightLightObj;
    public ParticleSystem nightLightParticle;
    public FireLight nightLightFire;

    private Rigidbody rb;
    private bool isMoving = false;
    private float movingTime = 0f;
    private float stoppingTime = 0f;
    private float duration;
    private int progress;
    private bool nightLightEnabled;
    private float nightLightIntensity;
    private float nightLightDecreaseRate = 2f;
    bool forceStopFlag = false;
    public bool ParentIsPlayer { get; private set; }

    void ActivateObjects(bool flag) {
        for (int i = 0; i < activateObjects.Length; i++) {
            activateObjects[i].SetActive(flag);
        }
        for (int i = 0; i < particles.Length; i++) {
            if (flag) {
                particles[i].Play();
            } else {
                particles[i].Stop();
            }
        }
    }

    public void ForceStop() {
        forceStopFlag = true;
        nightLightDecreaseRate = 5f;
    }

    void Awake()
    {
        CharacterBase parentCBase = GetComponentInParent<CharacterBase>();
        if (parentCBase != null)
        {
            ParentIsPlayer = parentCBase.isPlayer;
        }
    }
    
    void Start() {
        rb = GetComponent<Rigidbody>();
        duration = 0;
        progress = 0;
        movingTime = 0f;
        stoppingTime = 0f;
        ActivateObjects(false);
        if (nightLightObj && LightingDatabase.Instance && LightingDatabase.Instance.IsNight) {
            normalLooks.SetActive(false);
            emissionLooks.SetActive(true);
            nightLightObj.SetActive(true);
            nightLightFire.intensity = 1.5f;
            nightLightEnabled = true;
        }
    }

    void FixedUpdate()
    {
        if (rb != null && Time.timeScale > 0f)
        {
            rb.drag = Mathf.Clamp((rb.velocity.magnitude - 3f) * 0.25f, 0.05f, 10f);
            if (lookForward && rb.velocity.sqrMagnitude >= lookForwardVelocityThreshold * lookForwardVelocityThreshold)
            {
                rb.MoveRotation(Quaternion.LookRotation(rb.velocity.normalized));
            }
            if (specialGravity != 0.0f)
            {
                rb.AddForce(Physics.gravity * specialGravity * Time.timeScale, ForceMode.Acceleration);
            }
        }
    }
    
    void Update() {
        if (rb != null && Time.timeScale > 0f) {
            float deltaTimeCache = Time.deltaTime;
            if (!rb.isKinematic)
            {
                isMoving = (rb.velocity.sqrMagnitude >= decoyVelocityThreshold * decoyVelocityThreshold);
                if (isMoving)
                {
                    movingTime += deltaTimeCache;
                    stoppingTime = 0f;
                }
                else
                {
                    movingTime = 0f;
                    stoppingTime += deltaTimeCache;
                }
            }
            else
            {
                isMoving = false;
                movingTime = 0f;
                stoppingTime = 0f;
            }
            duration += deltaTimeCache;

            switch (progress)
            {
                case 0:
                    if (duration >= 0.6f && !rb.isKinematic && movingTime >= 0.3f)
                    {
                        ActivateObjects(true);
                        progress++;
                    }
                    break;
                case 1:
                    if (stoppingTime >= 0.2f || forceStopFlag)
                    {
                        ActivateObjects(false);
                        progress++;
                        if (nightLightEnabled && nightLightParticle)
                        {
                            nightLightParticle.Stop();
                        }
                    }
                    break;
                case 2:
                    if (nightLightEnabled && nightLightFire)
                    {
                        nightLightFire.intensity -= deltaTimeCache * nightLightDecreaseRate;
                        if (nightLightFire.intensity <= 0f)
                        {
                            emissionLooks.SetActive(false);
                            normalLooks.SetActive(true);
                            nightLightFire.Extinguish();
                            nightLightFire.gameObject.SetActive(false);
                            nightLightEnabled = false;
                        }
                    }
                    break;
            }
        }
    }

}
