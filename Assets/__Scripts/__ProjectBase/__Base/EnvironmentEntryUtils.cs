using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

# if UNITY_5_6_OR_NEWER
using UnityEngine;
# endif

/// <summary>
/// Environment特性
/// 用于标记Environment类的类型，实现自动映射
/// 
/// 使用示例：
/// [Environment(EnvironmentType.Game)]
/// public class GameEnvironment : BaseEnvironment<GameEnvironment>
/// {
///     // ...
/// }
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class EnvironmentAttribute : Attribute
{
    public EnvironmentType Type { get; }
        
    public EnvironmentAttribute(EnvironmentType type)
    {
        Type = type;
    }
}

public class EnvironmentEntryUtils
{
    // ===== Static变量（需要在清理时重置）=====
    private static Dictionary<EnvironmentType, Type> _environmentTypeMap;

    /// <summary>
    /// 清理缓存
    /// </summary>
    public static void ClearCache()
    {
        _environmentTypeMap?.Clear();
        _environmentTypeMap = null;
    }
    
    /// <summary>
    /// 扫描所有标记了[Environment]特性的类
    /// 只扫描项目相关的程序集，避免扫描Unity和第三方程序集
    /// </summary>
    public static Dictionary<EnvironmentType, Type> ScanEnvironments()
    {
        _environmentTypeMap = new Dictionary<EnvironmentType, Type>();
        
        try
        {
            // 只扫描项目相关程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Assembly-CSharp"));
            
            int foundCount = 0;
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => 
                            t.IsClass && 
                            !t.IsAbstract && 
                            typeof(IEnvironment).IsAssignableFrom(t)
                        );
                    
                    foreach (var type in types)
                    {
                        var attr = type.GetCustomAttribute<EnvironmentAttribute>();
                        if (attr != null)
                        {
                            _environmentTypeMap[attr.Type] = type;
                            foundCount++;
# if UNITY_5_6_OR_NEWER
                            Debug.Log($"<color=cyan>[EnvironmentEntry]</color> Found Environment: {type.Name} ({attr.Type})");
# endif
                        }
                    }
                }
                catch (Exception e)
                {
                    // 忽略无法访问的程序集
# if UNITY_5_6_OR_NEWER
                    Debug.LogWarning($"<color=cyan>[EnvironmentEntry]</color> Error scanning assembly {assembly.FullName}: {e.Message}");
# endif
                    return _environmentTypeMap;
                }
            }
# if UNITY_5_6_OR_NEWER
            Debug.Log($"<color=cyan>[EnvironmentEntry]</color> Scan complete, found {foundCount} Environment(s)");
# endif
            return _environmentTypeMap;
        }
        catch (Exception e)
        {
# if UNITY_5_6_OR_NEWER
            Debug.LogError($"<color=cyan>[EnvironmentEntry]</color> Fatal error during scan: {e.Message}\n{e.StackTrace}");
# endif
            return _environmentTypeMap;
        }
    }
    
    /// <summary>
    /// 初始化Environment（静态方法）
    /// </summary>
    public static IEnvironment InitializeEnvironment(EnvironmentType type)
    {
        // 创建并初始化新的Environment
        var env = CreateAndInitializeEnvironment(type);
        if (env == null)
        {
# if UNITY_5_6_OR_NEWER
            Debug.LogError($"<color=cyan>[EnvironmentEntry]</color> Failed to create Environment for type: {type}");
# endif
            return null;
        }
        return env;
    }
    
    /// <summary>
    /// 关闭Architecture（静态方法）
    /// </summary>
    public static void ShutdownEnvironment(IEnvironment env)
    {
        // 调用Environment的Shutdown方法
        if (env == null)
        {
# if UNITY_5_6_OR_NEWER
            Debug.LogError($"<color=cyan>[EnvironmentEntry]</color> Failed to shutdown Environment: {env.GetType().Name}");
# endif
            return;
        }
        env.Shutdown();
    }
    
    /// <summary>
    /// 创建Environment实例
    /// </summary>
    public static IEnvironment CreateAndInitializeEnvironment(EnvironmentType type)
    {
        if (_environmentTypeMap == null)
        {
#if UNITY_5_6_OR_NEWER
            Debug.LogError("<color=cyan>[EnvironmentEntry]</color> Environment type map is null, rescan...");
#endif
            ScanEnvironments();
        }
    
        if (_environmentTypeMap.TryGetValue(type, out var envType))
        {
            try
            {
                // 直接通过反射创建实例
                var instance = Activator.CreateInstance(envType) as IEnvironment;
                if (instance == null)
                {
#if UNITY_5_6_OR_NEWER
                    Debug.LogError($"<color=cyan>[EnvironmentEntry]</color> Failed to create instance of {envType.Name}");
#endif
                    return null;
                }
            
                // 初始化实例
                instance.Init();

                instance.Initialized = true;
            
#if UNITY_5_6_OR_NEWER
                Debug.Log($"<color=cyan>[EnvironmentEntry]</color> Created and initialized Environment: {envType.Name}");
#endif
                return instance;
            }
            catch (Exception e)
            {
#if UNITY_5_6_OR_NEWER
                Debug.LogError($"<color=cyan>[EnvironmentEntry]</color> Error creating Environment {envType.Name}: {e.Message}\n{e.StackTrace}"); 
#endif
                return null;
            }
        }
        else
        {
#if UNITY_5_6_OR_NEWER
            Debug.LogError($"<color=cyan>[EnvironmentEntry]</color> No Environment found for type: {type}. Available types: {string.Join(", ", _environmentTypeMap.Keys)}");   
#endif
            return null;
        }
    }
}