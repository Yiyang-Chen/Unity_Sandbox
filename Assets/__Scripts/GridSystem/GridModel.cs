using UnityEngine;

public interface IGridModel : IRuntimeModel
{
    Vector3 GetWorldPosition(IGridPosition gridPosition);
    bool IsValidGridPosition(IGridPosition gridPosition);
    void CreateDebugObjects(Transform debugPrefab);
    IGridPosition GetGridPosition(Vector3 worldPosition);
    IGridObject GetGridObject(IGridPosition gridPosition);
}