using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition2D : IEquatable<GridPosition2D>, IGridPosition
{
    public int X { get => x; set => x = value; }
    public int Y { get => 0; set {}}
    public int Z { get => z; set => z = value; }
        
    private int x;
    private int z;

    public GridPosition2D(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override string ToString()
    {
        return $"x: {x}, z: {z}";
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition2D position &&
               x == position.x &&
               z == position.z;
    }

    public bool Equals(GridPosition2D other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public static bool operator ==(GridPosition2D a, GridPosition2D b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(GridPosition2D a, GridPosition2D b)
    {
        return !(a == b);
    }

    public static GridPosition2D operator +(GridPosition2D a, GridPosition2D b)
    {
        return new GridPosition2D(a.x + b.x, a.z + b.z);
    }

    public static GridPosition2D operator -(GridPosition2D a, GridPosition2D b)
    {
        return new GridPosition2D(a.x - b.x, a.z - b.z);
    }
}