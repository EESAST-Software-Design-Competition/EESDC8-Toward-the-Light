using System;
using System.Collections;
using System.Collections.Generic;
using SpriteGlow;
using UnityEngine;
using UnityEngine.UI;

public class GlowingControl : MonoBehaviour
{
    public GameObject hoverBar;
    public Material glowingMat;
    public Material unglowingMat;
    private Vector3 _origin;
    private void Start()
    {
        _origin = transform.position;
        hoverBar.SetActive(false);
        GetComponent<Image>().material = unglowingMat;
    }
    public void OnEnter()
    {
        if (!hoverBar.activeSelf)
            hoverBar.SetActive(true);
        GetComponent<Image>().material = glowingMat;
    }
    

    public void OnExit()
    {
        if (hoverBar.activeSelf)
            hoverBar.SetActive(false);
        GetComponent<Image>().material = unglowingMat;
    }
}
