#if UNITY_5_6_OR_NEWER
using UnityEngine;
#endif

[Environment(EnvironmentType.Minimal)]
public class MinimalEnvironment: BaseEnvironment<MinimalEnvironment>
{
    //如果只有自己一个实例，可以设置为单例
    public MinimalEnvironment()
    {
        SetInstance(this);
    }
    
    protected override void OnInit()
    {
        // 注册 System（按 SingletonManager 的顺序）
        // ManualShutDown 已在各 System 的构造函数中设置
        RegisterSystem(new LogSystem());
        GetSystem<LogSystem>().ManualShutDown = true;
        RegisterSystem(new MonoSystem());
        GetSystem<MonoSystem>().ManualShutDown = true;
        RegisterSystem(new EventSystem());
        GetSystem<EventSystem>().ManualShutDown = true;
        RegisterSystem(new ResourceSystem());
        RegisterSystem(new PoolSystem());
        RegisterSystem(new SceneSystem());
        RegisterSystem(new InputSystem());
        RegisterSystem(new DataSystem());
        GetSystem<DataSystem>().ManualShutDown = true;
        RegisterSystem(new UISystem());
        RegisterSystem(new GameStateSystem());
        
#if UNITY_5_6_OR_NEWER
        Debug.Log($"<color=lime>[MinimalEnvironment]</color> Init");
#endif
    }
    
    protected override void PreShutdown()
    {
#if UNITY_5_6_OR_NEWER
        Debug.Log($"<color=lime>[MinimalEnvironment]</color> PreShutdown");
#endif
        
        // 触发关闭事件（在手动关闭 EventCenter 之前）
        var eventCenter = GetSystem<EventSystem>();
        if (eventCenter != null)
        {
            eventCenter.EventTrigger(EGlobalEvent.OnShutdown);
        }
    }
    
    protected override void PostShutdown()
    {
        // 按照 SingletonManager 的顺序手动关闭需要特殊处理的 System
        // 关闭顺序：DataMgr → EventCenter → MonoManager → LogManager
        ShutdownSystem(GetSystem<DataSystem>());
        ShutdownSystem(GetSystem<EventSystem>());
        ShutdownSystem(GetSystem<MonoSystem>());
        ShutdownSystem(GetSystem<LogSystem>());
        
#if UNITY_5_6_OR_NEWER
        Debug.Log($"<color=lime>[MinimalEnvironment]</color> PostShutdown");
#endif
    }
}