using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform playerTransform;
    public float leftLimit = -7.2f;
    public float rightLimit = 50f;

    public float downLimit1 = -1.2f;
    public float downLimit2 = -10f;
    public float downLimit ;
    private Camera _cam;
    private float _horizontalExtent;
    private float _verticalExtent;
    void Awake()
    {
        _cam = GetComponent<Camera>();
        _horizontalExtent = _cam.orthographicSize * _cam.aspect;
        if (gameObject.name == "Main Camera")
            _verticalExtent = _cam.orthographicSize;
        else
            _verticalExtent = _cam.orthographicSize - 20f;
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        downLimit = playerTransform.position.x < 45 ? downLimit1 : downLimit2;
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        Vector3 targetPos = playerTransform.position;
        targetPos.z = transform.position.z;
        float leftBoundary = targetPos.x - _horizontalExtent;
        float downBoundary = targetPos.y - _verticalExtent;
        float rightBoundary = targetPos.x + _horizontalExtent;
        if (leftBoundary < leftLimit)
        {
            targetPos.x = leftLimit + _horizontalExtent;
        }
        if (rightBoundary > rightLimit)
        {
            targetPos.x = rightLimit - _horizontalExtent;
        }
        if (downBoundary < downLimit)
        {
            targetPos.y = downLimit + _verticalExtent;
        }
        transform.position = targetPos;
    }
}
