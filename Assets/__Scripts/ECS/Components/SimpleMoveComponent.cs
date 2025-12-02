using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class SimpleMoveComponent : MoveComponent
{
    [ShowInInspector]
    [ReadOnly]
    protected Vector3 targetPosition;

    [ShowInInspector]
    [ReadOnly]
    protected bool isMoving;
    
    [ShowInInspector]
    protected float moveSpeed;

    [ShowInInspector]
    [ReadOnly]
    protected Vector3 moveDirection; // 新增方向移动向量

    public override void BeginMove()
    {
        isMoving = true;
        entity.eventComponent.EventTrigger(EEntityEvent.EntityBeginMove);
    }

    public override void StopMove()
    {
        isMoving = false;
        entity.eventComponent.EventTrigger(EEntityEvent.EntityStopMove);
    }

    public override void MoveTo(Vector3 worldPosition)
    {
        targetPosition = worldPosition;
        moveDirection = Vector3.zero; // 清除方向移动
        BeginMove();
    }

    // 新增方向移动方法
    public virtual void MoveByDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
        targetPosition = Vector3.zero; // 清除目标点移动
        BeginMove();
    }

    protected virtual void Update()
    {
        if (!isMoving) return;
        
        // 优先处理方向移动
        if (moveDirection != Vector3.zero)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            return;
        }
        
        // 处理目标点移动
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        
        // 如果距离很小，直接到达目标位置
        if (distance < 0.1f)
        {
            transform.position = targetPosition;
            StopMove();  // 到达目标时触发停止移动事件
            return;
        }
        
        // 平滑移动
        transform.position += direction * moveSpeed * Time.deltaTime;
        
    }
}