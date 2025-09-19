using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParticleSystemFollowing : MonoBehaviour
{
    private GameObject _player;
    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        transform.position = _player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.transform.position;
    }
}
