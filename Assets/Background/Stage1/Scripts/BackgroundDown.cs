using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDown : MonoBehaviour
{
    public Transform cameraTransform;
    private Camera _cam;
    public float parallaxFactor = 1f;
    public float verticalParallaxFactor = 0.9f;
    public int spriteCount = 2;
    private Vector3 _lastCameraPosition;
    private SpriteRenderer _spriteRenderer;
    private float _spriteWidth;
    void Start()
    {
        _cam = Camera.main;
        _lastCameraPosition = cameraTransform.position;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteWidth = _spriteRenderer.sprite.bounds.size.x * transform.localScale.x;
    }
    
    void LateUpdate()
    {
        float cameraWidth = _cam.orthographicSize * _cam.aspect * 2;
        Vector3 cameraDelta = cameraTransform.position - _lastCameraPosition;
        _lastCameraPosition = cameraTransform.position;
        float parallaxOffset = cameraDelta.x * parallaxFactor;
        transform.position += Vector3.right * parallaxOffset;
        if (transform.position.x < cameraTransform.position.x - cameraWidth / 2 - _spriteWidth)
        {
            transform.position += _spriteWidth * spriteCount * Vector3.right;
        }

        if (transform.position.x > cameraTransform.position.x + cameraWidth / 2)
        {
            transform.position += _spriteWidth * spriteCount * Vector3.left;
        }
        float downOffset = cameraDelta.y * parallaxFactor;
        transform.position += Vector3.up * (downOffset * verticalParallaxFactor);
    }
}
