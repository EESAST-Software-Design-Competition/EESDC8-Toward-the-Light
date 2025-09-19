using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLight : MonoBehaviour
{
    private bool _isPickedUp = true;
    private GameObject _player;
    private Rigidbody2D _rb;
    private bool _isWithinRange;
    [SerializeField] private PlayerDetect _playerDetect;
    private void Awake()
    {
        _player = GameObject.FindWithTag("Player");
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _playerDetect.AddFunction(SetWithinRange);
    }

    private void Update()
    {
        /*if (_isPickedUp)
        {
            transform.position = _player.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.E) && (_isWithinRange || _isPickedUp))
        {
            if (_isPickedUp)
            {
                OnDropped();
            }
            else
            {
                OnPickedUp();
            }
        }*/
        transform.position = _player.transform.position;
        if (!_isPickedUp) OnPickedUp();
    }

    public void OnPickedUp()
    {
        Debug.Log("Picked up");
        _isPickedUp = true;
        _rb.isKinematic = true;
    }

    public void OnDropped()
    {
        Debug.Log("Dropped");
        _isPickedUp = false;
        _rb.isKinematic = false;
    }
    
    public void SetWithinRange(bool withinRange)
    {
        _isWithinRange = withinRange;
    }
}
