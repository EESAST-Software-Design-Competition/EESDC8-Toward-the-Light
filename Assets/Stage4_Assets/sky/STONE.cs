using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STONE: MonoBehaviour
{
    public Rigidbody2D rb;
    
    public float fadeSpeed = 0.01f; // ���뵭���ٶ�
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer spritesky;
    private bool isFadingIn = false;
    private Color currentColor;
    private Color currentColor0;
    public ParticleSystem particle;
    double a = 0;

    void Start()
    {
        // ��ʼ��Rigidbody2D���
        rb = GetComponent<Rigidbody2D>();

        // ����Ƿ�ɹ���ȡRigidbody2D���
        if (rb == null)
        {
            UnityEngine.Debug.LogError("Rigidbody2D δ�ҵ���");
        }

        if (spriteRenderer == null)
        {
            UnityEngine.Debug.LogError("SpriteRenderer δ�ҵ���");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Fade();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �����ײ�����Ƿ������
        if (collision.gameObject.CompareTag("Player"))
        {


            // �޸���ɫ
            if (spriteRenderer != null)
            {

                spriteRenderer.color = Color.yellow; // �޸�Ϊ��ɫ
                particle.Play(); 
                Invoke("change", 0.5f);
            }

        }
    }

    private void change()
    {
        currentColor = spritesky.color;
        currentColor0 = spritesky.color;
        a = currentColor0.a * 1.5 + 0.015;
        if (a > 1) a = 1;
        isFadingIn = true;
        
        Destroy(rb);
    }


    private void Fade()
    {
        if (spritesky != null)
        {



            if (isFadingIn)
            {

                if (currentColor.a < a)
                {

                    currentColor.a += fadeSpeed * Time.deltaTime;
                    spritesky.color = currentColor;



                }
                if (currentColor.a >= a)
                {
                    isFadingIn = false;
                }
                if (currentColor.a >= 1)
                {
                    currentColor.a = 1;
                    // Ӧ���µ���ɫ
                    spritesky.color = currentColor;
                    isFadingIn = false;

                }
            }
        }
    }
}