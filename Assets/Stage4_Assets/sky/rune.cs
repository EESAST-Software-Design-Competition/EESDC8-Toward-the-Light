using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class rune : MonoBehaviour
{
    public Flowchart flowchart; // �������Flowchart
    [SerializeField] private string blockName = "rune"; // ָ��Ҫ������Block����

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �����ײ�����Ƿ�����ң�ͨ��Tag��Layer��
        if (other.CompareTag("Player"))
        {
            // ����Fungus�Ի�
            flowchart.ExecuteBlock(blockName);
            

            // ��ѡ��������ײ������ظ�����
            GetComponent<Collider2D>().enabled = false;
        }
    }
}