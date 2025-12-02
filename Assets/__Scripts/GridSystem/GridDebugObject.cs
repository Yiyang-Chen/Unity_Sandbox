using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] 
    private TextMeshPro text;

    private IGridObject gridObject;
    
    public virtual void SetGridObject(IGridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    protected virtual void Update()
    {
        text.text = gridObject.ToString();
    }
}
