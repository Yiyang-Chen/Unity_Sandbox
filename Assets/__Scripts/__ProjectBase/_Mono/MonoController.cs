using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Mono的管理者
/// </summary>
public class MonoController : UnityController
{
    private event UnityAction startEvent;
    private event UnityAction updateEvent;

    protected override void OnInit()
    {
        
    }

    protected void Start()
    {
        if (startEvent != null) startEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null) updateEvent();
    }

    protected override void OnShutdown()
    {
        
    }

    public void AddStartListener(UnityAction fun)
    {
        //___Singletons.Log.Debug("add start fun");
        startEvent += fun;
    }

    public void RemoveStartListener(UnityAction fun)
    {
        startEvent -= fun;
    }

    /// <summary>
    /// 添加帧更新事件的函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }
    /// <summary>
    /// 移除帧更新事件的函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }

    public void _DontDestoryOnLoad(GameObject obj)
    {
        DontDestroyOnLoad(obj);
    }

    public void _Destory(Object obj)
    {
        Destroy(obj);
    }
}
