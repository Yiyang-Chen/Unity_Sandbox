using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

// 基础面板
// 可以自动查找并管理子控件
// 提供显示和隐藏的回调函数

// The most basic panel, shold be attached to UI panels.
// Easily get access to all its controls.
// Can do something when the panel is showed or hidden.

public class BasePanel : MonoBehaviour, INodeParent
{
    [SerializeField] 
    protected bool disableInputOnOpen = false;
    
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();
    private bool isShow = false;
    public bool IsShow
    {
        get { return isShow; }
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        // 需要什么控件就在这里添加
        // What kind of controls you are going to use.
        // You can call more other controls in children classes.
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
    }

    /// <summary>
    /// 显示自己
    /// Do something when the panel is showed.
    /// </summary>
    public virtual void ShowPanel()
    {
        isShow = true;
        if (disableInputOnOpen)
        {
            MinimalEnvironment.Instance.GetSystem<InputSystem>().SwitchInputState(EInputState.DisableWithDebug);
        }
    }

    /// <summary>
    /// 隐藏自己
    /// Do something when the panel is hidden.
    /// </summary>
    public virtual void HidePanel()
    {
        isShow = false;
        if (disableInputOnOpen)
        {
            MinimalEnvironment.Instance.GetSystem<InputSystem>().SwitchInputState(EInputState.Default);
        }
    }

    /// <summary>
    /// Get the list of type T with exact name.
    /// </summary>
    protected List<T> GetControl<T>(string controlName) where T : UIBehaviour
    {
        List<T> returnArray = new List<T>();
        if (controlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < controlDic[controlName].Count; ++i)
            {
                if (controlDic[controlName][i] is T)
                    returnArray.Add(controlDic[controlName][i] as T);
            }
        }

        return returnArray;
    }

    //事件绑定basepanel示例，在FindChildrenControl函数中可以找到OnClick的调用位置
    //Example of how to write events in basepanel.
    //In FindChildrenControl function, you can find where the OnClick is called.
    /// <summary>
    /// 一个虚函数，用于在按钮点击时执行操作
    /// A virtual function to do things when a button is clicked.
    /// </summary>
    /// <param name="buttonName"></param>
    protected virtual void OnClick(string buttonName)
    {
    }

    protected virtual void OnValueChanged(string toggleName, bool value)
    {
    }

    /// <summary>
    /// Find children in Dic.
    /// </summary>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();

        for (int i = 0; i < controls.Length; ++i)
        {
            string objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
                controlDic[objName].Add(controls[i]);
            else
                controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });

            // 事件绑定示例
            // Example of how to write events in basepanel.
            if (controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(() => { OnClick(objName); });
            }
            else if (controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) => { OnValueChanged(objName, value); });
            }
        }
    }
    
    public virtual T GetPanel<T>() where T : BasePanel
    {
        Type panelType = GetType();
        if (panelType.IsSubclassOf(typeof(T)) || panelType == typeof(T))
        {
            return this as T;
        }
        else
        {
            MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"BasePanel: {this.GetType()} GetPanel: Cannot get panel of type " + typeof(T));
            return null;
        }
    }
}