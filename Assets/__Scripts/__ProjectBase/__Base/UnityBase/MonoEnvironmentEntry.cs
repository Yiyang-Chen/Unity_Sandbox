# if UNITY_5_6_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Environment绑定器
/// 职责：
/// 1. 自动扫描并映射所有标记了[Environment]特性的类
/// 2. 在游戏启动时自动初始化对应的Environment
/// 3. 支持在场景中配置使用哪种Environment
/// 4. 支持禁用Domain Reload
/// 
/// 使用方法：
/// 在场景中放置此组件，配置EnvironmentType
///
/// TODO: 支持动态增减environment类型
/// 
/// </summary>
public class MonoEnvironmentEntry : MonoBehaviour
{
    [Header("Environment Configuration")]
    [Tooltip("选择要使用的Environment类型")]
    [SerializeField] 
    private List<EnvironmentType> _environmentTypes = new List<EnvironmentType> { EnvironmentType.Game };
    private Dictionary<EnvironmentType, IEnvironment> _environments = new Dictionary<EnvironmentType, IEnvironment>();
    
    // ===== Static变量（需要在清理时重置）=====
    private static bool _hasInitialized = false;
    
    // ===== Unity生命周期 =====
    
    /// <summary>
    /// Unity启动时自动初始化整个环境
    /// 使用BeforeSceneLoad确保在任何场景加载前初始化
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInit()
    {
        if (_hasInitialized)
        {
            Debug.LogWarning("<color=cyan>[EnvironmentEntry]</color> Already initialized");
            return;
        }
        
        _hasInitialized = true;
        
        Debug.Log("<color=cyan>[EnvironmentEntry]</color> Auto initializing...");
        
        // 1. 查找场景中的Entry组件
        var entry = FindObjectOfType<MonoEnvironmentEntry>(true);
        
        if (entry != null)
        {
            Debug.Log($"<color=cyan>[EnvironmentEntry]</color> Found entry in scene, using {string.Join(", ", entry._environmentTypes)}");
            // 3. 查找所有标记了[Environment]特性的类
            EnvironmentEntryUtils.ScanEnvironments();
            // 4. 初始化配置的Environment
            entry.InitializeChosenEnvironments();
        }
        else
        {
            // 2. 如果场景中没有Entry，不加载Environment
            Debug.Log("<color=cyan>[EnvironmentEntry]</color> No entry in scene, do not load any Environment");
        }
        
        // 使用原ProjectBase测试过的最稳定方法
        #if UNITY_EDITOR
        // ✅ 监听Assembly重新加载（处理禁用Domain Reload）
        UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        #endif
        // ✅ 监听应用退出
        Application.quitting += OnApplicationQuitting;
        
        Debug.Log("<color=cyan>[EnvironmentEntry]</color> Initialization complete");
    }
    
    private void Awake()
    {
        // 确保只有一个Entry实例
        var  entries = FindObjectsOfType<MonoEnvironmentEntry>();
        if ( entries.Length > 1)
        {
            Debug.LogWarning("<color=cyan>[EnvironmentEntry]</color> Multiple environment entries found, destroying duplicate");
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    /// <summary>
    /// 初始化Environment（实例方法）
    /// </summary>
    public void InitializeChosenEnvironments()
    {
        foreach (var environmentType in _environmentTypes)
        {
            IEnvironment env =EnvironmentEntryUtils.InitializeEnvironment(environmentType);
            if (env != null)
            {
                _environments[environmentType] = env;
            }
        }
    }
    
    // ===== 清理逻辑（支持禁用Domain Reload）=====
        
    private static void OnApplicationQuitting()
    {
        Debug.Log("<color=cyan>[EnvironmentEntry]</color> Application quitting, shutting down...");
        Shutdown();
    }
    
    #if UNITY_EDITOR
    private static void OnBeforeAssemblyReload()
    {
        Debug.Log("<color=cyan>[EnvironmentEntry]</color> Assembly reloading, shutting down...");
        Shutdown();
    }
    #endif
    
    /// <summary>
    /// 关闭Environment并清理所有static变量
    /// ✅ 关键：支持禁用Domain Reload
    /// </summary>
    private static void Shutdown()
    {
        Debug.Log("<color=cyan>[EnvironmentEntry]</color> Starting shutdown sequence...");
        
        // 1. 关闭管理的所有Environment（Environment负责清理它注册的所有System）
        var entry = FindObjectOfType<MonoEnvironmentEntry>(true);
        
        if (entry != null)
        {
            Debug.Log($"<color=cyan>[EnvironmentEntry]</color> Found entry in scene, using {string.Join(", ", entry._environmentTypes)}");
            // 3. 关闭配置的Environment
            entry.ShutdownChosenEnvironments();
        }
        else
        {
            // 2. 如果场景中没有Entry，关闭Environment
            Debug.Log("<color=cyan>[EnvironmentEntry]</color> No entry in scene, do not load any Environment");
        }
        
        // 2. 清理EnvironmentEntry自己的static变量
        EnvironmentEntryUtils.ClearCache();
        _hasInitialized = false;
        
        // 3. 取消事件监听
        #if UNITY_EDITOR
        UnityEditor.AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        #endif
        Application.quitting -= OnApplicationQuitting;
        
        Debug.Log("<color=cyan>[EnvironmentEntry]</color> Shutdown complete");
    }
    
    /// <summary>
    /// 关闭Environment（实例方法）
    /// </summary>
    public void ShutdownChosenEnvironments()
    {
        foreach (var environmentType in _environmentTypes)
        {
            IEnvironment env = _environments[environmentType];
            EnvironmentEntryUtils.ShutdownEnvironment(env);
        }
    }
    
    // ===== 编辑器辅助方法 =====
    
    #if UNITY_EDITOR
    [ContextMenu("Rescan Environments")]
    private void RescanEnvironments()
    {
        var environmentTypeMap = EnvironmentEntryUtils.ScanEnvironments();
        Debug.Log($"<color=cyan>[EnvironmentEntry]</color> Rescan complete. Found: {string.Join(", ", environmentTypeMap.Keys)}");
    }
    #endif
}
# endif
