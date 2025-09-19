using UnityEngine;
using System.Collections;

public class HiddenRoomTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip revealSound;
    [SerializeField] private GameObject obstructionObjects;
    [SerializeField] private float fadeDuration = 2.0f;
    
    private UnityEngine.AudioSource audioSource;
    private bool hasBeenRevealed = false;
    
    private void Start()
    {
        // 确保有一个AudioSource组件
        audioSource = GetComponent<UnityEngine.AudioSource>();
        if (audioSource == null)
        {

            audioSource = gameObject.AddComponent<UnityEngine.AudioSource>();
        }
        // UnityEngine.Debug.Log("HiddenRoomTrigger initialized with AudioSource: " + audioSource.name);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        UnityEngine.Debug.Log("Trigger entered by: " + other.name);
        if (hasBeenRevealed || !other.CompareTag("Player")) return;
        
        hasBeenRevealed = true;
        StartCoroutine(RevealHiddenRoom());
    }
    
    private IEnumerator RevealHiddenRoom()
    {
        UnityEngine.Debug.Log("Revealing hidden room...");
        // 播放音效 - 修正后的PlayOneShot调用方式
        if (revealSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(revealSound);
        }
        
        // 渐变消失效果
        if (obstructionObjects != null)
        {
            Renderer[] renderers = obstructionObjects.GetComponentsInChildren<Renderer>();
            float elapsedTime = 0f;
            
            // 存储初始alpha值
            float[] initialAlphas = new float[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null && renderers[i].material != null)
                {
                    initialAlphas[i] = renderers[i].material.color.a;
                }
            }
            
            // 渐变过程
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                
                for (int i = 0; i < renderers.Length; i++)
                {
                    if (renderers[i] != null && renderers[i].material != null)
                    {
                        Color newColor = renderers[i].material.color;
                        newColor.a = alpha * initialAlphas[i];
                        renderers[i].material.color = newColor;
                    }
                }
                yield return null;
            }
            
            // 完全隐藏遮挡物
            obstructionObjects.SetActive(false);
        }
    }
}