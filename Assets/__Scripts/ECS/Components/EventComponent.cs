using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventComponent : BaseComponent
{
    private EventHandler eventHandler;

    protected override void InitComponent()
    {
        base.InitComponent();
        eventHandler = new EventHandler();
    }

    protected override void DisposeComponent()
    {
        base.DisposeComponent();
        Clear();
        eventHandler = null;
    }
    
    public void AddEventListener<T>(EEntityEvent eventName, UnityAction<T> action)
    {
        eventHandler?.AddEventListener((int)eventName, action);
    }

    public void AddEventListener(EEntityEvent eventName, UnityAction action)
    {
        eventHandler?.AddEventListener((int)eventName, action);
    }
    
    public void RemoveEventListener<T>(EEntityEvent eventName, UnityAction<T> action)
    {
        eventHandler?.RemoveEventListener((int)eventName, action);
    }
    
    public void RemoveEventListener(EEntityEvent eventName, UnityAction action)
    {
        eventHandler?.RemoveEventListener((int)eventName, action);
    }

    public void EventTrigger<T>(EEntityEvent eventName, T info)
    {
        eventHandler?.EventTrigger((int)eventName, info);
    }

    public void EventTrigger(EEntityEvent eventName)
    {
        eventHandler?.EventTrigger((int)eventName);
    }

    public void Clear()
    {
        eventHandler?.Clear();
    }
}