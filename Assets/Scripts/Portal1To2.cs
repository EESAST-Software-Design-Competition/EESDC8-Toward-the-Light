using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal1To2 : MonoBehaviour
{
    public GameObject sceneTransition;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        sceneTransition.GetComponent<SceneTransitioner>().targetSceneName = "Scene_2_Mountain";
        sceneTransition.GetComponent<SceneTransitioner>().StartSceneTransition();
    }
}
