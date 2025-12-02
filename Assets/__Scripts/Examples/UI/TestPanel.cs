using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestPanel : BasePanel
{
    // Start is called before the first frame update
    void Start()
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug(GetControl<Text>("Text").Count);
        // 自定义事件监听
        UISystem.AddCustomEventListener(GetControl<Button>("ButtonStart")[0], EventTriggerType.PointerEnter,
            (data) => { MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("进入"); });
    }

    public void InitialFinished()
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("initial finished");
    }

    public void ClickStart()
    {
        MinimalEnvironment.Instance.GetSystem<UISystem>().GetPanel<TestPanel>(EPanels.ExamplePanel).ClickAgain();
    }

    public void ClickClose()
    {
        MinimalEnvironment.Instance.GetSystem<UISystem>().HidePanel(EPanels.ExamplePanel);
    }

    private void ClickAgain()
    {
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("DoSomething on the panel");
    }

    public override void ShowPanel()
    {
        base.ShowPanel();
        MinimalEnvironment.Instance.GetSystem<LogSystem>().Debug("showed panel, initialze");
    }

    // 按钮点击事件处理
    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "ButtonStart":
                ClickStart();
                break;
            case "ButtonEnd":
                ClickClose();
                break;
        }
    }
}