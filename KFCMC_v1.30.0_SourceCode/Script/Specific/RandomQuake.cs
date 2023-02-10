using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomQuake : MonoBehaviour {

    public GameObject effectPrefab;
    public Vector2 intervalRange;
    public Vector2 amplitudeRange;
    public Vector2 frequencyRange;
    public Vector2 volumeRange;
    public Vector2 pitchRange;

    private float intervalRemain;

    void Start() {
        intervalRemain = Random.Range(intervalRange.x, intervalRange.y);
    }
    
    void Update() {
        if (Time.timeScale > 0) {
            intervalRemain -= Time.deltaTime;
            if (intervalRemain <= 0f) {
                float ampParam = Random.Range(0f, 1f);
                float freqParam = Random.Range(0f, 1f);
                SetQuake(ampParam, freqParam, 100f);
            }
        }
    }

    public void SetQuake(float ampParam, float freqParam, float positionRandomRadius) {
        intervalRemain = Random.Range(intervalRange.x, intervalRange.y);
        if (CameraManager.Instance && CharacterManager.Instance && CharacterManager.Instance.playerTrans) {
            Vector3 epicenter = CharacterManager.Instance.playerTrans.position + Random.insideUnitSphere * positionRandomRadius;
            CameraManager.Instance.SetQuake(epicenter, Mathf.Lerp(amplitudeRange.x, amplitudeRange.y, ampParam), Mathf.Lerp(frequencyRange.x, frequencyRange.y, freqParam), 0f, 0f, 3f, 100f, 500f);
            GameObject effectInstance = Instantiate(effectPrefab, epicenter, Quaternion.identity);
            AudioSource audioSource = effectInstance.GetComponent<AudioSource>();
            audioSource.volume = Mathf.Lerp(volumeRange.x, volumeRange.y, ampParam);
            audioSource.pitch = Mathf.Lerp(pitchRange.x, pitchRange.y, freqParam);
            audioSource.Play();
        }
    }
}
