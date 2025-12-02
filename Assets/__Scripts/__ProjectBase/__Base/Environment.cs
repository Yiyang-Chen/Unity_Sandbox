using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_5_6_OR_NEWER
using UnityEngine;
#endif

public interface IEnvironment : ICanInit
{
    void RegisterSystem<T>(T system) where T : ISystem;

    T GetSystem<T>() where T : class, ISystem;
}

public abstract class BaseEnvironment<T> : IEnvironment where T : BaseEnvironment<T>, new()
{
    // ICanInit
    public bool Initialized { get; set; }
    // 存储ISystem的容器
    private IOCContainer mContainer;

    //TODO: 使用非单例以支持多个相同的Environment实例
    #region Singleton
    // 单例模式，线程安全
    private static T mEnvironment;
    private static readonly object lockObj = new object();
    
    public static T Instance
    {
        get
        {
            if (mEnvironment == null)
            {
#if UNITY_5_6_OR_NEWER
                Debug.LogError($"<color=red>[Environment]</color> {typeof(T).Name} is not initialized");
#endif
                return null;
            }
            return mEnvironment;
        }
    }

    public static void SetInstance(T instance)
    {
        mEnvironment = instance;
    }

    protected BaseEnvironment() { }
    #endregion Singleton

    #region ICanInit

    void ICanInit.Init()
    {
        mContainer = new IOCContainer();
        OnInit();
        
        foreach (var system in mContainer.GetInstancesByType<ISystem>()
                     .Where(s => !s.Initialized))
        {
            system.Init();
        }
    }

    void ICanInit.Shutdown()
    {
        PreShutdown();

        foreach (var system in mContainer.GetInstancesByType<ISystem>()
            .Where(s => s.Initialized)
            .Where(s => !s.ManualShutDown))
        {
            ShutdownSystem(system);
        }
        PostShutdown();
        mContainer.Clear();
        
        // 线程安全，清空单例引用
        lock (lockObj)
        {
            mEnvironment = null;
        }
    }

    protected void ShutdownSystem(ISystem system)
    {
        if(system == null){
#if UNITY_5_6_OR_NEWER
            Debug.LogWarning($"<color=red>[Environment]</color> system to shutdown is null");
#endif
            return;
        }
        try
        {
            system.Shutdown();
        }
        catch (System.Exception)
        {
#if UNITY_5_6_OR_NEWER
            Debug.LogError($"<color=red>[Environment]</color> error when shutting down system {system.GetType().Name}");
#endif
            throw;
        }
        
    }
    
    // ===== 需要子类实现 =====
    
    /// <summary>
    /// 初始化环境，子类必须实现，可以在这里注册系统
    /// </summary>
    protected abstract void OnInit();

    protected abstract void PreShutdown();

    protected abstract void PostShutdown();
    
    // ===== /需要子类实现 =====

    #endregion
    
    public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
    {
        system.SetEnvironment(this);
        mContainer.Register<TSystem>(system);

        if (Initialized && !system.Initialized)
        {
            system.Init();
        }
    }
    
    public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
    {
        return mContainer.Get<TSystem>();
    }
}