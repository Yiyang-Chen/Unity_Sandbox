#if UNITY_5_6_OR_NEWER

using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 可绑定的属性，绑定到 IData 的字段，根据 IData 类型自动控制可编辑性
/// </summary>
[Serializable]
[InlineProperty]
[HideLabel]
public class BindableProperty<T>
{
    [SerializeField]
    [HideInInspector]
    private EDataEditMode _editMode = EDataEditMode.Auto;

    [SerializeField]
    [HideInInspector]
    private string _labelText = "Value";
    
    private Type _ownerType;
    private Func<T> _getter;
    private Action<T> _setter;

    /// <summary>
    /// 获取/设置值
    /// </summary>
    [ShowInInspector]
    [EnableIf(nameof(IsEditable))]
    [LabelText("$_labelText")]
    [HideLabel]
    public T Value
    {
        get
        {
            if (_getter != null)
            {
                return _getter();  // ← 实时读取 IData 字段
            }
            return default(T);
        }
        set
        {
            if (_setter != null)
            {
                T oldValue = _getter != null ? _getter() : default(T);
                _setter(value);  // ← 直接修改 IData 字段
                OnValueChanged?.Invoke(oldValue, value);
            }
        }
    }

    /// <summary>
    /// 值变更事件
    /// </summary>
    public event Action<T, T> OnValueChanged;

    /// <summary>
    /// 构造函数
    /// </summary>
    public BindableProperty() { }

    /// <summary>
    /// 绑定到 IData 的字段
    /// </summary>
    /// <param name="getter">字段的 Getter（() => data.field）</param>
    /// <param name="setter">字段的 Setter（value => data.field = value）</param>
    public BindableProperty<T> BindTo(Func<T> getter, Action<T> setter)
    {
        _getter = getter;
        _setter = setter;
        return this;
    }

    /// <summary>
    /// 设置标签文本
    /// </summary>
    public BindableProperty<T> WithLabel(string label)
    {
        _labelText = label;
        return this;
    }

    /// <summary>
    /// 设置所属的 IData（用于判断编辑模式）
    /// </summary>
    public BindableProperty<T> BelongsTo(IData owner)
    {
        _ownerType = owner.GetType();
        return this;
    }
    
    /// <summary>
    /// 设置所属的 IData 类型（用于判断编辑模式）
    /// </summary>
    public BindableProperty<T> BelongsTo(Type ownerType)
    {
        _ownerType = ownerType;
        return this;
    }

    /// <summary>
    /// 强制设置编辑模式
    /// </summary>
    public BindableProperty<T> WithEditMode(EDataEditMode mode)
    {
        _editMode = mode;
        return this;
    }

    /// <summary>
    /// 是否已绑定
    /// </summary>
    public bool IsBound => _getter != null && _setter != null;

    /// <summary>
    /// 是否可编辑
    /// </summary>
    private bool IsEditable()
    {
        // 未绑定时不可编辑
        if (!IsBound) return false;

        // 强制设置的编辑模式
        if (_editMode == EDataEditMode.AlwaysEditable) return true;
        if (_editMode == EDataEditMode.AlwaysReadOnly) return false;

        // 自动判断模式（根据 IData 类型）
        if (_ownerType != null)
        {
            // IConfigData 只能在 Editor 模式编辑
            if (typeof(IConfigData).IsAssignableFrom(_ownerType))
            {
                return !Application.isPlaying;
            }
            
            // ITableData 永远只读
            if (typeof(ITableData).IsAssignableFrom(_ownerType))
            {
                return false;
            }
            
            // 其他 IStaticData Editor 模式可编辑，Play 模式只读
            if (typeof(IStaticData).IsAssignableFrom(_ownerType))
            {
                return !Application.isPlaying;
            }
            
            // IRuntimeModel 只能在 Play 模式编辑
            if (typeof(IRuntimeModel).IsAssignableFrom(_ownerType))
            {
                return Application.isPlaying;
            }
        }

        // 默认：Editor 模式可编辑，Play 模式只读
        return !Application.isPlaying;
    }

    // 隐式转换
    public static implicit operator T(BindableProperty<T> property) => property.Value;
}

/// <summary>
/// 数据编辑模式
/// </summary>
public enum EDataEditMode
{
    /// <summary>
    /// 自动判断（根据 IData 类型和运行模式）
    /// </summary>
    Auto = 0,
    
    /// <summary>
    /// 总是可编辑
    /// </summary>
    AlwaysEditable = 1,
    
    /// <summary>
    /// 总是只读
    /// </summary>
    AlwaysReadOnly = 2
}

#endif

