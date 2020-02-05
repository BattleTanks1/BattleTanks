using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle
{
    public Rectangle(int left, int right, int top, int bottom)
    {
        m_left = left;
        m_right = right;
        m_top = top;
        m_bottom = bottom;
    }

    public Rectangle(Vector2Int position, int distance)
    {
        reset(position, distance);
    }

    public void reset(Vector2Int position, int distance)
    {
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;

        m_left = Mathf.Max(position.x - distance, 0);
        m_right = Mathf.Min(position.x + distance, mapSize.x);
        if (m_right == mapSize.x)
        {
            --m_right;
        }
        m_top = Mathf.Max(position.y - distance, 0);
        m_bottom = Mathf.Min(position.y + distance, mapSize.y);
        if (m_bottom == mapSize.y)
        {
            --m_bottom;
        }
    }

    public int m_left { get; private set; }
    public int m_right { get; private set; }
    public int m_top { get; private set; }
    public int m_bottom { get; private set; }
}

public enum eDirection2D
{
    Left = 0,
    Right,
    Top,
    Bottom,
    Total
}

public static class Utilities
{
    public const int INVALID_ID = -1;
    static public Vector2Int[] getDirections2D()
    {
        Vector2Int[] directions2D = new Vector2Int[(int)eDirection2D.Total]
        {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        return directions2D;
    }

    static public Vector2Int[] getDiagonalDirections2D()
    {
        Vector2Int[] diagonaldirections2D = new Vector2Int[(int)eDirection2D.Total]
        {
            new Vector2Int(-1, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1)
        };

        return diagonaldirections2D;
    }

    static public Vector2Int convertToGridPosition(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
    }

    static public Vector3 convertToWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x, 0, position.y);
    }
}