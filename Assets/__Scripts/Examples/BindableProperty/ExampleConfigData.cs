#if UNITY_5_6_OR_NEWER

using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 示例配置数据
/// 继承 ScriptableObject 和 IConfigData
/// 字段在 Editor 模式可编辑，Play 模式只读
/// </summary>
[CreateAssetMenu(fileName = "ExampleConfig", menuName = "ProjectBase/Example Config")]
public class ExampleConfigData : StaticDataScriptableObject
{
    [Title("Basic Settings")]
    [Tooltip("配置名称")]
    public string configName = "Example Config";

    [Tooltip("最大数量")]
    [Range(1, 100)]
    public int maxCount = 10;

    [Title("Advanced Settings")]
    [Tooltip("启用高级功能")]
    public bool enableAdvancedFeatures = false;

    [ShowIf(nameof(enableAdvancedFeatures))]
    [Tooltip("高级功能参数")]
    public float advancedParameter = 1.5f;
}

/// <summary>
/// 示例运行时数据
/// 实现 IRuntimeModel，任何时候都可以编辑
/// </summary>
[System.Serializable]
public class ExampleRuntimeData : IRuntimeModel
{
    [Title("Player State")]
    public int health = 100;
    
    [ProgressBar(0, 100)]
    public int mana = 50;
    
    [Title("Game State")]
    public bool isGameOver = false;
    
    [ShowIf(nameof(isGameOver))]
    public string gameOverReason = "";
}

#endif

