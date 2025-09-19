using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cameraTransform;
    void Start()
    {
        transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, transform.position.z);
    }
}
