using UnityEngine;

public class TreeShadowCaster : MonoBehaviour
{
    [SerializeField] private Transform shadowObject;
    [SerializeField] private float shadowHeight = 1f;
    [SerializeField] private float maxShadowLength = 10f;
    [SerializeField] private float minShadowLength = 2f;
    [SerializeField] private LightRangeHandler lightSource;
    [SerializeField] private float treeBaseYOffset = 0f; // 树干底部相对于树对象中心的Y偏移量
    
    private BoxCollider2D shadowCollider;
    private SpriteRenderer shadowRenderer;
    
    private void Start()
    {
        if (shadowObject != null)
        {
            shadowCollider = shadowObject.GetComponent<BoxCollider2D>();
            if (shadowCollider == null)
            {
                shadowCollider = shadowObject.gameObject.AddComponent<BoxCollider2D>();
            }
            
            shadowRenderer = shadowObject.GetComponent<SpriteRenderer>();
            
            // 初始时隐藏阴影
            shadowObject.gameObject.SetActive(false);
        }
        
        if (lightSource == null)
        {
            // 如果未设置光源，尝试在场景中查找
            lightSource = FindObjectOfType<LightRangeHandler>();
        }
        treeBaseYOffset = - gameObject.GetComponent<SpriteRenderer>().bounds.extents.y;
    }
    
    private void Update()
    {
        if (lightSource != null)
        {
            UpdateShadow(lightSource.transform.position, lightSource.GetStatus());
        }
    }
    
    public void UpdateShadow(Vector2 lightPosition, bool lightEnabled)
    {
        if (shadowObject == null) return;
        
        // 如果光源关闭，隐藏阴影
        if (!lightEnabled)
        {
            shadowObject.gameObject.SetActive(false);
            return;
        }
        
        // 计算树的底部位置，而不是中心
        Vector2 treeBasePosition = new Vector2(transform.position.x, transform.position.y + treeBaseYOffset);
        
        // 计算光源到树底部的方向向量
        Vector2 directionToTree = treeBasePosition - lightPosition;
        
        // 如果光源与树几乎在同一位置，不显示阴影
        float distance = directionToTree.magnitude;
        if (distance < 0.001f)
        {
            shadowObject.gameObject.SetActive(false);
            return;
        }
        
        // 激活阴影对象
        shadowObject.gameObject.SetActive(true);
        
        // 确定阴影方向（只保留水平分量）
        float shadowDirection = Mathf.Sign(directionToTree.x);
        if (shadowDirection == 0) shadowDirection = 1; // 防止x方向为0的情况
        
        // 计算阴影长度（基于到光源的距离）
        float shadowLength = Mathf.Clamp(maxShadowLength * (1 / (distance * 0.5f)), minShadowLength, maxShadowLength);
        
        // 定位阴影（从树的底部水平延伸）
        Vector2 shadowPosition = new Vector2(
            treeBasePosition.x + (shadowLength / 2 * shadowDirection), 
            treeBasePosition.y
        );
        
        shadowObject.position = new Vector3(shadowPosition.x, shadowPosition.y, 0);
        
        // 保持阴影水平放置（无旋转）
        shadowObject.rotation = Quaternion.identity;
        if (shadowDirection < 0)
        {
            // 如果阴影向左延伸，翻转阴影而不是旋转
            shadowObject.localScale = new Vector3(-shadowLength, shadowHeight, 1);
        }
        else
        {
            shadowObject.localScale = new Vector3(shadowLength, shadowHeight, 1);
        }
    }
}
