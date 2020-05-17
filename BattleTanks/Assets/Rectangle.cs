using UnityEngine;

abstract public class Rectangle<T>
{
    public Rectangle()
    { }

    public Rectangle(T left, T right, T bottom, T top)
    {
        m_left = left;
        m_right = right;
        m_top = top;
        m_bottom = bottom;
    }

    public T m_left { get; set; }
    public T m_right { get; set; }
    public T m_top { get; set; }
    public T m_bottom { get; set; }
}

public class iRectangle : Rectangle<int>
{
    public iRectangle(Vector3 position, Vector3 localScale)
        : base()
    {
        reset(position, localScale);
    }

    public iRectangle(int left, int right, int bottom, int top)
    : base(left, right, bottom, top)
    {}

    public iRectangle(Vector2Int position, int distance)
        : base()
    {
        reset(position, distance);
    }

    public void reset(Vector3 position, Vector3 localScale)
    {
        Vector3 scale = new Vector3(localScale.x / 2.0f, 0, localScale.z / 2.0f);

        m_left = (int)Mathf.Min(position.x - scale.x, position.x + scale.x);
        m_right = (int)Mathf.Max(position.x - scale.x, position.x + scale.x);
        m_bottom = (int)Mathf.Min(position.z - scale.z, position.z + scale.z);
        m_top = (int)Mathf.Max(position.z - scale.z, position.z + scale.z);
    }

    public void reset(Vector2Int position, int distance)
    {
        Vector2Int mapSize = Map.Instance.m_mapSize;

        m_left = Mathf.Max(position.x - distance, 0);
        m_right = Mathf.Min(position.x + distance, mapSize.x - 1);
        m_top = Mathf.Min(position.y + distance, mapSize.y - 1);
        m_bottom = Mathf.Max(position.y - distance, 0);
    }
}

public class fRectangle : Rectangle<float>
{
    public fRectangle(Vector3 position, Vector3 localScale)
    {
        reset(position, localScale);
    }

    public fRectangle(float left, float right, float bottom, float top)
        : base(left, right, bottom, top)
    {}

    public bool contains(fRectangle other)
    {
        return m_left <= other.m_right &&
            m_right >= other.m_left &&
            m_top >= other.m_bottom &&
            m_bottom <= other.m_top;
    }

    public bool contains(Vector2Int position)
    {
        return position.x >= m_left &&
            position.x <= m_right &&
            position.y >= m_bottom &&
            position.y <= m_top;
    }

    public bool contains(Vector3 position)
    {
        return position.x >= m_left &&
            position.x <= m_right &&
            position.z >= m_bottom &&
            position.z <= m_top;
    }

    public void reset(Vector3 position, Vector3 localScale)
    {
        Vector3 scale = new Vector3(localScale.x / 2.0f, 0, localScale.z / 2.0f);

        m_left = Mathf.Min(position.x - scale.x, position.x + scale.x);
        m_right = Mathf.Max(position.x - scale.x, position.x + scale.x);
        m_bottom = Mathf.Min(position.z - scale.z, position.z + scale.z);
        m_top = Mathf.Max(position.z - scale.z, position.z + scale.z);
    }
}