using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSettings : MonoBehaviour
{
    public GameObject openingCanvas;
    public GameObject settingsCanvas;

    private void Start()
    {
        openingCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void OnSettingsClicked()
    {
        openingCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void OnReturnClicked()
    {
        openingCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }
}
