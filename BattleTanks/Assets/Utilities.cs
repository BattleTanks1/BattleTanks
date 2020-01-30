using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    static public Vector2Int[] get2DDirections()
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
    static public Vector2Int[] get2DDirections(int scalar)
    {
        Vector2Int[] directions2D = new Vector2Int[(int)eDirection2D.Total]
        {
            new Vector2Int(-scalar, 0),
            new Vector2Int(scalar, 0),
            new Vector2Int(0, scalar),
            new Vector2Int(0, -scalar)
        };

        return directions2D;
    }

}
