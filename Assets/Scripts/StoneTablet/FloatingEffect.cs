using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    public float floatSpeed = 1f;      // 浮动速度
    public float floatHeight = 0.03f;   // 浮动高度
    
    private Vector3 startPos;
    
    void Start()
    {
        startPos = transform.position;
    }
    
    void Update()
    {
        // 使用正弦函数创建平滑的上下浮动效果
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}