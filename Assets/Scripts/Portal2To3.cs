using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal2To3 : MonoBehaviour
{
    public GameObject sceneTransition;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        sceneTransition.GetComponent<SceneTransitioner>().targetSceneName = "Scene_3_Forest";
        sceneTransition.GetComponent<SceneTransitioner>().StartSceneTransition();
    }
}
