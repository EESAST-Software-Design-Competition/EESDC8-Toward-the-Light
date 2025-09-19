using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInRelease : MonoBehaviour
{
    private void Awake()
    {
        if (Application.isEditor)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
