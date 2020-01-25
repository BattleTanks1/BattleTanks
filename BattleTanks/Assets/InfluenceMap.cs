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

public class Point
{
    public float value = 0.0f;
    public bool visited = false;
}

public class InfluenceMap : MonoBehaviour
{
    public float origMaxValue;
    public float maxValue;
    public float maxDistance;
    public GameObject box;
    public Vector2Int mapSize;  
    public float spacing;
    public float scalar;
    Point[,] map;

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
        map = new Point[(int)mapSize.y, (int)mapSize.x];
        for(int y = 0; y < mapSize.y; ++y)
        {
            for(int x = 0; x < mapSize.x; ++x)
            {
                map[y, x] = new Point();
            }
        }




        //        -Choose whatever is a relevant cell size for your setup, let's say 1 unity distance unit.
        //- Then you either use Mathf.Floor, Mathf.Round, or Mathf.Ceil(which ever gives best result for you, doesn't matter which you use as long as you are consistent) on both axis from your plane.

        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                foreach(Tank tank in fGameManager.Instance.getAllTanks())
                {
                    Vector3 tankPosition = tank.transform.position;
                    tankPosition.x = Mathf.Abs(Mathf.Round(tankPosition.x));
                    tankPosition.y = Mathf.Abs(Mathf.Round(tankPosition.z));

                    createInfluence(new Vector2Int((int)tankPosition.x, (int)tankPosition.y));
                }
            }
        }

        for (int y = 0; y < mapSize.y; ++y)
        {
            for (int x = 0; x < mapSize.x; ++x)
            {
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);

                GameObject clone;
                clone = Instantiate(box, position, Quaternion.identity);
                clone.transform.localScale += new Vector3(0, map[y, x].value + (map[y, x].value / 2.0f), 0);
            }
        }
    }

    void createInfluence(Vector2Int position)
    {
        map[position.y, position.x].value = 1;
        map[position.y, position.x].visited = true;

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(position);

        while (frontier.Count != 0)
        {
            Vector2Int lastPosition = frontier.Dequeue();

            foreach (Vector2Int adjacentPosition in getAdjacentPositions(lastPosition))
            {
                if (!map[adjacentPosition.y, adjacentPosition.x].visited)
                {

                    if (maxValue <= 0)
                    {
                        print("Hi");
                        maxValue = origMaxValue;
                        return;
                    }
                    map[adjacentPosition.y, adjacentPosition.x].visited = true;
                    float distance = Vector2.Distance(new Vector2(adjacentPosition.x, adjacentPosition.y), new Vector2(position.x, position.y));
                    maxValue -= maxValue * (distance / maxDistance);

                    map[adjacentPosition.y, adjacentPosition.x].value = maxValue;
                    frontier.Enqueue(adjacentPosition);
                }
            }
        }

        maxValue = origMaxValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
