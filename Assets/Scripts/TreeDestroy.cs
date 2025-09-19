using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDestroy : MonoBehaviour
{
    private TimeWrapMysteries _timeWrapMysteries;

    private void Awake()
    {
        _timeWrapMysteries = GameObject.FindWithTag("Mystery").GetComponent<TimeWrapMysteries>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        _timeWrapMysteries.OnTreeDestroyedBefore();
    }
}
