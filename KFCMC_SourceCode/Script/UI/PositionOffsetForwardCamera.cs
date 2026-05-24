using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionOffsetForwardCamera : MonoBehaviour
{
    public float OffsetDistance;

    private Transform _camT;
    private Transform _parentTransform;

    private void Awake()
    {
        _parentTransform = transform.parent;
    }

    private void LateUpdate()
    {
        if (!_camT && CameraManager.Instance)
        {
            _camT = CameraManager.Instance.mainCamera.transform;
        }
        if (_camT && _parentTransform)
        {
            Vector3 camPos = _camT.position;
            Vector3 parentPos = _parentTransform.position;
            Vector3 diff = camPos - parentPos;
            float distance = Vector3.Distance(camPos, parentPos);
            transform.position = parentPos + diff.normalized * Mathf.Min(OffsetDistance, distance * 0.5f);
        }
    }
}
