using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring: MonoBehaviour
{
    public float jumpForce = 33f;
    // Start is called before the first frame update
    public GameObject LineLight;
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {


            LineLight.SetActive(true);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }


}
