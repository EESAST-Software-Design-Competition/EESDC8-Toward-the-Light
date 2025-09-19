using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveNoUp : MonoBehaviour
{
    public Transform playerTransform;
    public float leftLimit = -1.4f;
    public float rightLimit = 12.8f;
    private Camera _cam;
    private float _horizontalExtent;
    void Start()
    {
        _cam = GetComponent<Camera>();
        _horizontalExtent = _cam.orthographicSize * _cam.aspect;
    }

    void Update()
    {
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        Vector3 targetPos = playerTransform.position;
        targetPos.z = transform.position.z;
        targetPos.y = transform.position.y;
        float leftBoundary = targetPos.x - _horizontalExtent;
        if (leftBoundary < leftLimit)
        {
            targetPos.x = leftLimit + _horizontalExtent;
        }
        float rightBoundary = targetPos.x + _horizontalExtent;
        if (rightBoundary > rightLimit)
        {
            targetPos.x = rightLimit - _horizontalExtent;
        }
        transform.position = targetPos;
    }
}
