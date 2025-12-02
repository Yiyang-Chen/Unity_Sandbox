using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

/// <summary>
/// MonoBehaviour 生命周期管理器
/// 提供统一的 Update、Coroutine 等 Unity 生命周期能力
/// </summary>
public class MonoSystem : System<MonoSystem>
{
    private MonoController controller;
    
    private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

    // TODO: 配置 MonoController Prefab 路径
    private const string MONO_CONTROLLER_PREFAB_PATH = "MonoController";

    protected override void OnInit()
    {
        // 先尝试在场景中查找
        controller = GameObject.FindObjectOfType<MonoController>();
        
        // 如果场景中没有，从 Prefab 加载
        if (controller == null)
        {
            var prefab = Resources.Load<GameObject>(MONO_CONTROLLER_PREFAB_PATH);
            if (prefab != null)
            {
                var go = GameObject.Instantiate(prefab);
                controller = go.GetComponent<MonoController>();
                Debug.Log($"<color=cyan>[MonoManager]</color> MonoController loaded from prefab");
            }
            else
            {
                Debug.LogWarning($"[MonoManager] MonoController prefab not found at path: {MONO_CONTROLLER_PREFAB_PATH}");
            }
        }
        
        if (controller != null)
        {
            RegisterController(controller);
            AddUpdateListener(Update);
        }
    }

    protected override void OnShutdown()
    {
        if (controller != null)
        {
            RemoveUpdateListener(Update);
        }
        controller = null;
    }

    //Awake and enable are not be able to do before controller start

    public void AddStartListener(UnityAction fun)
    {
        controller.AddStartListener(fun);
    }
    
    public void RemoveStartListener(UnityAction fun)
    {
        controller.RemoveStartListener(fun);
    }

    public void AddUpdateListener(UnityAction fun)
    {
        controller.AddUpdateListener(fun);
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        controller.RemoveUpdateListener(fun);
    }
    //这个只能运行在controller里有的函数
    public Coroutine StartCoroutine(string methodName)
    {
        return controller.StartCoroutine(methodName);
    }
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }

    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(methodName, value);
    }

    public void StopAllCoroutines()
    {
        controller.StopAllCoroutines();
    }

    public void StopCoroutine(IEnumerator routine)
    {
        controller.StopCoroutine(routine);
    }

    public void StopCoroutine(Coroutine routine)
    {
        controller.StopCoroutine(routine);
    }

    public void StopCoroutine(string methodName)
    {
        controller.StopCoroutine(methodName);
    }

    public void DontDestoryOnLoad(GameObject obj)
    {
        controller._DontDestoryOnLoad(obj);
    }

    public void Destory(UnityEngine.Object obj)
    {
        controller._Destory(obj);
    }

    //子线程调用主线程代码
    public void Enqueue(Action action)
    {
        if (action == null) return;

        _executionQueue.Enqueue(action);
    }
    
    void Update()
    {
        // 在主线程中执行所有排队的任务
        while (_executionQueue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

}
