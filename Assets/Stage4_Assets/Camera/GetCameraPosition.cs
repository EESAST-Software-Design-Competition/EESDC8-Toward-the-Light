using UnityEngine;

public class GetCameraPosition : MonoBehaviour
{
    public Transform PlayerNowposition;
    void Update()
    {
        // ��ȡ Main Camera
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // ��ȡ Main Camera ��λ��

            mainCamera.transform.position = new Vector3(PlayerNowposition.transform.position.x, PlayerNowposition.transform.position.y, PlayerNowposition.transform.position.z - 10);
//            Debug.Log("Main Camera ��λ��: ");
        }
        else
        {
            Debug.LogError("Main Camera δ�ҵ���");
        }
    }
}