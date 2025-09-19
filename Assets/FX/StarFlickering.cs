using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class StarFlickering : MonoBehaviour
{
    public GameObject originRectangle;    
    public float flickerInterval = 0.5f;
    public float flickerVariable = 0.1f;
    public float floatSpeed = 0.7f;
    public float flickerMaxAlpha = 1f;
    public float flyingDis = 2f;
    public float flyingDisRange = 0.3f;
    public float rotateSpeed = 0.5f;
    public float stayingThreshold = 0.8f;
    public Vector2 flickerDirection = new Vector2(-3f, -1f);
    public GameObject FlickerPrefab;
    private float _flickerTick;
    private float _nextFlickerTick;
    private Vector2 _startMin;
    private Vector2 _startMax;
    private Vector2 _endMin;
    private Vector2 _endMax;
    void Start()
    {
        _startMax = originRectangle.transform.position + originRectangle.transform.localScale / 2;
        _startMin = originRectangle.transform.position - originRectangle.transform.localScale / 2;
        _flickerTick = 0;
    }
    private void fireFlicker()
    {
        GameObject flicker = Instantiate(FlickerPrefab);
        Vector2 startPos = new Vector2(Random.Range(_startMin.x, _startMax.x), Random.Range(_startMin.y, _startMax.y));
        Vector2 endPos = startPos + flickerDirection.normalized *
            Random.Range(flyingDis - flyingDisRange, flyingDis + flyingDisRange);
        flicker.GetComponent<Rigidbody2D>().velocity = (endPos - startPos).normalized * floatSpeed;
        flicker.GetComponent<Transform>().position = startPos;
        float flyingTime = (endPos - startPos).magnitude / floatSpeed;
        flicker.GetComponent<FlickerController>().flickerTime = flyingTime;
        flicker.GetComponent<FlickerController>().maxAlpha = flickerMaxAlpha;
        flicker.GetComponent<FlickerController>().rotateSpeed = rotateSpeed;
        flicker.GetComponent<FlickerController>().stayingThreshold = stayingThreshold;
        Destroy(flicker, flyingTime);
    }
    // Update is called once per frame
    void Update()
    {
        _flickerTick += Time.deltaTime;
        if (_flickerTick > _nextFlickerTick)
        {
            fireFlicker();
            _nextFlickerTick = Random.Range(flickerInterval - flickerVariable, flickerInterval + flickerVariable);
            _flickerTick = 0;
        }
    }
}
