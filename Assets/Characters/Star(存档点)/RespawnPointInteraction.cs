using UnityEngine;

public class RespawnPointInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform respawnPoint; // 重生点引用

    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SetRespawnPoint();
                UnityEngine.Debug.Log("SetRespawnPoint");

            }
        }
    }

    void SetRespawnPoint()
    {
            // 设置重生点位置为当前存档点位置
            respawnPoint.position = transform.position;
            
            // 可以在这里添加一些反馈效果，比如声音或粒子效果
            Debug.Log("Respawn point set to: " + transform.position);
            
            // 可选：显示存档成功的UI提示
            //UIManager.Instance.ShowMessage("Checkpoint Saved!");
    }

    // 可视化交互范围（仅在编辑器中可见）
}