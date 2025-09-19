using UnityEngine;

public class AdjustPlayerYPosition : MonoBehaviour
{
    [Header("设置")]
    public float thresholdY = -10f; // 判断阈值
    public float belowThresholdY = 20f; // 小于阈值时设置的Y坐标
    public float aboveThresholdY = -20f; // 大于阈值时设置的Y坐标
    public KeyCode triggerKey = KeyCode.R; // 触发按键（可选）

    void Update()
    {
        // 自动检测（每帧）
        // AutoAdjustPosition();
        
        // 或者使用按键触发（注释掉上面那行，取消下面注释）
        if (Input.GetKeyDown(triggerKey)) 
        {
            AdjustPosition();
        }
    }

    private void AdjustPosition()
    {
        float currentY = transform.position.y;
        
        if (currentY > thresholdY)
        {
            // 当前Y > -10，设置为-20
            SetPlayerYPosition(currentY + aboveThresholdY);
        }
        else
        {
            // 当前Y <= -10，设置为+20
            SetPlayerYPosition(currentY + belowThresholdY);
        }
    }

    private void SetPlayerYPosition(float newY)
    {
        // 只改变Y坐标，保持X和Z不变
        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
        
        Debug.Log($"玩家Y坐标已调整到: {newY}");
    }
}