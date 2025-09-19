using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockObtain : MonoBehaviour
{
    public TimeWrapTest timeWrap;
    private GameObject player;
    public GameObject exclaimMark;
    private GameObject _exclaim;
    public float exclaimDis = 0.4f;
    private bool _isTouching = false;
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.Log("Couldn't find player!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (!_isTouching)
        {
            _isTouching = true;
            _exclaim = Instantiate(exclaimMark, player.transform);
            _exclaim.transform.position += Vector3.up * exclaimDis;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (_isTouching)
        {
            _isTouching = false;
            Destroy(_exclaim);
        }
    }

    private void Update()
    {
        if (_isTouching)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                timeWrap.EnableTimeWrap();
            }
        }
    }
}
