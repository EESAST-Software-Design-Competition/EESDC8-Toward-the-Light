using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
        if (GameObject.FindWithTag("UI") != null)
            Destroy(this);
        gameObject.SetActive(true);
    }
}
