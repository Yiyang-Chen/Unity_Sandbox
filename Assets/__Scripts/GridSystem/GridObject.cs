using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridObject
{
    public IGridPosition GetGridPosition();
    public void SetGridPosition(IGridPosition gridPosition);
}
