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

public class Point
{
    public float value = 0.0f;
}

public class Map
{
    public Map(eFactionName owner)
    {
        ownerName = owner;

        Vector2Int mapSize = fGameManager.Instance.m_mapSize;
        map = new Point[mapSize.y, mapSize.x];
        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                map[y, x] = new Point();
            }
        }
    }
    
    public void createInfluence(Vector2Int position, float strength, int maxDistance)
    {
        SearchRect searchableRect = new SearchRect(position, maxDistance);
        for (int y = searchableRect.top; y <= searchableRect.bottom; ++y)
        {
            for (int x = searchableRect.left; x <= searchableRect.right; ++x)
            {
                float distance = Vector2Int.Distance(new Vector2Int(x, y), position);
                if (distance <= maxDistance)
                {
                    map[y, x].value += strength - (strength * (distance / maxDistance));
                }
            }
        }
    }

    public void createThreat(Vector2Int position, float strength, int maxDistance)
    {
        SearchRect searchableRect = new SearchRect(position, maxDistance);
        for (int y = searchableRect.top; y <= searchableRect.bottom; ++y)
        {
            for (int x = searchableRect.left; x <= searchableRect.right; ++x)
            {
                float distance = Vector2Int.Distance(new Vector2Int(x, y), position);
                if (distance <= maxDistance)
                {
                    map[y, x].value = strength * (1 - ((distance / maxDistance) * (distance / maxDistance)));
                }
            }
        }
    }

    public Point getPoint(Vector3 position)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);

        return map[positionOnGrid.y, positionOnGrid.x];
    }

    public Point[,] map { get; private set; }
    public eFactionName ownerName { get; private set; }
}

//GameAIPro Notes
//They updated their tactical influence map once per second

public class InfluenceMap : MonoBehaviour
{
    List<GameObject> m_boxes;
    public GameObject m_redBox;
    public GameObject m_blueBox;
    //Proximity Map
    private Map[] m_proximityMaps;
    private Map[] m_threatMaps;
    private Point[,] m_workingMap;

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

        m_proximityMaps = new Map[(int)eFactionName.Total];
        m_proximityMaps[(int)eFactionName.Red] = new Map(eFactionName.Red);
        m_proximityMaps[(int)eFactionName.Blue] = new Map(eFactionName.Blue);

        m_threatMaps = new Map[(int)eFactionName.Total];
        m_threatMaps[(int)eFactionName.Red] = new Map(eFactionName.Red);
        m_threatMaps[(int)eFactionName.Blue] = new Map(eFactionName.Blue);

        m_workingMap = new Point[20, 20];

        IEnumerator coroutine = reset();
        StartCoroutine(coroutine);
    }

    public bool isPositionInThreat(Tank tank)
    {
        float value = 0.0f;
        foreach(Map threatMap in m_threatMaps)
        {
            if(threatMap.ownerName != tank.m_factionName)
            {
                value += threatMap.getPoint(tank.transform.position).value;
            }
        }

        return tank.m_scaredValue >= value;
    }

    private void spawnCube(int x, int y, float value)
    {
        //Red Faction
        if(value > 0)
        {
            GameObject clone;
            clone = Instantiate(m_redBox, new Vector3(x, 0, y), Quaternion.identity);
            clone.transform.localScale += new Vector3(0, Mathf.Abs(value), 0);
            m_boxes.Add(clone);
        }
        //Blue Faction
        else if(value < 0)
        {
            GameObject clone;
            clone = Instantiate(m_blueBox, new Vector3(x, 0, y), Quaternion.identity);
            clone.transform.localScale += new Vector3(0, Mathf.Abs(value), 0);
            m_boxes.Add(clone);
        }
    }

    public Point getPointOnProximityMap(Vector3 position)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);

        return proximityMap[positionOnGrid.y, positionOnGrid.x];
    }

    public Point getPointOnThreatMap(Vector3 position)
    {
        Vector2Int positionOnGrid = Utilities.convertToGridPosition(position);

        return m_threatMap[positionOnGrid.y, positionOnGrid.x];
    }

    public Point getPointOnThreatMap(Vector2Int position)
    {
        return m_threatMap[position.y, position.x];
    }

    private IEnumerator reset()
    {
        while(true)
        {
            yield return new WaitForSeconds(1.0f);
            foreach (GameObject box in m_boxes)
            {
                Destroy(box);
            }

            Vector2Int mapSize = fGameManager.Instance.m_mapSize;
            for (int y = 0; y < mapSize.y; ++y)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    proximityMap[y, x].value = 0.0f;
                    m_threatMap[y, x].value = 0.0f;
                }
            }
            
            foreach(Faction faction in fGameManager.Instance.m_factions)
            {
                foreach(Tank tank in faction.m_tanks)
                {
                    Vector2Int tankPositionOnGrid = Utilities.convertToGridPosition(tank.transform.position);
                    createInfluence(tankPositionOnGrid, tank.m_proximityStrength, tank.m_proximityDistance);
                    createThreat(tankPositionOnGrid, tank.m_threatStrength, tank.m_threatDistance);
                }
            }
        }
    }
}