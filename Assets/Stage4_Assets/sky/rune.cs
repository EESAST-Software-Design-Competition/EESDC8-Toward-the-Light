using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class rune : MonoBehaviour
{
    public Flowchart flowchart; // 关联你的Flowchart
    [SerializeField] private string blockName = "rune"; // 指定要触发的Block名称

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞对象是否是玩家（通过Tag或Layer）
        if (other.CompareTag("Player"))
        {
            // 触发Fungus对话
            flowchart.ExecuteBlock(blockName);
            

            // 可选：禁用碰撞体避免重复触发
            GetComponent<Collider2D>().enabled = false;
        }
    }
}