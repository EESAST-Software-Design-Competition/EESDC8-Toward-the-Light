using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
    public delegate void PlayerDetectDelegate(bool isPlayerInRange);
    private List<PlayerDetectDelegate> _playerDetectDelegates = new List<PlayerDetectDelegate>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void AddFunction(PlayerDetectDelegate function)
    {
        if (function != null)
        {
            _playerDetectDelegates.Add(function);
        }
    }
    
    public void RemoveFunction(PlayerDetectDelegate function)
    {
        if (function != null && _playerDetectDelegates.Contains(function))
        {
            _playerDetectDelegates.Remove(function);
        }
    }

    public void ClearFunctions()
    {
        _playerDetectDelegates.Clear();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected");
            foreach (var function in _playerDetectDelegates)
            {
                function?.Invoke(true);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player out of range");
            foreach (var function in _playerDetectDelegates)
            {
                function?.Invoke(false);
            }
        }
    }
}
