using UnityEngine;

public class Rectangle
{
    public Rectangle()
    {

    }

    public Rectangle(int left, int right, int top, int bottom)
    {
        m_left = left;
        m_right = right;
        m_top = top;
        m_bottom = bottom;
    }

    bool contains(Rectangle other)
    {


        return false;
    }

    //-- Working map integration - forgot

    public Rectangle(Vector2Int position, int distance)
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

    public int m_left { get; set; }
    public int m_right { get; set; }
    public int m_top { get; set; }
    public int m_bottom { get; set; }
}

//public class Rectangle : MonoBehaviour
//{
//    public float left { get; private set; }
//    public float right { get; private set; }
//    public float top { get; private set; }
//    public float bottom { get; private set; }


//    public bool intersects(Rectangle other)
//    {
//        return (left <= other.right &&
//                right >= other.left &&
//                top >= other.bottom &&
//                bottom <= other.top);
//    }

//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}
