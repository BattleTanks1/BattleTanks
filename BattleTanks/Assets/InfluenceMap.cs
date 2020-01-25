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

//  -Choose whatever is a relevant cell size for your setup, let's say 1 unity distance unit.
//- Then you either use Mathf.Floor, Mathf.Round, or Mathf.Ceil(which ever gives best result for you, doesn't matter which you use as long as you are consistent) on both axis from your plane.

public class Point
{
    public float value = 0.0f;
    public bool visited = false;
}

public class InfluenceMap : MonoBehaviour
{
    public float maxValue;
    public float maxDistance;
    public GameObject box;
    public Vector2Int mapSize;  
    public float spacing;
    public float scalar;

    private bool spawned = false;
    Point[,] map;
    List<GameObject> m_boxes;

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

        for(int y = position.y - 1; y <= position.y + 1; y += 2)
        {
            if(y >= 0 && y < mapSize.y)
            {
                adjacentPositions.Add(new Vector2Int(position.x, y));
            }
        }

        return adjacentPositions;
    }



    // Start is called before the first frame update
    void Start()
    {
        m_boxes = new List<GameObject>();
        map = new Point[(int)mapSize.y, (int)mapSize.x];
        for(int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                map[y, x] = new Point();
            }
        }

        IEnumerator coroutine = Propogate();
        StartCoroutine(coroutine);
    }

    void createInfluence(Vector2Int position)
    {
        map[position.y, position.x].value = maxValue;
        map[position.y, position.x].visited = true;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(position);

        while (frontier.Count != 0)
        {
            Vector2Int lastPosition = frontier.Dequeue();

            foreach (Vector2Int adjacentPosition in getAdjacentPositions(lastPosition))
            { 
                if (Vector2Int.Distance(position, adjacentPosition) <= maxDistance &&
                    !map[adjacentPosition.y, adjacentPosition.x].visited)
                {
                    map[adjacentPosition.y, adjacentPosition.x].visited = true;
                    float distance = Vector2.Distance(new Vector2(adjacentPosition.x, adjacentPosition.y), new Vector2(position.x, position.y));
                    float tempMaxValue = maxValue;

                    map[adjacentPosition.y, adjacentPosition.x].value += (tempMaxValue -= maxValue * (distance / maxDistance));
                    frontier.Enqueue(adjacentPosition);

                    //Create box at location
                    Vector3 i = new Vector3(adjacentPosition.x * spacing, 0, adjacentPosition.y * spacing);

                    GameObject clone;
                    clone = Instantiate(box, i, Quaternion.identity);
                    clone.transform.localScale += new Vector3(0, map[adjacentPosition.y, adjacentPosition.x].value, 0);
                    m_boxes.Add(clone);
                }
            }
        }

        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                map[y, x].visited = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Propogate()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            foreach (GameObject box in m_boxes)
            {
                Destroy(box);
            }

            for (int y = 0; y < mapSize.y; ++y)
            {
                for (int x = 0; x < mapSize.x; ++x)
                {
                    map[y, x].value = maxValue;
                }
            }

            foreach (Tank tank in fGameManager.Instance.getAllTanks())
            {
                Vector3 tankPosition = tank.transform.position;
                tankPosition.x = Mathf.Abs(Mathf.Round(tankPosition.x));
                tankPosition.y = Mathf.Abs(Mathf.Round(tankPosition.z));

                createInfluence(new Vector2Int((int)tankPosition.x, (int)tankPosition.y));
                print("Create Influence");
            }
        }
    }
}