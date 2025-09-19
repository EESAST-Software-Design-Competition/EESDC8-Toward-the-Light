using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CreateBackgroundShift : MonoBehaviour
{
    public List<GameObject> backgroundList;
    // 添加视差系数数组，用于控制每层背景的移动速度
    public List<float> parallaxEffects;

    private List<List<GameObject>> _currentBackgroundObjects;
    private int _layerCount;
    private Camera _camera;
    private float _cameraWidth;
    private Vector3 _cameraLastPosition;
    
    void Awake()
    {
        _camera = Camera.main;
        _cameraLastPosition = _camera.transform.position;
        _cameraWidth = _camera.orthographicSize * _camera.aspect * 2; // 正确计算摄像机宽度
        _layerCount = backgroundList.Count;
        
        // 初始化_currentBackgroundObjects列表
        _currentBackgroundObjects = new List<List<GameObject>>();
        
        // 确保视差系数列表长度与背景列表相同
        if (parallaxEffects == null || parallaxEffects.Count != _layerCount)
        {
            parallaxEffects = new List<float>();
            for (int i = 0; i < _layerCount; i++)
            {
                // 默认视差效果：前景层移动更快，背景层移动更慢
                parallaxEffects.Add(1.0f - (float)i / _layerCount);
            }
        }
        
        // 为每一层背景初始化对象列表并创建初始背景
        for (int i = 0; i < _layerCount; i++)
        {
            _currentBackgroundObjects.Add(new List<GameObject>());
            GameObject currentBackgroundObject = Instantiate(backgroundList[i], Vector3.zero, Quaternion.identity);
            currentBackgroundObject.transform.SetParent(transform);
            _currentBackgroundObjects[i].Add(currentBackgroundObject);
            
            // 初始化时创建足够的背景覆盖屏幕
            float objectWidth = currentBackgroundObject.GetComponent<Renderer>().bounds.size.x;
            int requiredObjects = Mathf.CeilToInt(_cameraWidth / objectWidth) + 1;
            
            // 创建初始背景对象
            for (int j = 0; j < requiredObjects; j++)
            {
                CreateObject(i, true);
            }
        }
    }

    private void LateUpdate()
    {
        // 计算摄像机移动的方向和距离
        Vector3 cameraDelta = _camera.transform.position - _cameraLastPosition;
        
        // 更新每一层背景
        for (int i = 0; i < _layerCount; i++)
        {
            // 根据视差系数移动当前层的所有背景对象
            float parallaxFactor = parallaxEffects[i];
            foreach (GameObject bg in _currentBackgroundObjects[i])
            {
                bg.transform.position = new Vector3(bg.transform.position.x + cameraDelta.x * parallaxFactor, _camera.transform.position.y, 0);
            }
            
            // 检查是否需要在右侧创建新背景
            GameObject rightmostBg = _currentBackgroundObjects[i][_currentBackgroundObjects[i].Count - 1];
            float rightEdge = rightmostBg.transform.position.x + rightmostBg.GetComponent<Renderer>().bounds.extents.x;
            float cameraRightEdge = _camera.transform.position.x + _cameraWidth / 2;
            
            if (rightEdge < cameraRightEdge)
            {
                CreateObject(i, true);
            }
            
            // 检查是否需要在左侧创建新背景
            GameObject leftmostBg = _currentBackgroundObjects[i][0];
            float leftEdge = leftmostBg.transform.position.x - leftmostBg.GetComponent<Renderer>().bounds.extents.x;
            float cameraLeftEdge = _camera.transform.position.x - _cameraWidth / 2;
            
            if (leftEdge > cameraLeftEdge)
            {
                CreateObject(i, false);
            }
            
            // 移除不再可见的背景对象以节省内存
            CleanupOffscreenObjects(i);
        }
        
        // 更新摄像机位置记录
        _cameraLastPosition = _camera.transform.position;
    }

    private void CreateObject(int layer, bool front)
    {
        GameObject lastBackgroundObject = _currentBackgroundObjects[layer][front ? _currentBackgroundObjects[layer].Count - 1 : 0];
        float objectWidth = lastBackgroundObject.GetComponent<Renderer>().bounds.size.x;
        Vector3 createPos = lastBackgroundObject.transform.position + objectWidth *
            (front ? Vector3.right : Vector3.left);
        GameObject currentBackgroundObject = Instantiate(backgroundList[layer], createPos, Quaternion.identity);
        currentBackgroundObject.transform.SetParent(transform);
        if (front)
        {
            _currentBackgroundObjects[layer].Add(currentBackgroundObject);
        }
        else
        {
            _currentBackgroundObjects[layer].Insert(0, currentBackgroundObject);
        }
    }
    
    private void CleanupOffscreenObjects(int layer)
    {
        // 移除右侧超出视野范围的背景对象
        while (_currentBackgroundObjects[layer].Count > 3) // 保留至少3个对象以确保平滑过渡
        {
            GameObject leftmostBg = _currentBackgroundObjects[layer][0];
            float rightEdge = leftmostBg.transform.position.x + leftmostBg.GetComponent<Renderer>().bounds.extents.x;
            float cameraLeftEdge = _camera.transform.position.x - _cameraWidth / 2 - leftmostBg.GetComponent<Renderer>().bounds.size.x;
            
            if (rightEdge < cameraLeftEdge)
            {
                Destroy(leftmostBg);
                _currentBackgroundObjects[layer].RemoveAt(0);
            }
            else
            {
                break;
            }
        }
        
        // 移除左侧超出视野范围的背景对象
        while (_currentBackgroundObjects[layer].Count > 3)
        {
            GameObject rightmostBg = _currentBackgroundObjects[layer][_currentBackgroundObjects[layer].Count - 1];
            float leftEdge = rightmostBg.transform.position.x - rightmostBg.GetComponent<Renderer>().bounds.extents.x;
            float cameraRightEdge = _camera.transform.position.x + _cameraWidth / 2 + rightmostBg.GetComponent<Renderer>().bounds.size.x;
            
            if (leftEdge > cameraRightEdge)
            {
                Destroy(rightmostBg);
                _currentBackgroundObjects[layer].RemoveAt(_currentBackgroundObjects[layer].Count - 1);
            }
            else
            {
                break;
            }
        }
    }
}
