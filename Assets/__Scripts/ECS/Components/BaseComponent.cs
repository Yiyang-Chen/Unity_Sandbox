using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseComponent : MonoBehaviour
{
    [HideInInspector]
    public Entity entity;

    private bool inited = false;
    private bool posetInited = false;
    private bool disposed = false;

    protected string LogComponentName
    {
        get
        {
            return "[" + GetType().Name + "]";
        }
    }
    
    private void OnDestroy()
    {
        if (!disposed)
        {
            Dispose();
        }
    }

    public void Init()
    {
        if (inited)
        {
            return;
        }
        if (!TryGetComponent<Entity>(out entity))
        {
            MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"{LogComponentName} 必须挂载在Entity上");
        }
        
        InitComponent();
        inited = true;
    }

    protected virtual void InitComponent()
    {
        
    }
    
    public void PostInit()
    {
        if (posetInited)
        {
            return;
        }
        PostInitComponent();
        posetInited = true;
    }
    
    protected virtual void PostInitComponent()
    {
        
    }

    //在Destroy前调用
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }
        DisposeComponent();
        disposed = true;
    }

    protected virtual void DisposeComponent()
    {
        
    }
}