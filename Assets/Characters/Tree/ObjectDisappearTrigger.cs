using UnityEngine;

public class ObjectDisappearTrigger : MonoBehaviour
{
    [Header("设置")]
    public GameObject objectToDisappear; // 将要消失的物体
    public GameObject objectToDisappear2; // 将要消失的物体
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
                UnityEngine.Debug.Log("ObjectDisappearTrigger");
                if (objectToDisappear != null)
                {
                    UnityEngine.Debug.Log("ObjectDisappearTrigger");
                    objectToDisappear.SetActive(false);objectToDisappear2.SetActive(false);
                    
                    Fungus.Flowchart.BroadcastFungusMessage("BreakTheTree");
                    UnityEngine.Debug.Log("BreakTheTree");
                }
            }
        }
    }

}