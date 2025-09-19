using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLog : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(_spriteRenderer.sprite.bounds.size.x);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
