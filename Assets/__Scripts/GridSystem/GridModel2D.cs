using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridModel2D<TGridObject> : IGridModel where TGridObject : IGridObject
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray;
    
    public GridModel2D(int width, int height, float cellSize, 
        Func<GridModel2D<TGridObject>, GridPosition2D, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        
        gridObjectArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition2D gridPosition = new GridPosition2D(x, z);
                gridObjectArray[x, z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(IGridPosition gridPosition)
    {
        return new Vector3(gridPosition.X, 0, gridPosition.Z) * cellSize;
    }

    public GridPosition2D GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition2D(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
            );
    }

    IGridPosition IGridModel.GetGridPosition(Vector3 worldPosition)
    {
        return GetGridPosition(worldPosition);
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition2D gridPosition = new GridPosition2D(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition2D gridPosition)
    {
        return gridObjectArray[gridPosition.X, gridPosition.Z];
    }

    IGridObject IGridModel.GetGridObject(IGridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.X, gridPosition.Z];
    }

    public bool IsValidGridPosition(GridPosition2D gridPosition)
    {
        return gridPosition.X >= 0 &&
               gridPosition.Z >= 0 &&
               gridPosition.X < width &&
               gridPosition.Z < height;
    }

    bool IGridModel.IsValidGridPosition(IGridPosition gridPosition)
    {
        return gridPosition.X >= 0 &&
               gridPosition.Z >= 0 &&
               gridPosition.X < width &&
               gridPosition.Z < height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}

