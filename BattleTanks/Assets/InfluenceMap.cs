﻿using System.Collections;
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

public class FrontierNode
{
    public FrontierNode(Vector2Int p, int d)
    {
        position = p;
        depth = d;
    }

    public Vector2Int position;
    public int depth = 0;
}

public class Point
{
    public float value = 0.0f;
    public bool visited = false;
}

public class WorkingMap
{ 
    public WorkingMap()
    {
        m_workingMap = new Point[40, 40];
        for(int y = 0; y < 40; ++y)
        {
            for(int x = 0; x < 40; ++x)
            {
                m_workingMap[y, x] = new Point();
            }
        }
    }

    public void reset(Vector2Int position, int distance)
    {
        m_searchableArea.reset(position, distance);
    }

    private Rectangle m_searchableArea;
    [SerializeField]
    public Point[,] m_workingMap { get; private set; }
}

public class Map
{
    public Point[,] m_map { get; private set; }

    public eFactionName m_ownerName { get; private set; }

    public Map(eFactionName ownerName)
    {
        m_ownerName = ownerName;
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        m_map = new Point[mapSize.y, mapSize.x];
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                m_map[y, x] = new Point();
            }
        }
    }

    public void createInfluence(Vector2Int position, float strength, int maxDistance)
    {
        Rectangle searchableRect = new Rectangle(position, maxDistance);
        for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                float distance = Vector2Int.Distance(new Vector2Int(x, y), position);
                if (distance <= maxDistance)
                {
                    m_map[y, x].value += strength - (strength * (distance / maxDistance));
                }
            }
        }
    }

    public void createThreat(Vector2Int position, float strength, int maxDistance, float fallOfStrength, int fallOfDistance)
    {
        int totalDistance = maxDistance + fallOfDistance;
        Rectangle searchableRect = new Rectangle(position, totalDistance);
        for (int y = searchableRect.m_top; y <= searchableRect.m_bottom; ++y)
        {
            for (int x = searchableRect.m_left; x <= searchableRect.m_right; ++x)
            {
                float distance = Vector2Int.Distance(new Vector2Int(x, y), position);
                if (distance <= maxDistance)
                {
                    m_map[y, x].value += strength;
                    //m_map[y, x].value = strength * (1 - ((distance / maxDistance) * (distance / maxDistance)));
                }
                else if(distance <= maxDistance + fallOfDistance)
                {
                    //m_map[y, x].value += strength - (strength * (distance / maxDistance));
                    m_map[y, x].value += fallOfStrength - (fallOfStrength * (distance / totalDistance));
                }
            }
        }
    }

    public void reset()
    {
        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                m_map[y, x].value = 0.0f;
            }
        }
    }

    public Point getPoint(Vector3 position)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);

        return m_map[positionOnGrid.y, positionOnGrid.x];
    }

    public Point getPoint(Vector2Int position)
    {
        return m_map[position.y, position.x];
    }
}

public class InfluenceMap : MonoBehaviour
{
    List<GameObject> m_boxes;
    public GameObject m_redBox;
    public GameObject m_blueBox;
    //Proximity Map
    [SerializeField]
    private Map[] m_proximityMaps = new Map[(int)eFactionName.Total];
    [SerializeField]
    private Map[] m_threatMaps = new Map[(int)eFactionName.Total];
   
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
    private void Start()
    {
        m_boxes = new List<GameObject>();

        m_proximityMaps = new Map[(int)eFactionName.Total];
        m_proximityMaps[(int)eFactionName.Red] = new Map(eFactionName.Red);
        m_proximityMaps[(int)eFactionName.Blue] = new Map(eFactionName.Blue);

        m_threatMaps = new Map[(int)eFactionName.Total];
        m_threatMaps[(int)eFactionName.Red] = new Map(eFactionName.Red);
        m_threatMaps[(int)eFactionName.Blue] = new Map(eFactionName.Blue);

        IEnumerator coroutine = resetBaseMaps();
        StartCoroutine(coroutine);
    }

    private void spawnCube(int x, int y, Map map)
    {
        if(map.m_map[y, x].value > 0)
        {
            if (map.m_ownerName == eFactionName.Red)
            {
                GameObject clone;
                clone = Instantiate(m_redBox, new Vector3(x, 0, y), Quaternion.identity);
                clone.transform.localScale += new Vector3(0, Mathf.Abs(map.m_map[y, x].value), 0);
                m_boxes.Add(clone);
            }
            else if (map.m_ownerName == eFactionName.Blue)
            {
                GameObject clone;
                clone = Instantiate(m_blueBox, new Vector3(x, 0, y), Quaternion.identity);
                clone.transform.localScale += new Vector3(0, Mathf.Abs(map.m_map[y, x].value), 0);
                m_boxes.Add(clone);

            }
        }
    }

    public bool isPositionInThreat(Tank tank)
    {
        float value = 0.0f;
        foreach(Map threatMap in m_threatMaps)
        {
            if(threatMap.m_ownerName != tank.m_factionName)
            {
                value += threatMap.getPoint(tank.transform.position).value;
            }
        }

        return tank.m_scaredValue >= value;
    }

    public bool isPositionInThreat(Vector2Int position, eFactionName factionName)
    {
        float value = 0.0f;
        foreach (Map threatMap in m_threatMaps)
        {
            if (threatMap.m_ownerName != factionName)
            {
                value += threatMap.getPoint(position).value;
            }
        }

        return value >= 0;
    }


    private void inverseWorkingMap(Vector2Int position)
    {
      
    }

    private IEnumerator resetBaseMaps()
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
            
            foreach(Faction faction in fGameManager.Instance.m_factions)
            {
                foreach(Tank tank in faction.m_tanks)
                {
                    Vector2Int tankPositionOnGrid = Utilities.convertToGridPosition(tank.transform.position);
                    m_proximityMaps[(int)tank.m_factionName].createInfluence(tankPositionOnGrid, tank.m_proximityStrength, tank.m_proximityDistance);
                    m_threatMaps[(int)tank.m_factionName].createThreat(tankPositionOnGrid, tank.m_threatStrength, tank.m_threatDistance, 
                        tank.m_threatFallOffStrength, tank.m_threatFallOffDistance);
                }
            }

            Vector2Int mapSize = fGameManager.Instance.m_mapSize;
            for (int y = 0; y < mapSize.y; ++y)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    spawnCube(x, y, m_threatMaps[(int)eFactionName.Red]);
                    //spawnCube(x, y, m_proximityMaps[(int)eFactionName.Blue]);
                }
            }
        }
    }
}