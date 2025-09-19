using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal4ToEnding : MonoBehaviour
{
    public GameObject sceneTransition;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        sceneTransition.GetComponent<SceneTransitioner>().targetSceneName = "Ending";
        sceneTransition.GetComponent<SceneTransitioner>().StartSceneTransition();
    }
    public void SceneTransition()
    {
        sceneTransition.GetComponent<SceneTransitioner>().targetSceneName = "Ending";
        sceneTransition.GetComponent<SceneTransitioner>().StartSceneTransition();
    }
}
