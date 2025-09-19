using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFalling : MonoBehaviour
{
    [Header("设置")]
    [SerializeField] private float detectionWidth = 0.8f; // 检测区域宽度
    [SerializeField] private float detectionHeight = 2f; // 检测区域高度
    [SerializeField] private float fallDelay = 0.5f; // 检测到玩家后延迟掉落的时间
    [SerializeField] private float destroyDelay = 2f; // 穿过地面后销毁的延迟时间
    [SerializeField] private LayerMask playerLayer; // 玩家所在的层
    [SerializeField] private LayerMask groundLayer; // 地面所在的层
    
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private bool playerDetected = false;
    private bool hasFallen = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isFalling = false;
    private void Awake()
    {
        // 获取组件
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        // 如果没有这些组件，添加它们
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
            
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        
        // 保存初始位置和旋转
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // 初始设置刚体
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        playerLayer = LayerMask.GetMask("Player");
        groundLayer = LayerMask.GetMask("Tiles");
    }
    
    private void Update()
    {
        // 检测玩家是否在尖刺正下方
        if (!playerDetected && !hasFallen)
        {
            DetectPlayer();
        }
    }
    
    private void DetectPlayer()
    {
        // 创建一个矩形检测区域在尖刺正下方
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (detectionHeight / 2 + boxCollider.size.y / 2);
        Vector2 boxSize = new Vector2(detectionWidth, detectionHeight);
        
        // 使用OverlapBox检测玩家是否在矩形区域内
        Collider2D playerCollider = Physics2D.OverlapBox(boxCenter, boxSize, 0f, playerLayer);
        
        if (playerCollider != null)
        {
            playerDetected = true;
            StartCoroutine(InitiateFall());
        }
    }
    
    private IEnumerator InitiateFall()
    {
        isFalling = true;
        // 简单的晃动效果
        yield return StartCoroutine(ShakeEffect());
        
        // 等待延迟时间后掉落
        yield return new WaitForSeconds(fallDelay);
        
        // 切换为动态刚体并启用重力
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        hasFallen = true;
        
    }
    
    private IEnumerator ShakeEffect()
    {
        // 简单的晃动效果
        float elapsed = 0f;
        float duration = fallDelay;
        float magnitude = 0.02f;
        
        while (elapsed < duration && isFalling)
        {
            float x = initialPosition.x + Random.Range(-1f, 1f) * magnitude;
            transform.position = new Vector3(x, initialPosition.y, initialPosition.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 恢复初始位置以准备掉落
        transform.position = initialPosition;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检测是否与地面碰撞
        if (hasFallen && (groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            // 延迟销毁游戏对象
            StartCoroutine(FadeAndDestroy());
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 逐渐淡出尖刺
            float elapsed = 0f;
            float duration = destroyDelay;
            Color originalColor = spriteRenderer.color;
            
            while (elapsed < duration && isFalling)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        gameObject.SetActive(false); // 隐藏尖刺
    }
    
    private void OnDrawGizmosSelected()
    {
        // 在编辑器中可视化检测范围（矩形）
        Gizmos.color = Color.red;
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (detectionHeight / 2 + (boxCollider != null ? boxCollider.size.y / 2 : 0.5f));
        Vector2 boxSize = new Vector2(detectionWidth, detectionHeight);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
    
    // 重置方法（用于编辑器测试）
    public void ResetSpike()
    {
        gameObject.SetActive(true);
        isFalling = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = new Color(
                GetComponent<SpriteRenderer>().color.r,
                GetComponent<SpriteRenderer>().color.g,
                GetComponent<SpriteRenderer>().color.b,
                1f
            );
        }
        
        boxCollider.enabled = true;
        playerDetected = false;
        hasFallen = false;
    }
}
