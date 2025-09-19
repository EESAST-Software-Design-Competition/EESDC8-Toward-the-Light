using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public GameObject pauseButton;
    public GameObject pauseCanvas;
    public GameObject settingsCanvas;
    public GameObject player;
    private bool _isPause = false;
    private Vector2 _pausePlayerSpeed;
    private void Start()
    {
        pauseCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (!_isPause) 
                OnPause();
            else
            {
                OnResume();
            }
        }
    }

    public void OnPause()
    {
        if (_isPause)
        {
            return;
        }
        Debug.Log("Pause Here");
        Time.timeScale = 0f;
        _pausePlayerSpeed = player.GetComponent<Rigidbody2D>().velocity;
        pauseCanvas.SetActive(true);
        pauseButton.SetActive(false);
        _isPause = true;
    }

    public void OnExternPause()
    {
        if (_isPause)
        {
            return;
        }
        Debug.Log("Pause Here");
        _pausePlayerSpeed = player.GetComponent<Rigidbody2D>().velocity;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        _isPause = true;
    }
    public void OnResume()
    {
        if (!_isPause)
        {
            return;
        }
        Time.timeScale = 1f;
        player.GetComponent<Rigidbody2D>().velocity = _pausePlayerSpeed;
        pauseCanvas.SetActive(false);
        pauseButton.SetActive(true);
        _isPause = false;
    }

    public void OnExternResume()
    {
        if (!_isPause)
        {
            return;
        }
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player.GetComponent<Rigidbody2D>().velocity = _pausePlayerSpeed;
        _isPause = false;
    }
    public void OnSettings()
    {
        pauseCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public bool GetPauseStatus()
    {
        return _isPause;
    }
    public void OnSettingsReturn()
    {
        settingsCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
