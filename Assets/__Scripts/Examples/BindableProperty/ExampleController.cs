#if UNITY_5_6_OR_NEWER

using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 示例 Controller，展示如何使用 BindableProperty 绑定 IData 字段
/// 
/// 核心特性：
/// 1. BindableProperty 通过 BindTo() 直接绑定到 IData 的字段
/// 2. 在 Inspector 中修改 BindableProperty → 直接修改 IData 的字段
/// 3. Inspector 中显示的值是实时读取 IData 字段的当前值
/// 4. 根据 IData 类型自动控制可编辑性：
///    - IConfigData: Editor 可编辑，Play 只读（灰色）
///    - IRuntimeModel: 任何时候都可编辑
/// 
/// 重要：绑定时机
/// - OnValidate(): Editor 模式下绑定配置数据，这样在不运行 Play 也能在 Inspector 中编辑
/// - Awake(): Play 模式开始时绑定所有数据，此时配置数据会变为只读
/// - 配置数据在 OnValidate 中就绑定好了，进入 Play 模式时会自动变为只读（灰色）
/// </summary>
public class ExampleController : UnityController
{
    [Title("Static Config Data")]
    [InfoBox("配置数据：可以在 Inspector 中选择，或从 Resources 自动加载", InfoMessageType.Info)]
    [SerializeField]
    [AssetSelector(Paths = "Assets/Resources")]
    private ExampleConfigData _config;
    
    [SerializeField]
    [Tooltip("如果 Inspector 中没有指定配置，则从此路径加载（相对于 Resources 文件夹）")]
    private string _configPath = "Configs/ExampleConfig";

    [Title("Runtime Data")]
    [InfoBox("运行时数据：存储游戏运行时的状态", InfoMessageType.Info)]
    [ReadOnly]
    [ShowInInspector]
    private ExampleRuntimeData _runtimeData;

    [FoldoutGroup("Runtime Properties")]
    [SerializeField]
    private BindableProperty<int> health = new();

    [FoldoutGroup("Runtime Properties")]
    [SerializeField]
    private BindableProperty<int> mana = new();

    [FoldoutGroup("Config Properties")]
    [SerializeField]
    private BindableProperty<int> maxCount = new();

    [FoldoutGroup("Config Properties")]
    [SerializeField]
    private BindableProperty<bool> enableAdvancedFeatures = new();

    [Title("Verification - IData Fields")]
    [FoldoutGroup("IData Fields")]
    [ShowInInspector, ReadOnly]
    [LabelText("Runtime: Health")]
    private int RuntimeHealth => _runtimeData?.health ?? 0;

    [FoldoutGroup("IData Fields")]
    [ShowInInspector, ReadOnly]
    [LabelText("Runtime: Mana")]
    private int RuntimeMana => _runtimeData?.mana ?? 0;

    [FoldoutGroup("IData Fields")]
    [ShowInInspector, ReadOnly]
    [LabelText("Config: Max Count")]
    private int ConfigMaxCount => _config?.maxCount ?? 0;

    [FoldoutGroup("IData Fields")]
    [ShowInInspector, ReadOnly]
    [LabelText("Config: Enable Advanced")]
    private bool ConfigEnableAdvanced => _config?.enableAdvancedFeatures ?? false;

    protected override void OnInit()
    {
        _runtimeData = new ExampleRuntimeData();
        RegisterEventListeners();
    }

    protected override void OnShutdown()
    {
        health.OnValueChanged -= OnPlayerHealthChanged;
    }

    /// <summary>
    /// 绑定配置数据字段（IConfigData - Editor 可编辑，Play 只读）
    /// 
    /// 演示两种绑定方式：
    /// 1. 自动绑定：一行代码批量绑定所有字段（推荐）
    /// 2. 手动绑定：逐个字段绑定（保留作为对比）
    /// </summary>
    protected override void BindStaticData()
    {
        if (_config != null)
        {
            // ===== 方式1：自动绑定（推荐）=====
            // 自动匹配并绑定 Controller 和 Data 中同名的字段
            // 要求：字段名完全一样，类型匹配
            int autoBindCount = this.AutoBindProperties(_config);
            
            if (autoBindCount > 0)
            {
                Debug.Log($"[ExampleController] Auto-bound {autoBindCount} config fields in {(Application.isPlaying ? "Play" : "Editor")} mode");
            }
            
            // ===== 方式2：手动绑定（保留作为对比）=====
            // 如果你想要更精细的控制，或者字段名不一样，可以手动绑定
            // Label 会自动设置为字段名，不需要手动调用 WithLabel
            // 注释掉的代码展示了手动绑定的方式：
            /*
            if (!maxCount.IsBound)
            {
                maxCount
                    .BindTo(
                        () => _config.maxCount,
                        value => _config.maxCount = value
                    )
                    .BelongsTo(_config);
                    // Label 已自动设置为 "Max Count (int)"
                    // 如需自定义：.WithLabel("自定义名称");
            }

            if (!enableAdvancedFeatures.IsBound)
            {
                enableAdvancedFeatures
                    .BindTo(
                        () => _config.enableAdvancedFeatures,
                        value => _config.enableAdvancedFeatures = value
                    )
                    .BelongsTo(_config);
                    // Label 已自动设置为 "Enable Advanced Features (bool)"
            }
            */
        }
    }

    /// <summary>
    /// 绑定运行时数据字段（IRuntimeModel - 总是可编辑）
    /// </summary>
    protected override void BindRuntimeData()
    {
        if (_runtimeData != null)
        {
            // 使用自动绑定
            int autoBindCount = this.AutoBindProperties(_runtimeData);
            
            if (autoBindCount > 0)
            {
                Debug.Log($"[ExampleController] Auto-bound {autoBindCount} runtime fields");
            }
            
            // 手动绑定方式（注释掉，保留作为参考）
            // Label 会自动设置，不需要手动调用 WithLabel
            /*
            if (!health.IsBound)
            {
                health
                    .BindTo(
                        () => _runtimeData.health,
                        value => _runtimeData.health = value
                    )
                    .BelongsTo(_runtimeData);
                    // Label 已自动设置为 "Health (int)"

                mana
                    .BindTo(
                        () => _runtimeData.mana,
                        value => _runtimeData.mana = value
                    )
                    .BelongsTo(_runtimeData);
                    // Label 已自动设置为 "Mana (int)"
            }
            */
        }
    }

    /// <summary>
    /// 注册事件监听
    /// </summary>
    private void RegisterEventListeners()
    {
        // 监听值变更（只在 Play 模式）
        if (Application.isPlaying)
        {
            health.OnValueChanged += OnPlayerHealthChanged;
        }
    }

    [Title("Actions")]
    [Button("Modify Runtime Health (-10)", ButtonSizes.Large)]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    private void ModifyRuntimeHealth()
    {
        health.Value -= 10;
        Debug.Log($"[Runtime] Health: {health.Value} (IData field: {_runtimeData.health})");
    }

    [Button("Modify Runtime Mana (+5)", ButtonSizes.Large)]
    [GUIColor(0.3f, 0.8f, 0.8f)]
    private void ModifyRuntimeMana()
    {
        mana.Value += 5;
        Debug.Log($"[Runtime] Mana: {mana.Value} (IData field: {_runtimeData.mana})");
    }

    [Button("Try Modify Config (Will be blocked in Play mode)", ButtonSizes.Large)]
    [GUIColor(0.8f, 0.8f, 0.3f)]
    private void TryModifyConfig()
    {
        if (!Application.isPlaying)
        {
            maxCount.Value = 999;
            Debug.Log($"[Config] Max Count modified to: {maxCount.Value}");
        }
        else
        {
            Debug.LogWarning("[Config] Cannot modify config in Play mode! (Field is read-only)");
        }
    }

    [Button("Load Config from Resources", ButtonSizes.Medium)]
    [ShowIf("@_config == null")]
    private void LoadConfig()
    {
        if (!string.IsNullOrEmpty(_configPath))
        {
            _config = Resources.Load<ExampleConfigData>(_configPath);
            if (_config != null)
            {
                BindStaticData(); // 重新绑定配置
                Debug.Log("[ExampleController] Config loaded and bound");
            }
            else
            {
                Debug.LogError($"[ExampleController] Failed to load config from: {_configPath}");
            }
        }
    }

    [Button("Verify Binding: Check IData Fields", ButtonSizes.Medium)]
    [InfoBox("点击后查看 Console，确认 BindableProperty.Value 和 IData 字段的值一致")]
    private void VerifyBinding()
    {
        Debug.Log("=== Binding Verification ===");
        Debug.Log($"health.Value = {health.Value}, _runtimeData.health = {_runtimeData.health}");
        Debug.Log($"mana.Value = {mana.Value}, _runtimeData.mana = {_runtimeData.mana}");
        
        if (_config != null)
        {
            Debug.Log($"maxCount.Value = {maxCount.Value}, config.maxCount = {_config.maxCount}");
            Debug.Log($"enableAdvancedFeatures.Value = {enableAdvancedFeatures.Value}, config.enableAdvancedFeatures = {_config.enableAdvancedFeatures}");
        }
    }

    private void OnPlayerHealthChanged(int oldValue, int newValue)
    {
        Debug.Log($"[Event] Player Health changed: {oldValue} → {newValue}");
    }
}

#endif

