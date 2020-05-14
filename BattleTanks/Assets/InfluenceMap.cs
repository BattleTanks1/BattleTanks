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

//GameAIPro Notes
//They updated their tactical influence map once per second

//Special Functions
//Normalize function - take a 1.4 to 0.7 and turn into 1.0 - 0.5
//Inverse Function - Cells start from 1.0 and below 

//Uses:
//1.
//might be how far we could attack in 1 s(our maximum threat range + our movement speed).

//Often, it is good to prioritize information that is closer to the agent so that it doesn’t
//make decisions that cause it to, perhaps, run past one threat to get to another.By multiplying the 
//    resulting working map by our personal interest template, we adjust the data
//so that closer cells are left relatively untouched, but cells on the periphery are reduced
//artificially—ultimately dropping to zero

public class PointOnInfluenceMap
{
    public float value = 0.0f;
    public bool visited = false;
}

public class FactionInfluenceMap
{
    public PointOnInfluenceMap[,] m_map { get; private set; }

    public eFactionName m_ownerName { get; private set; }

    public FactionInfluenceMap(eFactionName ownerName)
    {
        m_ownerName = ownerName;
        Vector2Int mapSize = Map.Instance.m_mapSize;
        m_map = new PointOnInfluenceMap[mapSize.y, mapSize.x];
        for (int x = 0; x < mapSize.x; ++x)
        {
            for (int y = 0; y < mapSize.y; ++y)
            {
                m_map[x, y] = new PointOnInfluenceMap();
            }
        }
    }

    public void createProximity(Vector2Int position, float strength, int maxDistance)
    {
        iRectangle searchableRect = new iRectangle(position, maxDistance);
        for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
        {
            for (int y = searchableRect.m_bottom; y <= searchableRect.m_top; ++y)
            {
                if(Map.Instance.isPositionScenery(x, y))
                {
                    continue;
                }

                float sqrDistance = (new Vector2Int(x, y) - position).sqrMagnitude;
                if (sqrDistance <= maxDistance * maxDistance)
                {
                    m_map[x, y].value += strength - (strength * (sqrDistance / (maxDistance * maxDistance)));
                }
            }
        }
    }

    public void createThreat(Vector2Int position, float strength, int maxDistance, float fallOfStrength, int fallOfDistance)
    {
        int totalDistance = maxDistance + fallOfDistance;
        iRectangle searchableRect = new iRectangle(position, totalDistance);
        for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
        {
            for (int y = searchableRect.m_bottom; y <= searchableRect.m_top; ++y)
            {
                if (Map.Instance.isPositionScenery(x, y))
                {
                    continue;
                }

                float sqrDistance = (new Vector2Int(x, y) - position).sqrMagnitude;
                if (sqrDistance <= maxDistance * maxDistance)
                {
                    m_map[x, y].value += strength;
                    //m_map[x, y].value = strength * (1 - ((distance / maxDistance) * (distance / maxDistance)));
                }
                else if(sqrDistance <= maxDistance * maxDistance + fallOfDistance * fallOfDistance)
                {
                    //m_map[x, y].value += strength - (strength * (distance / maxDistance));
                    m_map[x, y].value += fallOfStrength - (fallOfStrength * (sqrDistance / (totalDistance * totalDistance)));
                }
            }
        }
    }

    public void reset()
    {
        Vector2Int mapSize = Map.Instance.m_mapSize;
        for (int x = 0; x < mapSize.x; ++x)
        {
            for (int y = 0; y < mapSize.y; ++y)
            {
                m_map[x, y].value = 0.0f;
            }
        }
    }

    public PointOnInfluenceMap getPoint(Vector3 position)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);

        return m_map[positionOnGrid.x, positionOnGrid.y];
    }

    public PointOnInfluenceMap getPoint(Vector2Int position)
    {
        return m_map[position.x, position.y];
    }
}

public class InfluenceMap : MonoBehaviour
{
    //Visual aides
    List<GameObject> m_boxes;
    public GameObject m_redBox;
    public GameObject m_blueBox;


    //Proximity Map
    [SerializeField]
    private bool m_renderProximityMap = false;
    [SerializeField]
    private bool m_renderThreatMap = false;
    [SerializeField]
    private FactionInfluenceMap[] m_proximityMaps = new FactionInfluenceMap[(int)eFactionName.Total];
    [SerializeField]
    private FactionInfluenceMap[] m_threatMaps = new FactionInfluenceMap[(int)eFactionName.Total];
   
    private static InfluenceMap _instance;
    public static InfluenceMap Instance { get { return _instance; } }

    private void Awake()
    {
        //Prevents more than one instance existing in the scene
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
    private void Start()
    {
        m_boxes = new List<GameObject>();

        m_proximityMaps = new FactionInfluenceMap[(int)eFactionName.Total];
        m_proximityMaps[(int)eFactionName.Red] = new FactionInfluenceMap(eFactionName.Red);
        m_proximityMaps[(int)eFactionName.Blue] = new FactionInfluenceMap(eFactionName.Blue);

        m_threatMaps = new FactionInfluenceMap[(int)eFactionName.Total];
        m_threatMaps[(int)eFactionName.Red] = new FactionInfluenceMap(eFactionName.Red);
        m_threatMaps[(int)eFactionName.Blue] = new FactionInfluenceMap(eFactionName.Blue);

        IEnumerator coroutine = updateBaseMaps();
        StartCoroutine(coroutine);
    }

    private void spawnCube(int x, int y, FactionInfluenceMap map)
    {
        if(map.m_map[x, y].value > 0)
        {
            if (map.m_ownerName == eFactionName.Red)
            {
                GameObject clone;
                clone = Instantiate(m_redBox, new Vector3(x, 0, y), Quaternion.identity);
                clone.transform.localScale += new Vector3(0, Mathf.Abs(map.m_map[x, y].value), 0);
                m_boxes.Add(clone);
            }
            else if (map.m_ownerName == eFactionName.Blue)
            {
                GameObject clone;
                clone = Instantiate(m_blueBox, new Vector3(x, 0, y), Quaternion.identity);
                clone.transform.localScale += new Vector3(0, Mathf.Abs(map.m_map[x, y].value), 0);
                m_boxes.Add(clone);
            }
        }
    }

    public bool isPositionInThreat(Unit unit)
    {
        float threatValue = -1.0f;
        foreach(FactionInfluenceMap threatMap in m_threatMaps)
        {
            if(threatMap.m_ownerName != unit.getFactionName())
            {
                threatValue += threatMap.getPoint(unit.transform.position).value;
            }
        }

        return threatValue >= unit.getScaredValue();
    }

    public PointOnInfluenceMap getPointOnProximityMap(Vector2Int position, eFactionName factionName)
    {
        return m_proximityMaps[(int)factionName].getPoint(position);
    }

    private IEnumerator updateBaseMaps()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            foreach (GameObject box in m_boxes)
            {
                Destroy(box);
            }
    
            for(int i = 0; i < (int)(eFactionName.Total); ++i)
            {
                m_threatMaps[i].reset();
                m_proximityMaps[i].reset();
            }

            GameManager.Instance.createInfluence(m_proximityMaps, m_threatMaps);

            if(m_renderThreatMap)
            {
                Vector2Int mapSize = Map.Instance.m_mapSize;
                for (int x = 0; x < mapSize.x; ++x)
                {
                    for (int y = 0; y < mapSize.y; ++y)
                    {  
                        spawnCube(x, y, m_threatMaps[(int)eFactionName.Red]);
                        spawnCube(x, y, m_threatMaps[(int)eFactionName.Blue]);
                    }
                }
            }
            if(m_renderProximityMap)
            {
                Vector2Int mapSize = Map.Instance.m_mapSize;
                for (int x = 0; x < mapSize.x; ++x)
                {
                    for (int y = 0; y < mapSize.y; ++y)
                    {
                        spawnCube(x, y, m_proximityMaps[(int)eFactionName.Red]);
                        spawnCube(x, y, m_proximityMaps[(int)eFactionName.Blue]);
                    }
                }
            }
            Pathfinder.Instance.updateDangerMap((int)eFactionName.Red, m_threatMaps[(int)eFactionName.Red].m_map);
            Pathfinder.Instance.updateDangerMap((int)eFactionName.Blue, m_threatMaps[(int)eFactionName.Blue].m_map);
        }
    }
}