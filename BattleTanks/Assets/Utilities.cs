﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    static public Vector3 INVALID_POSITION = new Vector3(-1, -1, -1);
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

    static public Vector3 getRandomPositionOutsideAABB(fRectangle AABB, Vector3 centrePosition, float offSet)
    {
        Assert.IsNotNull(AABB);

        Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f)).normalized;
        Vector3 position = centrePosition;
        int distance = 1;
        while (AABB.contains(position))
        {
            position += direction * distance;
        }
        position += direction * offSet;

        return new Vector3(position.x, 1, position.z);
    }

    static public Vector3 getClosestPositionOutsideAABB(fRectangle AABB, Vector3 unitPosition, Vector3 centrePosition, float offSet)
    {
        Assert.IsNotNull(AABB);

        Vector3 direction = (unitPosition - centrePosition).normalized;
        Vector3 position = centrePosition;
        int distance = 1;
        while(AABB.contains(position))
        {
            position += direction * distance;
        }
        position += direction * offSet;

        return new Vector3(position.x, 1, position.z);
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