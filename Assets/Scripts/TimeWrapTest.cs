using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class TimeWrapTest : MonoBehaviour
{
    public GameObject closedClock;
    public TimeWarpController timeWrap;
    private bool _isTimeWrapEnabled = false;

    private void Start()
    {
        closedClock.GetComponent<Image>().enabled = false;
    }

    public void EnableTimeWrap()
    {
        _isTimeWrapEnabled = true;
        closedClock.GetComponent<Image>().enabled = true;
    }

    void UpdateTimeWrappper(){  
        
    }
    void Update()
    {   
        if (TimeWrapper.GetTimeWrapper && !_isTimeWrapEnabled)
        {
            EnableTimeWrap();
        }
        if (Input.GetKeyDown(KeyCode.R) && _isTimeWrapEnabled)
        {
            timeWrap.StartTimeWarp();
        }
    }
}
