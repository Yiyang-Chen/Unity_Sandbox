using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIController : UnityController
{
    [Title("Static Config Data")]
    public UIPanelPathModel configAsset;
    [Title("UIPanelPath")]
    [ShowInInspector] 
    public BindableProperty<Dictionary<EPanels, string>> _panelPaths = new();

    
    UISystem _uiSystem;
    
    protected override void OnInit()
    {
        _uiSystem = MinimalEnvironment.Instance.GetSystem<UISystem>();
    }

    protected override void BindStaticData()
    {
        _panelPaths
            .BindTo(
                ()=>
                {
                    return _uiSystem != null ? _uiSystem.PanelPaths : configAsset?._panelPaths;
                },
                (x) => { }
            )
            .BelongsTo(typeof(UIPanelPathModel))
            .WithEditMode(EDataEditMode.AlwaysReadOnly);
    }

    protected override void OnShutdown()
    {
        _uiSystem = null;
    }
}