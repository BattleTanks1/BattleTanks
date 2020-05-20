using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
            //Feeding opposite teams threat maps in
            Pathfinder.Instance.updateDangerMap((int)eFactionName.Blue, m_threatMaps[(int)eFactionName.Red].m_map);
            Pathfinder.Instance.updateDangerMap((int)eFactionName.Red, m_threatMaps[(int)eFactionName.Blue].m_map);
        }
    }
}