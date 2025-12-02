using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "UIPanelPathModel", menuName = "UIConfigs/UIPanelPathModel")]
public class UIPanelPathModel : OdinSerializedStaticDataScriptableObject
{
    public Dictionary<EPanels, string> _panelPaths = new();    
}