using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public AudioSource gameAudio;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;
    private void Start()
    {
            volumeSlider.value = 100f;
            volumeText.text = string.Format("{0:G}", (int)volumeSlider.value);
    }

    public void OnVolumeSliderValueChanged(float newValue)
    {
        if (gameAudio != null)
        {
            gameAudio.volume = newValue;
        }
        volumeText.text = string.Format("{0:G}", (int)volumeSlider.value);
    }
}
