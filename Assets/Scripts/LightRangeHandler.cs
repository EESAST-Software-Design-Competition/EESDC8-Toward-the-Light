using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class LightRangeHandler : MonoBehaviour
{
    private bool _isEnabled = true;
    private Light2D _light;
    public float fadeOutTime = .5f;
    public float maxLightIntensity = .7f;
    public float lightRadius = 1f;
    private bool _isIntensityChanging = false;
    private CircleCollider2D _circle;
    private void Awake()
    {
        _light = gameObject.GetComponent<Light2D>();
        _light.intensity = 0f;
        _circle = gameObject.GetComponent<CircleCollider2D>();
        _circle.radius = lightRadius;
        _light.pointLightOuterRadius = lightRadius;
        StartCoroutine(OnLightIntensityChange(maxLightIntensity));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_isIntensityChanging)
            {
                if (_isEnabled)
                {
                    _isEnabled = false;
                    StartCoroutine(OnLightIntensityChange(0f));
                }
                else
                {
                    _isEnabled = true;
                    StartCoroutine(OnLightIntensityChange(maxLightIntensity));
                }
            }
        }
    }
    public bool GetStatus()
    {
        // Debug.Log($"IsEnabled:{_isEnabled}");
        return _isEnabled;
    }

    public float GetIntensity()
    {
        return _light.intensity;
    }
    private IEnumerator OnLightIntensityChange(float targetIntensity)
    {
        _isIntensityChanging = true;
        float elapsed = 0f;
        float intensityBefore = _light.intensity;
        while (elapsed < fadeOutTime)
        {
            _light.intensity = (targetIntensity - intensityBefore) * (elapsed / fadeOutTime) + intensityBefore;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _light.intensity = targetIntensity;
        _isIntensityChanging = false;
    }
    
}
