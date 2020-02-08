using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum TileType
{
    empty,
    filled,
    semiFilled
}

public class Chunk
{
    TileType[,] tiles = new TileType[8,8];
}

public class Map : MonoBehaviour
{
    int m_width = 12;
    int m_height = 14;

    chunk[,] m_map;

    public TileType getTile(Vector2int tileCoordinate)
    {
        Vector2int inChunkCoordinate = (tileCoordinate.x % 8, tileCoordinate.y % 8);
        Vector2int chunkCoordinate = ((tileCoordinate.x - inChunkCoordinate.x) / 8, (tileCoordinate.y - inChunkCoordinate.y) / 8);
        return m_map[chunkCoordinate.x, chunkCoordinate.y][inChunkCoordinate.x, inChunkCoordinate.y];
    }

    public Chunk getChunk(Vector2int tileCoordinate)
    {
        Vector2int inChunkCoordinate = (tileCoordinate.x % 8, tileCoordinate.y % 8);
        return  ((tileCoordinate.x - inChunkCoordinate.x) / 8, (tileCoordinate.y - inChunkCoordinate.y) / 8);
    }

    private void Awake()
    {
        m_map = new int[m_height, m_width];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}