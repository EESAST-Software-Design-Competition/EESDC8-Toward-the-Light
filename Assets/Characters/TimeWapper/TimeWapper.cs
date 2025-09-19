using UnityEngine;

public class TimeWrapper : MonoBehaviour
{
    [Header("设置")]
    public GameObject objectToDisappear; // 将要消失的物体
    public static bool GetTimeWrapper = false;
    void Start()
    {
        
    }
    void Update()
    {

    }
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (objectToDisappear != null)
                {
                    objectToDisappear.SetActive(false);
                    
                    
                }
                GetTimeWrapper = true;
                Fungus.Flowchart.BroadcastFungusMessage("TimeWrapper");
                UnityEngine.Debug.Log("TimeWrapper");
            }
        }
    }

}