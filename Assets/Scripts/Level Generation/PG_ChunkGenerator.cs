using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PG_ChunkGenerator : MonoBehaviour
{

    [SerializeField]
    public List<GridBlock> m_regionOneWallPool;
    public List<GridBlock> m_regionOneBlockPool;
    public List<GameObject> m_regionOneBackgroundPool;

    public int m_nextRoomEntrance = int.MaxValue;
    public int m_previousRoomExit = int.MaxValue;
    
    private GridMap m_grid;
    private float m_worldScale = 1;
    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void GenerateGrid(int w, int h)
    {
          m_grid = new GridMap(w, h);    
    }

    public void SpawnStartingRoom(int width, int height)
    {
        GenerateGrid(width, height);
        int numOfPossibleWallBlocks = m_regionOneWallPool.Count;
        int numOfPossibleBlocks = m_regionOneBlockPool.Count;
        Vector2 locationPointer = Vector2.zero;
        int exitPos = m_nextRoomEntrance;
        if (exitPos == int.MaxValue)
        {
            exitPos = UnityEngine.Random.Range(1, width - 1);

        }
        for (int w  = 0; w < m_grid.m_width; w++)
        {
            for (int h = 0;  h < m_grid.m_height; h++)
            {
                locationPointer.x = w * m_worldScale;
                locationPointer.y = h * m_worldScale;
                if (h != 0 || h != height) continue;
                else if (h == 0)
                {
                    SpawnBlock(locationPointer, numOfPossibleWallBlocks);
                }
                else if (w == exitPos && h == height) continue; //new block needed for exit volume
                else if (h == height)
                {
                    SpawnBlock(locationPointer, numOfPossibleWallBlocks);
                }
            }
            if(w == 0 || w == width)
            {
                SpawnBlock(locationPointer, numOfPossibleWallBlocks);
            }
        }
        
    }
    public void SetWorldScale(float scale)
    {
        m_worldScale = scale;
    }
    void SpawnBlock(Vector3 pos, int numOfPossibleWallBlocks)
    {
        int id = UnityEngine.Random.Range(0, numOfPossibleWallBlocks);
        GridBlock fab = m_regionOneWallPool[id];


        GridBlock block = GridBlock.Instantiate(fab, pos, Quaternion.identity);
        block.transform.localScale = Vector3.one * m_worldScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
