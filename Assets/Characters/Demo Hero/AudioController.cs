using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]


public class SoundEffect {
    public string name;
    public AudioClip clip;
    [Range(0,1)] public float volume = 1f;
    [Range(0.1f,3f)] public float pitch = 1f;
}

public class AudioController : MonoBehaviour {
    [SerializeField] private SoundEffect[] soundEffects;
    private AudioSource audioSource;

    [SerializeField] private float fallingVolumeThreshold = 0.3f; // 下落音效最小音量阈值
    [SerializeField] private AudioSource _runSource; // 专用跑步音效通道
    [SerializeField] private AudioSource _fallSource; // 专用下落音效通道
    void Awake() {
        audioSource = GetComponent<AudioSource>();
        // _runSource = GetComponent<AudioSource>();
    }

    public void Play(string soundName) {
        SoundEffect sfx = System.Array.Find(soundEffects, s => s.name == soundName);
        if (sfx != null) {
            audioSource.pitch = sfx.pitch;
            audioSource.PlayOneShot(sfx.clip, sfx.volume);
        }
    }

    public void PlayRunLoop(bool isRunning, float speedNormalized = 1f) {
        if (_runSource == null) {
            _runSource = gameObject.AddComponent<AudioSource>();
            _runSource.loop = true;
            _runSource.clip = System.Array.Find(soundEffects, s => s.name == "PlayerRun").clip;
        }
        if (_runSource.clip == null) {
            Debug.LogError("Run sound clip not found in sound effects array.");
            return;
        }

        if (isRunning) {
            _runSource.pitch = Mathf.Clamp(speedNormalized, 0.8f, 1.5f); // 根据速度调整音调
            if (!_runSource.isPlaying) {
                _runSource.Play();
                UnityEngine.Debug.Log("Run sound started playing.");
            }
        } 
        else if (_runSource.isPlaying) {
            _runSource.Stop();
        }
    }
    public void HandleFalling(bool isFalling, float fallSpeedNormalized) {
        if (_fallSource == null) {
            _fallSource = gameObject.AddComponent<AudioSource>();
            _fallSource.clip = System.Array.Find(soundEffects, s => s.name == "Falling").clip;
            _fallSource.loop = true;
            _fallSource.volume = 0; // 初始静音
        }

        
        if (isFalling) {
            // 动态音量控制（下落速度越快音量越大）
            float targetVolume = Mathf.Clamp(fallSpeedNormalized, fallingVolumeThreshold, 1f);
            _fallSource.volume = Mathf.Lerp(_fallSource.volume, targetVolume, Time.deltaTime * 5f);
            
            if (!_fallSource.isPlaying) {
                _fallSource.Play();
            }
        } 
        else {
            // 平滑淡出
            _fallSource.volume = Mathf.Lerp(_fallSource.volume, 0, Time.deltaTime * 3f);
            if (_fallSource.volume < 0.01f) {
                _fallSource.Stop();
            }
        }
    }
}