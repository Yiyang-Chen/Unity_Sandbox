using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISystem : ICanInit, IBelongToEnvironment, ICanSetEnvironment, ICanGetSystem, IHasController
{
    bool ManualShutDown { get; set; }
}


/// <summary>
/// 单例基类
/// 使用方法：
/// 1. 继承 T : System<T>
/// 2. 实现 OnInit() 和 OnShutdown()-可选
/// 3. 通过 TEnv.Instance.GetSystem<T>() 访问
/// </summary>
public abstract class System<T> : ISystem where T : System<T>, new()
{
    private IEnvironment _mEnvironment;
    protected IController _mController;

    IEnvironment IBelongToEnvironment.GetEnvironment() => _mEnvironment;

    void ICanSetEnvironment.SetEnvironment(IEnvironment environment) => _mEnvironment = environment;

    //默认情况下，系统会在环境初始化时自动初始化，并在环境关闭时自动关闭
    //如果需要手动关闭，可以设置 ManualShutDown 为 true
    public bool ManualShutDown { get; set; }

    public bool Initialized { get; set; }
        
    void ICanInit.Init()
    {
        if (Initialized)
        {
            return;
        }
        OnInit();
#if UNITY_5_6_OR_NEWER
        Debug.Log($"<color=green>[System<{GetType().Name}>]</color> Init");
#endif
        Initialized = true;
    }

    void ICanInit.Shutdown()
    {
        if (!Initialized)
        {
            return;
        }

        _mController?.Shutdown();
        _mController = null;
        
        OnShutdown();

        _mEnvironment = null;
        
#if UNITY_5_6_OR_NEWER
        Debug.Log($"<color=green>[System<{GetType().Name}>]</color> Shutdown");
#endif
        Initialized = false;
    }

    protected abstract void OnShutdown();

    protected abstract void OnInit();
    

    public void RegisterController(IController controller) => _mController = controller;

    
    public IController GetController() => _mController;
}