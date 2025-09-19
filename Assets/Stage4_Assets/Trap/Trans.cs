using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trans: MonoBehaviour
{
    
    public GameObject Door;
    public Transform Player;
    public Transform doorposition;
    private Vector3 Nowposition;
    public float DelayTime = 2.5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke("positionchange", DelayTime);
            Door.SetActive(false);
            Invoke("change", 6.5f);
        }
    }

    private void positionchange()
    {
        Player.transform.position = Nowposition;
        
    }

    private void change()
    {
        Door.SetActive(true);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Nowposition = doorposition.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}