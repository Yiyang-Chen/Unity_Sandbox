using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INodeParent
{
    public T GetPanel<T>() where T : BasePanel;
}

public static class UINodeExtension
{
    public static void AddNode<T>(this INodeParent parent, Transform nodePrefab, Transform parentTransform, Action<T> initAction = null) where T : BaseNode
    {
        Transform prefabIns = GameObject.Instantiate(nodePrefab, parentTransform);
        if (prefabIns.TryGetComponent<BaseNode>(out BaseNode node))
        {
            node.Init(parent);
            
            T typedNode = node as T;
            if (typedNode == null)
            {
                MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"BasePanel: AddNode: prefab {nodePrefab.name} cannot be convert to type {typeof(T)}"); 
            }
            else
            {
                initAction?.Invoke(typedNode);
            }
        }
        else
        {
            MinimalEnvironment.Instance.GetSystem<LogSystem>().Error($"BasePanel: AddNode: prefab {nodePrefab.name} does not have BaseNode component");    
        }
    }
}