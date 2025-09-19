using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform: MonoBehaviour
{
    public GameObject[] points;
    public float speed = 2.0f;
    private int pointNum = 1;
    private float waitTime = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, points[pointNum].transform.position,speed*Time.deltaTime);

        if (Vector2.Distance(transform.position, points[pointNum].transform.position) < 0.1f)
        {
            if (waitTime < 0)
            {
                pointNum++;
                pointNum %= 2;
                waitTime = 0.5f;
            }
            else
                waitTime -= Time.deltaTime;
            
        }
    }


}
