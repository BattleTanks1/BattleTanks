using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    empty,
    filled,
    semiFilled
}

public class Chunk
{
    public TileType[,] tiles = new TileType[8,8];
}

public class Map : MonoBehaviour
{
    int m_width = 12;
    int m_height = 14;

    Chunk[,] m_map;

    public TileType getTile(Vector2Int tileCoordinate)
    {
        Vector2Int inChunkCoordinate = new Vector2Int(tileCoordinate.x % 8, tileCoordinate.y % 8);
        Vector2Int chunkCoordinate = new Vector2Int((tileCoordinate.x - inChunkCoordinate.x) / 8, (tileCoordinate.y - inChunkCoordinate.y) / 8);
        return m_map[chunkCoordinate.x, chunkCoordinate.y].tiles[inChunkCoordinate.x, inChunkCoordinate.y];
    }

    public Chunk getChunk(Vector2Int tileCoordinate)
    {
        Vector2Int inChunkCoordinate = new Vector2Int(tileCoordinate.x % 8, tileCoordinate.y % 8);
        return  m_map[(int)((tileCoordinate.x - inChunkCoordinate.x) / 8), (int)((tileCoordinate.y - inChunkCoordinate.y) / 8)];
    }

    private void Awake()
    {
        m_map = new Chunk[m_height, m_width];
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