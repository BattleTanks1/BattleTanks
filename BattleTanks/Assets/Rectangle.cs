using UnityEngine;

public class iRectangle : Rectangle<int>
{
    public iRectangle(int left, int right, int bottom, int top)
    : base(left, right, bottom, top)
    {}

    public iRectangle(Vector2Int position, int distance)
        : base()
    {
        reset(position, distance);
    }

    public void reset(Vector2Int position, int distance)
    {
        Vector2Int mapSize = GameManager.Instance.m_mapSize;

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
}

public class fRectangle : Rectangle<float>
{
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
}

abstract public class Rectangle<T>
{
    public Rectangle()
    {}

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