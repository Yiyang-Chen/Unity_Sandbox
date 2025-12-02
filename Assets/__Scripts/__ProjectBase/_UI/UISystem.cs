using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

/// <summary>
/// 管理UI
/// 1.加载或移除一个面板
/// 2.管理所有显示的面板
/// 3.提供外部操作面板的方法
/// 
/// Manage UI
/// 1.Show or hide a panel
/// 2.Manage all the shown panels
/// 3.Provide easy access to any panel
/// 
/// !!! IMPORTANT 
///                 The Canvas and the EventSystem should not be put into the scene.
/// !!! IMPORTANT                 
/// </summary>
public class UISystem : System<UISystem>
{
    public Dictionary<EPanels, BasePanel> panelDic = new Dictionary<EPanels, BasePanel>();

    //Reference resolution
    private Vector2 _REFERENCE_RESOULTION_RATIO = new Vector2(1920, 1080);

    private UIPanelPathModel _panelPathModel;
    public Dictionary<EPanels, string> PanelPaths => _panelPathModel?._panelPaths;

    //2D
    //Set by necessary
    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform popUp;
    private Transform system;
    private Transform dialogue;
    //canvas
    public Transform canvas;
    private GameObject eventSystem;

    protected override void OnInit()
    {
        //Loading
        _panelPathModel = MinimalEnvironment.Instance.GetSystem<ResourceSystem>().Load<UIPanelPathModel>("ScriptableObjects/UI/UIPanelPathModel");
        //2DCanvas
        GameObject obj = MinimalEnvironment.Instance.GetSystem<ResourceSystem>().Load<GameObject>("_UI/__ProjectBase/CanvasRoot");
        canvas = obj.transform;
        GameObject.DontDestroyOnLoad(obj);
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        popUp = canvas.Find("PopUp");
        system = canvas.Find("System");
        dialogue = canvas.Find("Dialogue");
        //EventSystem
        if (!GameObject.Find("EventSystem"))
            eventSystem = MinimalEnvironment.Instance.GetSystem<ResourceSystem>().Load<GameObject>("_UI/__ProjectBase/EventSystem");
        else
            eventSystem = GameObject.Find("EventSystem");
        GameObject.DontDestroyOnLoad(eventSystem);
        MinimalEnvironment.Instance.GetSystem<MonoSystem>().AddUpdateListener(Update);
    }

    private void Update()
    {
    }
    
    protected override void OnShutdown()
    {
        _panelPathModel = null;
        if (canvas != null)
        {
            GameObject.Destroy(canvas.gameObject);
        }
        if (eventSystem != null)
        {
            GameObject.Destroy(eventSystem);
        }
    }

    //2DVersion get layer father
    public Transform GetLayerFather(EUILayer layer)
    {
        switch (layer)
        {
            case EUILayer.Bot:
                return this.bot;
            case EUILayer.Mid:
                return this.mid;
            case EUILayer.Top:
                return this.top;
            case EUILayer.Popup:
                return this.popUp;
            case EUILayer.Dialogue:
                return this.dialogue;
            case EUILayer.System:
                return this.system;
        }
        return null;
    }

    /// <summary>
    /// 加载panel，并放在要求的层数
    /// 2DVersion
    /// Load a panel to desired layer.  
    /// !!! Important: panels should be streshed - see Interact&Examine/InteractTextPanel as an example
    /// </summary>
    /// <typeparam panelEnum="T">面板脚本类型  The class of your panel</typeparam>
    /// <param panelEnum="panelName">面板路径 panel panelEnum</param>
    /// <param panelEnum="callBack">创建完成后想做的事 callback function after creating your panel</param>
    /// <param panelEnum="layer">显示在哪一层，默认MID  Which layer to put panel. By default, MID.</param>
    public void ShowPanel<T>(EPanels panelEnum, UnityAction<T> callBack = null, EUILayer layer = EUILayer.Mid) where T : BasePanel
    {
        if (HasPanel(panelEnum))
        {
            panelDic[panelEnum].ShowPanel();
            panelDic[panelEnum].gameObject.SetActive(true);
            if (callBack != null)
                callBack(panelDic[panelEnum] as T);
            return;
        }
        else
        {
            MinimalEnvironment.Instance.GetSystem<ResourceSystem>().LoadAsyn<GameObject>("_UI/" + _panelPathModel._panelPaths[panelEnum], (o) =>
            {
                Transform father = GetLayerFather(layer);

                o.transform.SetParent(father);

                o.transform.localPosition = Vector3.zero;
                o.transform.localScale = Vector3.one;

                (o.transform as RectTransform).offsetMax = Vector2.zero;
                (o.transform as RectTransform).offsetMin = Vector2.zero;

                //得到预设体上的面板脚本
                //Get the script of your panel class.
                GameObject root = RSUtils.FindInChildren(o.transform, "root");
                T panel = root.GetComponent<T>();

                if (panel == null)
                {
                    MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"[UIMgr] the panel {panelEnum} prefab should has a root");
                    panel = o.GetComponent<T>();
                }
                //处理面板创建完成后的逻辑
                //After loaded call functions.
                panel.ShowPanel();
                if (callBack != null)
                    callBack(panel);
                //保存面板
                panelDic.Add(panelEnum, panel);
            });
        }
    }

    /// <summary>
    /// 2D
    /// 删除面板预设体
    /// Delete a panel
    /// </summary>
    /// <param panelEnum="panelEnum"></param>
    public void RemovePanel(EPanels panelEnum)
    {
        if (panelDic.ContainsKey(panelEnum))
        {
            panelDic[panelEnum].HidePanel();
            panelDic[panelEnum].gameObject.SetActive(false);
            GameObject.Destroy(panelDic[panelEnum].gameObject);
            panelDic.Remove(panelEnum);
        }
    }

    /// <summary>
    /// 2D
    /// Call HidePanel Function
    /// Delete a panel
    /// </summary>
    /// <param panelEnum="panelEnum"></param>
    public void HidePanel(EPanels panelEnum)
    {
        if (panelDic.ContainsKey(panelEnum))
        {
            panelDic[panelEnum].HidePanel();
            panelDic[panelEnum].gameObject.SetActive(false);
            return;
        }
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"UIMGR: No such panel: {_panelPathModel._panelPaths[panelEnum]}");
    }

    /// <summary>
    /// 2D
    /// 得到某一个已经存在的面板
    /// Get a exist panel.
    /// </summary>
    public T GetPanel<T>(EPanels panelEnum) where T : BasePanel
    {
        if (HasPanel(panelEnum))
            return panelDic[panelEnum] as T;
        return null;
    }

    public bool HasPanel(EPanels panelEnum)
    {
        if (panelDic.ContainsKey(panelEnum))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 封装event trigger自定义事件监听
    /// Allow you customize event trigger.
    /// </summary>
    /// <param panelEnum="control">控件对象 target</param>
    /// <param panelEnum="type">事件类型 event type</param>
    /// <param panelEnum="callBack">事件响应函数 callback</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);
        //这里可以检测有没有已经有这个事件了，一般不会出现
        //check existence
        trigger.triggers.Add(entry);
    }
}
