using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneFloating : MonoBehaviour
{
    public float floatingSpeed = 0.1f;
    public float floatingRange = 0.1f;
    private float _startTime;
    private Vector3 _startPosition;
    void Start()
    {
        _startTime = Time.time;
        _startPosition = transform.position;
    }
    void Update()
    {
        transform.position = _startPosition + Vector3.up * (Mathf.Sin((Time.time - _startTime) * floatingSpeed) * floatingRange);
    }
}
