using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundShift : MonoBehaviour
{
    public Transform cameraTransform;
    public float cameraWidth = 3.2f;
    public int spriteCount = 3;
    private Vector3 _lastCameraPosition;
    private SpriteRenderer _spriteRenderer;
    private float _spriteWidth;
    void Start()
    {
        _lastCameraPosition = cameraTransform.position;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteWidth = 4.8f;
    }
    
    void Update()
    {
        Vector3 cameraDelta = cameraTransform.position - _lastCameraPosition;
        _lastCameraPosition = cameraTransform.position;
        float parallaxOffset = cameraDelta.x;
        transform.position += Vector3.right * parallaxOffset;
        if (transform.position.x < cameraTransform.position.x - cameraWidth / 2 - 1.5f * _spriteWidth)
        {
            transform.position += _spriteWidth * spriteCount * Vector3.right;
        }

        if (transform.position.x > cameraTransform.position.x + cameraWidth / 2 + 1.5f * _spriteWidth)
        {
            transform.position += _spriteWidth * spriteCount * Vector3.left;
        }

        transform.position += cameraDelta.y * Vector3.up;
    }
}
