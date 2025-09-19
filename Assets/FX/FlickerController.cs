using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerController : MonoBehaviour
{
    public float flickerTime;
    public float maxAlpha = 1f;
    private float _startTime;
    public float rotateSpeed;
    public float stayingThreshold;
    private void Start()
    {
        _startTime = Time.time;
        GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", 0f);
    }

    void Update()
    {
        float flyingProgress = (Time.time - _startTime) / (flickerTime / 2);
        float target;
        if (flyingProgress > stayingThreshold && flyingProgress < 2 - stayingThreshold)
        {
            target = 1f;
        }
        else if (flyingProgress < stayingThreshold)
        {
            target = MathF.Pow(flyingProgress / stayingThreshold, 2) * maxAlpha;
        }
        else
        {
            target = MathF.Pow((2 - flyingProgress) / stayingThreshold, 2) * maxAlpha;
        }
        GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", target * maxAlpha);
        float rotAmount = rotateSpeed * Time.deltaTime;
        float curRot = transform.localRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Euler(new Vector3(0,0,curRot+rotAmount));
    }
}
