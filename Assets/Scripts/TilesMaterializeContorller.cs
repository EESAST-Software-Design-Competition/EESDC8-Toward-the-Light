using System.Collections;
using UnityEngine;

public class TilesMaterializeController : MonoBehaviour
{
    // 平台类型枚举
    public enum PlatformType
    {
        BlackCloud,
        Block
    }

    [SerializeField] private PlatformType platformType;
    [SerializeField] private LightRangeHandler lightRangeHandler; // 光照范围引用

    private BoxCollider2D platformCollider;
    private bool isInLightRange = false;
    
    [SerializeField] private LayerMask playerLayer;
    private int originalLayer;
    private GameObject player;
    private Collider2D playerCollider;
    private void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        originalLayer = gameObject.layer;
        player = GameObject.FindWithTag("Player");
        playerCollider = player.GetComponent<Collider2D>();
        // 根据类型设置初始状态
        SetPlatformState(!lightRangeHandler.GetStatus());
    }

    private void Update()
    {
        // 如果在光照范围内，实时监测光照状态并更新平台状态
        if (isInLightRange)
        {
            SetPlatformState(!lightRangeHandler.GetStatus());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否是光照范围
        if (other.GetComponent<LightRangeHandler>() != null)
        {
            isInLightRange = true;
            // 根据光照状态更新平台状态
            SetPlatformState(!lightRangeHandler.GetStatus());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查是否是光照范围
        if (other.GetComponent<LightRangeHandler>() != null)
        {
            isInLightRange = false;
            // 恢复到默认状态
            SetPlatformState(true);
        }
    }

    private void SetPlatformState(bool isSolid)
    {
        bool shouldBeSolid = (platformType == PlatformType.BlackCloud) ? isSolid : !isSolid;
    
        // Check if we're about to make this solid while player is inside
        if (shouldBeSolid && platformCollider.isTrigger)
        {
            if (playerCollider != null && platformCollider.bounds.Intersects(playerCollider.bounds))
            {
                // Player is inside, start coroutine to handle collision
                StartCoroutine(HandlePlayerInside());
                return;
            }
        }
    
        // Apply the state change
        platformCollider.isTrigger = !shouldBeSolid;
    }

    private IEnumerator HandlePlayerInside()
    {
        // Make the platform temporarily ignore collision with player
        int noCollisionLayer = LayerMask.NameToLayer("No Collision");
        gameObject.layer = noCollisionLayer;
    
        // Make non-trigger for physics but still ignore player
        platformCollider.isTrigger = false;
    
        while (playerCollider != null && platformCollider.bounds.Intersects(playerCollider.bounds))
        {
            yield return null;
        }
    
        // Restore original layer
        gameObject.layer = originalLayer;
    }
}