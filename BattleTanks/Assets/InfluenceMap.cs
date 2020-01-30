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

//https://www.reddit.com/r/gameai/comments/b0t0q5/modular_influence_map_system_demonstration_videos/

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

//Proximity map
//Threat map

//Red Positive
//Blue Negative

public class SearchRect
{
    public SearchRect(Vector2Int position, int distance)
    {
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        left = distance;
        right = distance;
        top = distance;
        bottom = distance;

        if (position.x - distance < 0)
        {
            left -= Mathf.Abs(position.x - distance);
        }
        else if(position.x + distance > mapSize.x)
        {
            right -= (position.x + distance) - mapSize.x;
        }
        if (position.y - distance < 0)
        {
            top -= Mathf.Abs(position.y - distance);
        }
        else if(position.y + distance > mapSize.y)
        {
            bottom -= (position.y + distance) - mapSize.y; 
        }
    }

    public int left { get; private set; }
    public int right { get; private set; }
    public int top { get; private set; }
    public int bottom { get; private set; }
}

public class Point
{
    public float value = 0.0f;
}

public class InfluenceMap : MonoBehaviour
{
    List<GameObject> m_boxes;
    public GameObject m_box;
    //Proximity Map
    private Point[,] proximityMap;
    //Threat Map
    private Point[,] m_threatMap;

    private static InfluenceMap _instance;
    public static InfluenceMap Instance { get { return _instance; } }

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
        m_boxes = new List<GameObject>();
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        proximityMap = new Point[mapSize.y, mapSize.x];
        m_threatMap = new Point[mapSize.y, mapSize.x];
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                proximityMap[y, x] = new Point();
                m_threatMap[y, x] = new Point();
            }
        }

        IEnumerator coroutine = Propogate();
        StartCoroutine(coroutine);
    }

    public bool isPositionInThreat(AITank tank)
    {
        if (tank.m_faction == Faction.AIRed)
        {
            if (getPointOnThreatMap(tank.transform.position).value <= -tank.m_scaredValue)
            {
                return true;
            }
        }
        else if (tank.m_faction == Faction.AIBlue)
        {
            if (getPointOnThreatMap(tank.transform.position).value >= tank.m_scaredValue)
            {
                return true;
            }
        }

        return false;
    }

    private void createInfluence(Vector2Int position, int maxDistance, float strength)
    {
        SearchRect searchableRect = new SearchRect(position, maxDistance);
        for (int y = position.y - searchableRect.top; y < position.y + searchableRect.bottom; ++y)
        {
            for (int x = position.x - searchableRect.left; x < position.x + searchableRect.right; ++x)
            {
                float distance = Vector2Int.Distance(new Vector2Int(x, y), position);
                if (distance <= maxDistance)
                {
                    proximityMap[y, x].value += strength - (strength * (distance / maxDistance));

                    ////Create box at location
                    GameObject clone;
                    clone = Instantiate(m_box, new Vector3(x, 0, y), Quaternion.identity);
                    clone.transform.localScale += new Vector3(0, proximityMap[y, x].value, 0);
                    m_boxes.Add(clone);
                }
            }
        }
    }

    private void createThreat(Vector2Int position, int threatDistance, float strength)
    {
        SearchRect searchableRect = new SearchRect(position, threatDistance);
        for (int y = position.y - searchableRect.top; y < position.y + searchableRect.bottom; ++y)
        {
            for (int x = position.x - searchableRect.left; x < position.x + searchableRect.right; ++x)
            {
                float distance = Vector2Int.Distance(new Vector2Int(x, y), position);
                if (distance <= threatDistance)
                {
                    m_threatMap[y, x].value = strength * (1 - ((distance / threatDistance) * (distance / threatDistance)));

                    //Create box at location
                    GameObject clone;
                    clone = Instantiate(m_box, new Vector3(x, 0, y), Quaternion.identity);
                    clone.transform.localScale += new Vector3(0, m_threatMap[y, x].value, 0);
                    m_boxes.Add(clone);
                }
            }
        }
    }

    public Point getPointOnProximityMap(Vector3 position)
    {
        Vector2Int positionOnGrid = new Vector2Int((int)Mathf.Abs(Mathf.Round(position.x)), 
            (int)Mathf.Abs(Mathf.Round(position.z)));

        return proximityMap[positionOnGrid.y, positionOnGrid.x];
    }

    public Point getPointOnThreatMap(Vector3 position)
    {
        Vector2Int positionOnGrid = new Vector2Int((int)Mathf.Abs(Mathf.Round(position.x)),
            (int)Mathf.Abs(Mathf.Round(position.z)));

        return m_threatMap[positionOnGrid.y, positionOnGrid.x];
    }

    public Point getPointOnThreatMap(Vector2Int position)
    {
        return m_threatMap[position.y, position.x];
    }

    private IEnumerator Propogate()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (GameObject box in m_boxes)
            {
                Destroy(box);
            }

            Vector2Int mapSize = fGameManager.Instance.m_mapSize;
            //minset - Compilar optimization
            for (int y = 0; y < mapSize.y; ++y)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    proximityMap[y, x].value = 0.0f;
                    m_threatMap[y, x].value = 0.0f;
                }
            }

            foreach (Tank tank in fGameManager.Instance.getAllAITanks())
            {
                Vector3 tankPosition = tank.transform.position;
                tankPosition.x = Mathf.Abs(Mathf.Round(tankPosition.x));
                tankPosition.y = Mathf.Abs(Mathf.Round(tankPosition.z));

                if(tank.m_faction == Faction.AIBlue)
                {
                    tank.m_strength = -tank.m_strength;
                    tank.m_threat = -tank.m_threat;
                }
                
                //createInfluence(new Vector2Int((int)tankPosition.x, (int)tankPosition.y), tank.m_proximity, tank.m_strength);
                createThreat(new Vector2Int((int)tankPosition.x, (int)tankPosition.y), tank.m_proximity, tank.m_threat);
            }
        }
    }
}