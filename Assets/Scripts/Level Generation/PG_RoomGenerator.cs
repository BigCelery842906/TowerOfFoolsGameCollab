using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PG_RoomGenerator : MonoBehaviour
{

    [SerializeField]
    public List<GridBlock> m_regionOneWallPool;
    public List<GridBlock> m_regionOneBlockPool;
    public List<PG_BackGround> m_regionOneBackgroundPool;

    public int m_nextRoomEntrance = int.MaxValue;
    public int m_previousRoomExit = int.MaxValue;

    private float m_worldScale = 1;
    private PG_GridMap m_grid;
    private List<PG_GridMap> m_grids;
    private int m_numGrids;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void GenerateRoom(int w, int h, float worldScale, int chunks)
    {
        m_numGrids = chunks;
        m_grids = new List<PG_GridMap>();
        m_worldScale = worldScale;
        for (int i = 0; i < chunks; i++)
        {
            PG_GridMap grid = GenerateGrid(w, h, worldScale);
            grid.m_gridNumber = i;
            
            m_grids.Add(GenerateGrid(w, h, worldScale));
            AddValuesToGrid( ref grid,chunks);
            SpawnBlocksInGrid(ref grid);
            SpawnBackGround(ref grid);
        }
    }
    public PG_GridMap GenerateGrid(int w, int h, float worldScale)
    {
        PG_GridMap grid = GetComponent<PG_GridMap>();
        if (!grid)
        {
            Debug.Log("No grid attached");
        }

        grid.SetSize(w, h, worldScale);
        return grid;

    }
    
    public void AddValuesToGrid(ref PG_GridMap grid, int chunks)
    {
        

        int exitPos = m_nextRoomEntrance;
        int entrancePos = m_previousRoomExit;
        bool isLastGrid = grid.m_gridNumber == chunks - 1;
        bool isFirstGrid = grid.m_gridNumber == 0;

        if (exitPos == int.MaxValue)
        {
            exitPos = UnityEngine.Random.Range(1, grid.m_width - 1);

        }
        for (int w = 0; w < grid.m_width; w++)
        {
            for (int h = 0; h < grid.m_height; h++)
            {


                if ((h == grid.m_height - 1 && w == exitPos && isLastGrid) //room exit
                    || h == grid.m_height - 1                              //room middle
                    && !isLastGrid 
                    && w != 0 
                    && w != grid.m_width -1) 
                {
                    grid.m_grid[w, h] = PG_GridMap.BLOCK_TYPE.NONE;
                    continue;
                }

                if((h == 0 && isFirstGrid) || w == 0 || w == grid.m_width - 1 ||(isLastGrid && h == grid.m_height - 1) )

                {

                    grid.m_grid[w, h] = PG_GridMap.BLOCK_TYPE.WALL;

                }

            }

        }

    }

    void SpawnBlocksInGrid(ref PG_GridMap grid)
    {
        int numOfPossibleWallBlocks = m_regionOneWallPool.Count;
        int numOfPossibleBlocks = m_regionOneBlockPool.Count;
        Vector2 coords = Vector2.zero;
        for (int w = 0;w < grid.m_width;w++)
        {
            for (int h = 0; h < grid.m_height;h++)
            {
                switch(grid.m_grid[w,h])
                {
                    case PG_GridMap.BLOCK_TYPE.NONE:
                        break;
                    case PG_GridMap.BLOCK_TYPE.WALL:
                        coords.x = w; coords.y = h;
                        int id = UnityEngine.Random.Range(0, numOfPossibleWallBlocks);
                        SpawnBlock(w,h,id, grid.m_gridNumber);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void SpawnBackGround(ref PG_GridMap grid)
    {
        PG_BackGround fab = m_regionOneBackgroundPool[0]; //first for testing
        if(!fab)
        {
            Debug.Log("background not found");
            return;
        }
        float gridHeight = grid.m_height * m_worldScale;
        float gridWidth = grid.m_width * m_worldScale;
        float vertOffset = gridHeight * grid.m_gridNumber - (m_worldScale * 0.5f);
        //if (grid.m_gridNumber != 0 && grid.m_gridNumber != m_numGrids - 1)
        //{
        //    vertOffset -= m_worldScale;
        //}
        //if (grid.m_gridNumber == m_numGrids - 1)
        //{
        //    vertOffset -= m_worldScale * 0.5f;
        //}
        Vector3 pos;
        pos.x = gridWidth * 0.5f - (m_worldScale * 0.5f);
        pos.y = gridHeight * 0.5f + vertOffset - m_worldScale;
        pos.z = m_worldScale * 0.5f;


        PG_BackGround backGround = PG_BackGround.Instantiate(fab, this.transform.position + pos, this.transform.rotation);


        //if (grid.m_gridNumber == 0 )
        //{
        //    backGround.ResizeMesh(gridWidth - m_worldScale, gridHeight - m_worldScale);
        //}
        //else if(grid.m_gridNumber != m_numGrids - 1)
        //{
        //    backGround.ResizeMesh(gridWidth - m_worldScale, gridHeight - (m_worldScale * 0.5f));
        //}
        //else
        //{
            backGround.ResizeMesh(gridWidth - (m_worldScale * 2), gridHeight);
        //}
            
        backGround.transform.localScale = Vector3.one;
    }
    void RegenerateStartingRoom(int width, int height)
    {

    }
    public void SetWorldScale(float scale)
    {
        for(int i = 0; i < m_grids.Count; i++)
        {
        m_grid.SetWorldScale(scale);

        }
        m_worldScale = scale;
    }
    void SpawnBlock(int x, int y, int id, int gridNum)
    {
        Vector3 coords = m_grids[gridNum].GetWorldPosFromCell(x, y);
        GridBlock fab = m_regionOneWallPool[id];
        GridBlock block = GridBlock.Instantiate(fab, this.transform.position + coords, this.transform.rotation);
        block.transform.localScale = Vector3.one * m_worldScale;
        Vector2 gridLoc = new Vector2(x, y);
        block.SetCoords(gridLoc);
        block.SetGridNumber(gridNum);
    }

    // Update is called once per frame
    void Update()
    {

    }


}
