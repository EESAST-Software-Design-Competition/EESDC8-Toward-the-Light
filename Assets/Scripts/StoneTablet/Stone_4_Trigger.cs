using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone_4_Trigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Fungus.Flowchart.BroadcastFungusMessage("Stone_4_Trigger");
                UnityEngine.Debug.Log("Stone_4_Trigger");
            }
        }
    }
}
