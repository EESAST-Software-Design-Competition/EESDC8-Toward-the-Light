using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private SceneTransitioner sceneTransitioner;
    
    // 用于检测世界对象点击的变量
    private bool isWorldObject = false;
    private Collider2D boxCollider2D;
    private Collider boxCollider3D;
    
    private void Start()
    {
        
            boxCollider2D = GetComponent<Collider2D>();
            
            if (boxCollider2D != null)
            {
                isWorldObject = true;
            }
            else
            {
                Debug.LogError("ButtonController: 这个游戏对象既没有Button组件，也没有任何Collider组件用于点击检测!");
            }
        
        // 如果没有在Inspector中分配，则尝试查找场景中的SceneTransitioner
        if (sceneTransitioner == null)
        {
            sceneTransitioner = FindObjectOfType<SceneTransitioner>();
            
            if (sceneTransitioner == null)
            {
                Debug.LogError("ButtonController: 场景中找不到SceneTransitioner组件!");
            }
        }
    }
    
    // private void Update()
    // {
    //     // 只有当是世界对象时才检测鼠标点击
    //     if (isWorldObject && Input.GetMouseButtonDown(0))
    //     {
    //         CheckWorldObjectClick();
    //     }
    // }
    //
    // private void CheckWorldObjectClick()
    // {
    //
    //         Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         
    //         if (boxCollider2D.OverlapPoint(mousePosition))
    //         {
    //             OnButtonClicked();
    //         }
    // }
    //
    private void OnMouseDown()
    {
        OnButtonClicked();
    }

    private void OnButtonClicked()
    {
        Debug.Log("ButtonController: 按钮被点击!");
        
        // 调用场景切换
        if (sceneTransitioner != null)
        {
            sceneTransitioner.StartSceneTransition();
        }
    }
    
    private void OnDestroy()
    {
        // 如果有Button组件，清理事件监听器
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}