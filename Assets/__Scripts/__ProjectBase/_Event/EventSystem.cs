using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



//事件中心
//Manage all the global events.
public class EventSystem : System<EventSystem>
{
    private EventHandler eventHandler;

    protected override void OnInit()
    {
        eventHandler = new EventHandler();
    }

    protected override void OnShutdown()
    {
        Clear();
        eventHandler = null;
    }
    
    /// <summary>
    /// 添加事件监听
    /// Add event listener with parameter.
    /// </summary>
    public void AddEventListener<T>(EGlobalEvent globalEventName, UnityAction<T> action)
    {
        eventHandler.AddEventListener((int)globalEventName, action);
    }
    /// <summary>
    /// 监听不需要参数的事件
    /// Add event listener without parameter.
    /// </summary>
    public void AddEventListener(EGlobalEvent globalEventName, UnityAction action)
    {
        eventHandler.AddEventListener((int)globalEventName, action);
    }
    /// <summary>
    /// 移除事件监听
    /// Remove event listener.
    /// Be sure to do this in case any bugs.
    /// <summary>
    public void RemoveEventListener<T>(EGlobalEvent globalEventName, UnityAction<T> action)
    {
        eventHandler.RemoveEventListener((int)globalEventName, action);
    }

    /// <summary>
    /// 移除不需要参数的事件
    /// Remove event listener.
    /// </summary>
    public void RemoveEventListener(EGlobalEvent globalEventName, UnityAction action)
    {
        eventHandler.RemoveEventListener((int)globalEventName, action);
    }
    /// <summary>
    /// 事件触发
    /// Trigger an event.
    /// Send the parameter with expected type.
    /// </summary>
    public void EventTrigger<T>(EGlobalEvent globalEventName, T info)
    {
        eventHandler.EventTrigger((int)globalEventName, info);
    }
    /// <summary>
    /// 触发不需要参数的事件
    /// Trigger an event.
    /// </summary>
    public void EventTrigger(EGlobalEvent globalEventName)
    {
        eventHandler.EventTrigger((int)globalEventName);
    }
    /// <summary>
    /// 清空事件中心,如场景切换时
    /// Clear all the events. 
    /// </summary>
    public void Clear()
    {
        eventHandler.Clear();
    }
}

