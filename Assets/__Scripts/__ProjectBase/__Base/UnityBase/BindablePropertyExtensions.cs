#if UNITY_5_6_OR_NEWER

using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// BindableProperty 扩展方法，提供批量自动绑定功能
/// </summary>
public static class BindablePropertyExtensions
{
    /// <summary>
    /// 自动为 Controller 的所有 BindableProperty 设置默认 Label（基于字段名）
    /// 应该在绑定之前调用
    /// </summary>
    /// <param name="controller">Controller 实例</param>
    public static void AutoSetPropertyLabels(this IController controller)
    {
        if (controller == null) return;

        Type controllerType = controller.GetType();
        
        // 获取 Controller 的所有字段（包括私有字段）
        FieldInfo[] fields = controllerType.GetFields(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        foreach (FieldInfo field in fields)
        {
            // 检查是否是 BindableProperty<T> 类型
            if (!IsBindablePropertyType(field.FieldType))
                continue;

            // 获取 BindableProperty 实例
            object bindableProperty = field.GetValue(controller);
            if (bindableProperty == null)
                continue;

            // 获取 BindableProperty<T> 的泛型参数 T
            Type propertyValueType = field.FieldType.GetGenericArguments()[0];
            
            // 生成友好的 Label
            string friendlyName = SplitCamelCase(field.Name);
            string typeName = GetFriendlyTypeName(propertyValueType);
            string label = $"{friendlyName} ({typeName})";

            // 设置 Label
            MethodInfo withLabelMethod = field.FieldType.GetMethod("WithLabel");
            if (withLabelMethod != null)
            {
                withLabelMethod.Invoke(bindableProperty, new object[] { label });
            }
        }
    }

    /// <summary>
    /// 自动绑定 Controller 的 BindableProperty 字段到 Data 对象的字段
    /// 
    /// 规则：
    /// 1. 字段名完全匹配（区分大小写）
    /// 2. 类型必须匹配（BindableProperty&lt;T&gt; 的 T 与 Data 字段类型一致）
    /// 3. 自动生成 Label（如 "maxHealth" → "Max Health (int)"）
    /// 4. 类型不匹配时跳过并警告
    /// </summary>
    /// <param name="controller">Controller 实例（通常传 this）</param>
    /// <param name="data">要绑定的 Data 对象</param>
    /// <param name="autoLabel">是否自动设置 Label（默认 true）</param>
    /// <returns>成功绑定的数量</returns>
    public static int AutoBindProperties(this IController controller, IData data, bool autoLabel = true)
    {
        if (controller == null)
        {
            Debug.LogError("[AutoBindProperties] Controller is null");
            return 0;
        }

        if (data == null)
        {
            Debug.LogError("[AutoBindProperties] Data is null");
            return 0;
        }

        int bindCount = 0;
        Type controllerType = controller.GetType();
        Type dataType = data.GetType();

        // 获取 Controller 的所有字段（包括私有字段）
        FieldInfo[] controllerFields = controllerType.GetFields(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        // 获取 Data 的所有公开字段
        FieldInfo[] dataFields = dataType.GetFields(
            BindingFlags.Instance | BindingFlags.Public
        );

        // 遍历 Controller 的所有字段
        foreach (FieldInfo controllerField in controllerFields)
        {
            // 检查是否是 BindableProperty<T> 类型
            if (!IsBindablePropertyType(controllerField.FieldType))
                continue;

            // 获取 BindableProperty<T> 的泛型参数 T
            Type propertyValueType = controllerField.FieldType.GetGenericArguments()[0];
            string fieldName = controllerField.Name;

            // 在 Data 中查找同名字段
            FieldInfo dataField = Array.Find(dataFields, f => f.Name == fieldName);
            if (dataField == null)
                continue;

            // 检查类型是否匹配
            if (dataField.FieldType != propertyValueType)
            {
                Debug.LogWarning(
                    $"[AutoBindProperties] Type mismatch for field '{fieldName}': " +
                    $"BindableProperty<{propertyValueType.Name}> vs Data field type {dataField.FieldType.Name}. Skipped."
                );
                continue;
            }

            // 获取 BindableProperty 实例
            object bindableProperty = controllerField.GetValue(controller);
            if (bindableProperty == null)
            {
                Debug.LogWarning($"[AutoBindProperties] Field '{fieldName}' is null. Skipped.");
                continue;
            }

            // 检查是否已经绑定
            PropertyInfo isBoundProperty = controllerField.FieldType.GetProperty("IsBound");
            if (isBoundProperty != null && (bool)isBoundProperty.GetValue(bindableProperty))
            {
                // 已经绑定，跳过
                continue;
            }

            // 执行绑定
            bool success = BindProperty(
                bindableProperty,
                controllerField.FieldType,
                data,
                dataField,
                propertyValueType,
                autoLabel
            );

            if (success)
            {
                bindCount++;
            }
        }

        return bindCount;
    }

    /// <summary>
    /// 检查类型是否是 BindableProperty&lt;T&gt;
    /// </summary>
    private static bool IsBindablePropertyType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(BindableProperty<>);
    }

    /// <summary>
    /// 执行绑定操作
    /// </summary>
    private static bool BindProperty(
        object bindableProperty,
        Type bindablePropertyType,
        IData data,
        FieldInfo dataField,
        Type valueType,
        bool autoLabel
    )
    {
        try
        {
            // 创建 Getter 委托：() => data.field
            Delegate getter = CreateGetter(data, dataField, valueType);

            // 创建 Setter 委托：value => data.field = value
            Delegate setter = CreateSetter(data, dataField, valueType);

            // 调用 BindTo(getter, setter)
            MethodInfo bindToMethod = bindablePropertyType.GetMethod("BindTo");
            if (bindToMethod == null)
            {
                Debug.LogError($"[AutoBindProperties] BindTo method not found for {bindablePropertyType.Name}");
                return false;
            }

            bindToMethod.Invoke(bindableProperty, new object[] { getter, setter });

            // 调用 BelongsTo(data)
            MethodInfo belongsToMethod = bindablePropertyType.GetMethod(
                "BelongsTo",
                new[] { typeof(IData) }
            );
            if (belongsToMethod != null)
            {
                belongsToMethod.Invoke(bindableProperty, new object[] { data });
            }

            // 设置 Label
            if (autoLabel)
            {
                string label = GenerateFriendlyLabel(dataField.Name, valueType);
                MethodInfo withLabelMethod = bindablePropertyType.GetMethod("WithLabel");
                if (withLabelMethod != null)
                {
                    withLabelMethod.Invoke(bindableProperty, new object[] { label });
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AutoBindProperties] Failed to bind field '{dataField.Name}': {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 创建 Getter 委托
    /// </summary>
    private static Delegate CreateGetter(IData data, FieldInfo field, Type valueType)
    {
        Type funcType = typeof(Func<>).MakeGenericType(valueType);
        
        // 创建 () => data.field
        MethodInfo getMethod = typeof(BindablePropertyExtensions)
            .GetMethod(nameof(CreateGetterImpl), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(field.DeclaringType, valueType);
        
        return (Delegate)getMethod.Invoke(null, new object[] { data, field });
    }

    private static Func<TValue> CreateGetterImpl<TData, TValue>(TData data, FieldInfo field)
    {
        return () => (TValue)field.GetValue(data);
    }

    /// <summary>
    /// 创建 Setter 委托
    /// </summary>
    private static Delegate CreateSetter(IData data, FieldInfo field, Type valueType)
    {
        Type actionType = typeof(Action<>).MakeGenericType(valueType);
        
        // 创建 value => data.field = value
        MethodInfo setMethod = typeof(BindablePropertyExtensions)
            .GetMethod(nameof(CreateSetterImpl), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(field.DeclaringType, valueType);
        
        return (Delegate)setMethod.Invoke(null, new object[] { data, field });
    }

    private static Action<TValue> CreateSetterImpl<TData, TValue>(TData data, FieldInfo field)
    {
        return value => field.SetValue(data, value);
    }

    /// <summary>
    /// 生成友好的 Label 名称
    /// 例如：maxHealth → "Max Health (int)"
    /// </summary>
    private static string GenerateFriendlyLabel(string fieldName, Type valueType)
    {
        // 将 camelCase 转换为 "Camel Case"
        string friendlyName = SplitCamelCase(fieldName);
        
        // 添加类型信息
        string typeName = GetFriendlyTypeName(valueType);
        
        return $"{friendlyName} ({typeName})";
    }

    /// <summary>
    /// 将 camelCase 拆分为单词
    /// 例如：maxHealth → "Max Health"
    /// </summary>
    private static string SplitCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // 使用简单的规则：在大写字母前插入空格
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            // 首字母大写
            if (i == 0)
            {
                result.Append(char.ToUpper(c));
            }
            // 如果是大写字母且不是第一个字符，前面加空格
            else if (char.IsUpper(c))
            {
                result.Append(' ');
                result.Append(c);
            }
            else
            {
                result.Append(c);
            }
        }
        
        return result.ToString();
    }

    /// <summary>
    /// 获取友好的类型名称
    /// </summary>
    private static string GetFriendlyTypeName(Type type)
    {
        if (type == typeof(int)) return "int";
        if (type == typeof(float)) return "float";
        if (type == typeof(double)) return "double";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(string)) return "string";
        if (type == typeof(long)) return "long";
        if (type == typeof(short)) return "short";
        if (type == typeof(byte)) return "byte";
        if (type == typeof(uint)) return "uint";
        if (type == typeof(ulong)) return "ulong";
        if (type == typeof(ushort)) return "ushort";
        if (type == typeof(sbyte)) return "sbyte";
        if (type == typeof(char)) return "char";
        if (type == typeof(decimal)) return "decimal";
        
        // Vector 类型
        if (type == typeof(Vector2)) return "Vector2";
        if (type == typeof(Vector3)) return "Vector3";
        if (type == typeof(Vector4)) return "Vector4";
        if (type == typeof(Quaternion)) return "Quaternion";
        if (type == typeof(Color)) return "Color";
        
        // 其他类型直接返回类型名
        return type.Name;
    }
}

#endif

