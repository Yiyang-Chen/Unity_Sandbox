using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour, INodeParent
{
    protected INodeParent parent;
    // TODO: 所有Node的基础设置
    // TODO: 代码生成相关代码

    public virtual void Init(INodeParent parent)
    {
        this.parent = parent;
    }
    
    public virtual T GetPanel<T>() where T : BasePanel
    {
        return parent.GetPanel<T>();
    }
}