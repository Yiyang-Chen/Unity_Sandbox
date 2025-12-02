using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private Dictionary<Type, BaseComponent> componentDict = new Dictionary<Type, BaseComponent>();
    private bool isDisposed;

    [InfoBox("如果不是默认值，那ID就是实体的唯一ID，否则会自动生成一个ID")]
    public int id = EntitySystem.DEFAULT_ID;
    [HideInInspector]
    public EventComponent eventComponent;
    [HideInInspector]
    public LogComponent logComponent;
    
    [SerializeField]
    protected float radius = 0.5f;
    [SerializeField]
    protected float height = 2f;

    [SerializeField] 
    protected bool manualCreate = false;
    
    private void Awake()
    {
        if (manualCreate)
        {
            Init();
            MinimalEnvironment.Instance.GetSystem<EntitySystem>().OnEntityCreated(this);
        }
    }

    private void Start()
    {
        if (manualCreate)
        {
            PostInit();
        }
    }
    
    public void Init()
    {
        isDisposed = false;
        
        foreach (var component in GetComponents<BaseComponent>())
        {
            componentDict.Add(component.GetType(), component);
        }
        eventComponent = GetCustomComponent<EventComponent>();
        logComponent = GetCustomComponent<LogComponent>();

        var componentList = new List<BaseComponent>(componentDict.Values);
        
        foreach (var component in componentList)
        {
            component.Init();
        }
    }

    public void PostInit()
    {
        var componentList = new List<BaseComponent>(componentDict.Values);
        foreach (var component in componentList)
        {
            component.PostInit();
        }
        
        eventComponent.EventTrigger(EEntityEvent.OnCreated);
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }
        foreach (var component in componentDict.Values)
        {
            if(component.GetType() == typeof(EventComponent) || component.GetType() == typeof(LogComponent))
                continue;
            TryDisposeComponent(component);
        }
        TryDisposeComponent(eventComponent);
        TryDisposeComponent(logComponent);
        MinimalEnvironment.Instance.GetSystem<EntitySystem>().OnEntityDestroyed(this);
        
        Destroy(gameObject);
        isDisposed = true;
    }

    private void OnDestroy()
    {
        if (!isDisposed)
        {
            Dispose();
        }
    }
    
    public T GetCustomComponent<T>() where T : BaseComponent
    {
        Type componentType = typeof(T);
        if (componentDict.ContainsKey(componentType))
        {
            return (T)componentDict[componentType];
        }
        return GetComponent<T>();
    }
    
    public virtual Vector3 GetHeadPosition()
    {
        return transform.position + new Vector3(0, height * 0.8f, 0);
    }

    //TODO: 应该要放到TransformComponent里面
    public virtual Vector3 GetFrontPosition()
    {
        return transform.position + new Vector3(0, height * 0.8f, 0) + GetForwardDirection() * radius * 1.2f;
    }

    public virtual Vector3 GetForwardDirection()
    {
        return transform.forward;
    }

    private void TryDisposeComponent(BaseComponent component)
    {
        try
        {
            component.Dispose();
        }
        catch (Exception e)
        {
            MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"Entity {id} dispose component error: {e.ToString()}");
        }
    }
    
}
