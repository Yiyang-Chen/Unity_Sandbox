# BindableProperty 使用说明

> 将 Controller 的字段直接绑定到 IData 的字段，根据 IData 类型自动控制可编辑性

---

## 🎯 核心特性

- **字段级绑定**：通过 Getter/Setter Lambda 绑定到 IData 的具体字段
- **直接修改 IData**：在 Inspector 修改 BindableProperty = 直接修改 IData 字段
- **实时显示**：显示 IData 字段的当前值
- **✨ 自动批量绑定**：一行代码自动绑定所有同名字段（推荐）
- **自动控制可编辑性**：
  - `IConfigData`：Editor 可编辑，Play 只读（灰色）
  - `IRuntimeModel`：任何时候都可编辑
  - `ITableData`：永远只读

---

## 📝 快速示例

### 方式1：自动绑定（✨ 推荐）

```csharp
public class PlayerController : UnityController
{
    // IData 对象
    [SerializeField, AssetSelector]
    private PlayerConfigData _config;
    
    [SerializeField]
    private PlayerRuntimeData _runtime = new();

    // BindableProperty（字段名必须与 Data 中的字段名完全一致）
    [SerializeField] private BindableProperty<int> maxHealth = new();
    [SerializeField] private BindableProperty<int> health = new();

    protected override void BindStaticData()
    {
        // 一行代码自动绑定所有同名字段
        this.AutoBindProperties(_config);
    }

    protected override void BindRuntimeData()
    {
        this.AutoBindProperties(_runtime);
        
        // 监听变更
        health.OnValueChanged += (old, newVal) =>
        {
            if (newVal <= 0) Die();
        };
    }
}
```

### 方式2：手动绑定（精细控制）

```csharp
public class PlayerController : UnityController
{
    [SerializeField] private PlayerConfigData _config;
    [SerializeField] private PlayerRuntimeData _runtime = new();
    
    // 字段名可以不同
    [SerializeField] private BindableProperty<int> _maxHP = new();
    [SerializeField] private BindableProperty<int> _currentHP = new();

    protected override void BindStaticData()
    {
        _maxHP
            .BindTo(() => _config.maxHealth, v => _config.maxHealth = v)
            .BelongsTo(_config)
            .WithLabel("Max HP (int)");
    }

    protected override void BindRuntimeData()
    {
        _currentHP
            .BindTo(() => _runtime.health, v => _runtime.health = v)
            .BelongsTo(_runtime)
            .WithLabel("Current HP (int)");
    }
}
```

---

## 🔧 API

### 自动绑定（推荐）

```csharp
// 扩展方法：自动绑定所有同名字段
int count = this.AutoBindProperties(IData data, bool autoLabel = true);

// 使用示例
this.AutoBindProperties(_config);        // 绑定配置数据
this.AutoBindProperties(_runtimeData);   // 绑定运行时数据

// 返回值：成功绑定的字段数量
```

**匹配规则**：
- 字段名必须完全一致（区分大小写）
- 类型必须匹配（`BindableProperty<int>` 只能绑定 `int` 字段）
- 自动生成 Label：`maxHealth` → `"Max Health (int)"`
- 类型不匹配时跳过并警告

### 手动绑定

```csharp
// 绑定到字段
.BindTo(Func<T> getter, Action<T> setter)

// 设置所属 IData（决定可编辑性）
.BelongsTo(IData owner)

// 设置标签
.WithLabel(string label)

// 强制编辑模式（可选）
.WithEditMode(EDataEditMode mode)
```

### 属性

```csharp
T Value { get; set; }                  // 获取/设置值
bool IsBound { get; }                   // 是否已绑定
event Action<T, T> OnValueChanged;      // 值变更事件
```

---

## ⚙️ 绑定时机

### Editor 模式：使用 OnValidate()

```csharp
#if UNITY_EDITOR
private void OnValidate()
{
    if (!Application.isPlaying)
    {
        BindConfigData();  // 绑定配置数据
    }
}
#endif
```

**作用**：在不运行 Play 时也能在 Inspector 中编辑配置

### Play 模式：使用 Awake()

```csharp
void Awake()
{
    BindConfigData();    // 绑定配置（进入 Play 后自动变只读）
    BindRuntimeData();   // 绑定运行时数据
}
```

**作用**：确保 Play 模式下所有数据都已绑定

### 避免重复绑定

```csharp
if (_config != null && !_maxHealth.IsBound)
{
    _maxHealth.BindTo(/* ... */);
}
```

---

## 💡 常见用法

### 1. 自动绑定多个字段（推荐）

```csharp
// Data 定义
public class PlayerConfigData : StaticDataScriptableObject
{
    public int maxHealth;
    public int maxMana;
    public float moveSpeed;
    public string playerName;
}

// Controller 定义（字段名必须与 Data 完全一致）
public class PlayerController : UnityController
{
    [SerializeField] private PlayerConfigData _config;
    
    [SerializeField] private BindableProperty<int> maxHealth = new();
    [SerializeField] private BindableProperty<int> maxMana = new();
    [SerializeField] private BindableProperty<float> moveSpeed = new();
    [SerializeField] private BindableProperty<string> playerName = new();
    
    protected override void BindStaticData()
    {
        // 一行代码绑定所有字段
        this.AutoBindProperties(_config);
        // Inspector 会显示：
        // - Max Health (int)
        // - Max Mana (int)
        // - Move Speed (float)
        // - Player Name (string)
    }
}
```

### 2. 手动绑定（字段名不同时）

```csharp
[SerializeField] private PlayerConfigData _config;
[SerializeField] private BindableProperty<int> _maxHP = new();  // 字段名不同

protected override void BindStaticData()
{
    _maxHP
        .BindTo(() => _config.maxHealth, v => _config.maxHealth = v)
        .BelongsTo(_config)
        .WithLabel("Max HP (int)");
}
```

### 3. 监听值变更

```csharp
[SerializeField] private BindableProperty<int> health = new();

protected override void BindRuntimeData()
{
    this.AutoBindProperties(_runtime);
    
    // 绑定后监听变更
    health.OnValueChanged += (oldVal, newVal) =>
    {
        Debug.Log($"Health: {oldVal} → {newVal}");
        if (newVal <= 0) Die();
    };
}
```

### 4. 混合使用自动绑定和手动绑定

```csharp
protected override void BindStaticData()
{
    // 自动绑定大部分字段
    this.AutoBindProperties(_config);
    
    // 手动绑定需要特殊处理的字段
    specialField
        .BindTo(() => _config.special, v => _config.special = v)
        .BelongsTo(_config)
        .WithLabel("Special Field")
        .WithEditMode(EDataEditMode.AlwaysEditable);
}
```

### 5. 绑定嵌套字段（只能手动）

```csharp
// 自动绑定无法处理嵌套字段，需要手动绑定
playerName.BindTo(
    () => _data.playerInfo.name,
    v => _data.playerInfo.name = v
);
```

---

## 📋 完整示例模板

```csharp
public class MyController : UnityController
{
    // === IData 对象 ===
    [SerializeField, AssetSelector]
    private MyConfigData _config;
    
    [SerializeField]
    private MyRuntimeData _runtime = new();
    
    // === BindableProperty ===
    [SerializeField]
    private BindableProperty<int> _configField = new();
    
    [SerializeField]
    private BindableProperty<int> _runtimeField = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            LoadConfigIfNeeded();
            BindConfigFields();
        }
    }
#endif

    void Awake()
    {
        LoadConfigIfNeeded();
        BindConfigFields();
        BindRuntimeFields();
        RegisterListeners();
    }

    void LoadConfigIfNeeded()
    {
        if (_config == null)
            _config = Resources.Load<MyConfigData>("Configs/MyConfig");
    }

    void BindConfigFields()
    {
        if (_config != null && !_configField.IsBound)
        {
            _configField
                .BindTo(() => _config.field, v => _config.field = v)
                .BelongsTo(_config);
        }
    }

    void BindRuntimeFields()
    {
        if (_runtime != null && !_runtimeField.IsBound)
        {
            _runtimeField
                .BindTo(() => _runtime.field, v => _runtime.field = v)
                .BelongsTo(_runtime);
        }
    }

    void RegisterListeners()
    {
        if (Application.isPlaying)
        {
            _runtimeField.OnValueChanged += OnFieldChanged;
        }
    }

    void OnDestroy()
    {
        _runtimeField.OnValueChanged -= OnFieldChanged;
    }
}
```

---

## 🎨 Inspector 效果

**Editor 模式（不运行 Play）**：
- 配置字段：白色，可编辑
- 运行时字段：白色，可编辑

**Play 模式**：
- 配置字段：灰色，只读
- 运行时字段：白色，可编辑

---

## 🚨 注意事项

### 1. 自动绑定的要求

**字段名必须完全一致**：
```csharp
// ✅ 正确
Data: public int maxHealth;
Controller: private BindableProperty<int> maxHealth = new();

// ❌ 错误 - 名称不同
Data: public int maxHealth;
Controller: private BindableProperty<int> _maxHP = new();  // 名称不匹配

// ❌ 错误 - 大小写不同
Data: public int MaxHealth;
Controller: private BindableProperty<int> maxHealth = new();  // 大小写不匹配
```

**类型必须匹配**：
```csharp
// ✅ 正确
Data: public int health;
Controller: private BindableProperty<int> health = new();

// ❌ 错误 - 类型不匹配
Data: public int health;
Controller: private BindableProperty<float> health = new();  // 跳过并警告
```

### 2. Lambda 无法序列化

- BindableProperty 可序列化
- 但 Getter/Setter Lambda 不会保存
- 需要在 `BindStaticData()` / `BindRuntimeData()` 中重新绑定

### 3. 绑定时机

- **配置数据**：在 `BindStaticData()` 中绑定
- **运行时数据**：在 `BindRuntimeData()` 中绑定
- **事件监听**：在 `OnInit()` 中注册

### 4. 自动绑定会跳过已绑定的字段

无需手动检查 `IsBound`，`AutoBindProperties()` 会自动跳过已绑定的字段。

### 5. 性能

- **自动绑定**：使用反射，只在初始化时执行一次，性能影响可忽略
- **运行时访问**：通过 Lambda 委托，接近直接访问性能
- 比反射快 100+ 倍

### 6. 何时使用手动绑定

以下情况需要手动绑定：
- 字段名不同
- 绑定嵌套字段（如 `data.info.name`）
- 绑定属性方法（如 `GetHealth()` / `SetHealth()`）
- 需要自定义 Label 或编辑模式

---

## 🎓 完整使用流程

### Step 1：定义 Data

```csharp
public class PlayerConfigData : StaticDataScriptableObject
{
    public int maxHealth = 100;
    public float moveSpeed = 5.0f;
    public string playerName = "Player";
}

public class PlayerRuntimeData : IRuntimeModel
{
    public int health = 100;
    public int mana = 50;
}
```

### Step 2：在 Controller 中声明 BindableProperty

```csharp
public class PlayerController : UnityController
{
    [SerializeField] private PlayerConfigData _config;
    private PlayerRuntimeData _runtime;
    
    // 字段名必须与 Data 完全一致
    [SerializeField] private BindableProperty<int> maxHealth = new();
    [SerializeField] private BindableProperty<float> moveSpeed = new();
    [SerializeField] private BindableProperty<string> playerName = new();
    
    [SerializeField] private BindableProperty<int> health = new();
    [SerializeField] private BindableProperty<int> mana = new();
```

### Step 3：绑定数据

```csharp
    protected override void OnInit()
    {
        _runtime = new PlayerRuntimeData();
    }
    
    protected override void BindStaticData()
    {
        this.AutoBindProperties(_config);  // 自动绑定 3 个字段
    }
    
    protected override void BindRuntimeData()
    {
        this.AutoBindProperties(_runtime);  // 自动绑定 2 个字段
        
        // 监听变更
        health.OnValueChanged += OnHealthChanged;
    }
}
```

### Step 4：在 Inspector 中编辑

- **Editor 模式**：配置字段可编辑
- **Play 模式**：配置字段只读，运行时字段可编辑

---

## 📚 参考

- **示例**：`Assets/__Scripts/Examples/BindableProperty/ExampleController.cs`
- **源码**：
  - `Assets/__Scripts/__ProjectBase/__Base/UnityBase/BindableProperty.cs`
  - `Assets/__Scripts/__ProjectBase/__Base/UnityBase/BindablePropertyExtensions.cs`
- **数据接口**：`Assets/__Scripts/__ProjectBase/__Base/Data.cs`

