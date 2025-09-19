using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EButtonFollowing : MonoBehaviour
{
    public float floatingDistance = 0.3f;

    private GameObject _playerObject;
    // Start is called before the first frame update
    void Start()
    {
        _playerObject = GameObject.FindWithTag("Player");
        transform.position = _playerObject.transform.position + Vector3.up * floatingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _playerObject.transform.position + Vector3.up * floatingDistance;
    }
}
