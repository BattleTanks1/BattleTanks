using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//http://aigamedev.com/open/tutorial/influence-map-mechanics/
//http://gameschoolgems.blogspot.com/2009/12/influence-maps-i.html
//http://gameschoolgems.blogspot.com/2010/03/influence-maps-ii-practical.html

// https://web.archive.org/web/20190717210940/http://aigamedev.com/open/tutorial/influence-map-mechanics/
//https://gamedev.stackexchange.com/questions/133577/are-there-well-known-algorithms-for-efficient-map-knowledge

//https://www.youtube.com/watch?v=6RGquWxNock

//http://www.gameaipro.com/GameAIPro2/GameAIPro2_Chapter30_Modular_Tactical_Influence_Maps.pdf



//https://www.gamedev.net/articles/programming/artificial-intelligence/the-core-mechanics-of-influence-mapping-r2799/

//  -Choose whatever is a relevant cell size for your setup, let's say 1 unity distance unit.
//- Then you either use Mathf.Floor, Mathf.Round, or Mathf.Ceil(which ever gives best result for you, doesn't matter which you use as long as you are consistent) on both axis from your plane.


/*
  Situation Summary -- 
  Influence maps do a great job of summarizing all the little details in the world and making them easy to understand at a glance. 
  Who's in control of what area? Where are the borders between the territories? How much enemy presence is there in each area?

Historical Statistics -- 
Beyond just storing information about the current situation, influence maps can also remember what happened for a certain period of time. 
Was this area being assaulted? How well did my previous attack go?


Future Predictions -- 
An often ignored aspect of influence maps, they can also help predict the future. Using the map of the terrain, 
you can figure out where an enemy would go and how his influence would extend in the future.
 * */

public class Point
{
    public float value = 0.0f;
    public bool blocked = false;
}

public class InfluenceMap : MonoBehaviour
{
    public GameObject box;
    public Vector2Int mapSize;
    public float spacing;
    public float scalar;

    Point[,] map;
    List<GameObject> m_boxes;

    private static InfluenceMap _instance;
    public static InfluenceMap Instance { get { return _instance; } }

    List<Vector2Int> getAdjacentPositions(Vector2Int position)
    {
        List<Vector2Int> adjacentPositions = new List<Vector2Int>();
        for (int x = position.x - 1; x <= position.x + 1; x += 2)
        {
            if (x >= 0 && x < mapSize.x)
            {
                adjacentPositions.Add(new Vector2Int(x, position.y));
            }
        }

        for (int y = position.y - 1; y <= position.y + 1; y += 2)
        {
            if (y >= 0 && y < mapSize.y)
            {
                adjacentPositions.Add(new Vector2Int(position.x, y));
            }
        }

        return adjacentPositions;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LayerMask layerMask = LayerMask.GetMask("Scenery");
        RaycastHit hit;
        m_boxes = new List<GameObject>();
        map = new Point[(int)mapSize.y, (int)mapSize.x];
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                map[y, x] = new Point();

                // Check for a Wall.
                
                
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(new Vector3(x, 0, y), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
                {
                   // map[y, x].blocked = true;
                }
            }
        }

        IEnumerator coroutine = Propogate();
        StartCoroutine(coroutine);
    }

    void createInfluence(Vector2Int position, int maxDistance, float strength)
    {
        Vector2Int horizontalDistance = new Vector2Int(maxDistance, maxDistance);
        if(position.x - horizontalDistance.x < 0)
        {
            horizontalDistance.x -= Mathf.Abs(position.x - horizontalDistance.x);
        }
        else if (position.x + horizontalDistance.y >= mapSize.x)
        {
            horizontalDistance.y -= mapSize.x - position.x;
        }
        Vector2Int verticalDistance = new Vector2Int(maxDistance, maxDistance);
        if (position.y - verticalDistance.x < 0)
        {
            verticalDistance.x -= Mathf.Abs(position.y - verticalDistance.x);
        }
        else if (position.y + verticalDistance.y >= mapSize.y)
        {
            verticalDistance.x -= mapSize.y - position.y;
        }

        for (int y = position.y - verticalDistance.x; y < position.y + verticalDistance.y; ++y)
        {
            for (int x = position.x - horizontalDistance.x; x < position.x + horizontalDistance.y; ++x)
            {
                float distance = Vector2Int.Distance(new Vector2Int(x, y), position);
                if (!map[y, x].blocked && distance <= maxDistance)
                {
                    map[y, x].value += (strength - strength * (distance / maxDistance));

                    //Create box at location
                    Vector3 i = new Vector3(x * spacing, 0, y * spacing);

                    GameObject clone;
                    clone = Instantiate(box, i, Quaternion.identity);
                    clone.transform.localScale += new Vector3(0, map[y, x].value, 0);
                    m_boxes.Add(clone);
                }
            }
        }
    }

    public float getValueOnPosition(Vector3 position)
    {
        Vector3 tankPosition = position;
        tankPosition.x = Mathf.Abs(Mathf.Round(tankPosition.x));
        tankPosition.z = Mathf.Abs(Mathf.Round(tankPosition.z));

        Vector2Int positionOnGrid = new Vector2Int((int)tankPosition.x, (int)tankPosition.z);
        return map[positionOnGrid.y, positionOnGrid.x].value;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Propogate()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject box in m_boxes)
            {
                Destroy(box);
            }

            //minset - Compilar optimization
            for (int y = 0; y < mapSize.y; ++y)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    map[y, x].value = 0.0f;
                }
            }

            foreach (Tank tank in fGameManager.Instance.getAllTanks())
            {
                Vector3 tankPosition = tank.transform.position;
                tankPosition.x = Mathf.Abs(Mathf.Round(tankPosition.x));
                tankPosition.y = Mathf.Abs(Mathf.Round(tankPosition.z));

                createInfluence(new Vector2Int((int)tankPosition.x, (int)tankPosition.y), tank.m_proximity, tank.m_strength);
            }
        }
    }
}