using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : BaseComponent
{
    public virtual void BeginMove()
    {
        
    }

    public virtual void MoveTo(Vector3 worldPosition)
    {
        transform.position = worldPosition;
    }

    public virtual void StopMove()
    {
        
    }
    
    public virtual void RotateTo(Vector3 worldPosition)
    {
        transform.LookAt(worldPosition);
    }
}
