#if UNITY_5_6_OR_NEWER
using System;
using UnityEngine;

public abstract class UnityController : MonoBehaviour, IController
{
    public bool Initialized { get; set; }
    
#if UNITY_EDITOR
    /// <summary>
    /// Editor 回调：在 Inspector 值改变时、脚本加载时、进入 Play 模式前调用
    /// 用于在 Editor 模式下就完成配置数据的绑定
    /// </summary>
    private void OnValidate()
    {
        // 在 Editor 模式下绑定static配置数据
        if (!Application.isPlaying)
        {
            // 自动为所有 BindableProperty 设置默认 Label
            this.AutoSetPropertyLabels();
            
            BindStaticData();
        }
    }
#endif
    
    protected virtual void Awake()
    {
        Init();
        
        // 自动为所有 BindableProperty 设置默认 Label
        this.AutoSetPropertyLabels();
        
        BindRuntimeData();
    }

    protected void OnDestroy()
    {
        Shutdown();
    }

    protected virtual void BindRuntimeData()
    {
        
    }

    protected virtual void BindStaticData()
    {
        
    }
    
    public void Init()
    {
        if (Initialized)
        {
            return;
        }
        OnInit();
        
        //找到最上层的parent
        Transform parent = transform;
        while (parent.parent != null)
        {
            parent = parent.parent;
        }
        DontDestroyOnLoad(parent.gameObject);
        
        Initialized = true;
    }

    public void Shutdown()
    {
        if (!Initialized)
        {
            return;
        }
        OnShutdown();
        Initialized = false;
    }
    
    protected abstract void OnInit();
    protected abstract void OnShutdown();
}
#endif
