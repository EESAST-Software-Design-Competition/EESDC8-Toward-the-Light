using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fail : MonoBehaviour
{
    public float fallDelay = 1.0f;
    public float destroyDelay = 1.5f;
    public Rigidbody2D rb;
    public GameObject Falling;
    public Rigidbody2D Player;
    private Vector3 Platformposition;
    public Transform Nowposition;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        Invoke("Falling.SetActive(false)",destroyDelay);
    }
    // Start is called before the first frame update
    void Start()
    {
        Platformposition = Nowposition.transform.position;
    }

    public void OnPlatformReset()
    {
            Invoke("tran", destroyDelay);
            
            rb.bodyType = RigidbodyType2D.Static;
            Falling.SetActive(true);
    }

    private void tran()
    {
        rb.transform.position = Platformposition;
    }
    
}
