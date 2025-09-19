using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    public static ResetScene Instance;
    private List<TrapFalling> _resetListSpikes = new List<TrapFalling>();
    private List<Fail> _resetListFail = new List<Fail>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Scene_1_Cave")
        {
            _resetListSpikes.Clear();
            _resetListSpikes.AddRange(FindObjectsOfType<TrapFalling>());
        }
        if (SceneManager.GetActiveScene().name == "Scene_4_Starry")
        {
            _resetListFail.Clear();
            _resetListFail.AddRange(FindObjectsOfType<Fail>());
        }
        // {
        //     _resetListFail.Clear();
        //     _resetListFail.AddRange(FindObjectsOfType<Fail>());
        // }
    }

    public void OnSceneReset()
    {
        if (SceneManager.GetActiveScene().name == "Scene_1_Cave")
            foreach (var trapFalling in _resetListSpikes)
            {
                trapFalling.ResetSpike();
            }
        if (SceneManager.GetActiveScene().name == "Scene_4_Starry")
        {
            foreach (var platformFalling in _resetListFail)
            {
                platformFalling.OnPlatformReset();
            }
        }
    }
}
