//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------


using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class PG_PlatformGenerator : MonoBehaviour
{
    [SerializeField]
    int m_randomChance = 0;
    [SerializeField]
    int m_randomHigh = 10;

    [SerializeField]
    int m_platformYSpace = 2;
    [SerializeField]
    int m_DesiredPlatformXStep = 1;
    [SerializeField]
    bool m_randomisePlatformXStep = false;
    [SerializeField]
    int m_maxPlatformRandomXStep = 2;
    [SerializeField]
    int m_minPlatformRandomXStep = 1;

    [SerializeField]
    List<PG_PlatformMiddle> m_middles;
    [SerializeField]
    List<PG_PlatformEnd> m_ends;

    [HideInInspector]
    public PG_PlatformMiddle m_middleToSpawn;
    [HideInInspector]
    public PG_PlatformEnd m_endToSpawn;
    [HideInInspector]
    public int m_xSpawnLocation = 1;
    [HideInInspector]
    public int m_ySpawnLocation = 1;
    [HideInInspector]
    public int m_platformSpawnSize = 1;
    [HideInInspector]
    public PG_GridMap m_currentGrid;
    [HideInInspector]
    GameObject m_currentRoom = null;

    public PLATFORM_GENERATION_METHOD m_platformGenMethod = PLATFORM_GENERATION_METHOD.NONE;
    private float m_scale;

    public void GeneratePlatforms(GameObject room, float worldScale)
    {
        m_scale = worldScale;
        m_currentGrid = room.GetComponent<PG_GridMap>();
        m_currentRoom = room;
        switch (m_platformGenMethod)
        {
            case PLATFORM_GENERATION_METHOD.NONE:
                Debug.Log("No platforms spawned, no generation method specified");
                break;
            case PLATFORM_GENERATION_METHOD.RANDOM:
                RandomPlatforms(room);
                break;
            case PLATFORM_GENERATION_METHOD.LAYER:
                LayerPlatforms(room);
                break;
            case PLATFORM_GENERATION_METHOD.CRITICAL_PATH:
                CriticalPath(room);
                break;
        }
    }

    void RandomPlatforms(GameObject room)
    {
        PG_GridMap grid = room.GetComponent<PG_GridMap>();
        List<Change> changes = new List<Change>();
        for (int w = 0; w < grid.m_width; w++)
        {
            for (int h = 0; h < grid.m_height; h++)
            {
                PG_GridMap.Cell cell = grid.m_grid[w, h];
                int random = UnityEngine.Random.Range(0, m_randomHigh);
                if (cell.m_blockType == PG_GridMap.BLOCK_TYPE.WALL) continue;
                if (random > m_randomChance)
                    continue;
                else
                {
                    Change change = new Change();
                    change.x = w; change.y = h;
                    cell.SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE);
                    change.blockType = PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE;
                    changes.Add(change);
                }
            }
        }
        CommitChanges(changes, room);

    }
    void LayerPlatforms(GameObject room)
    {
        PG_GridMap grid = room.GetComponent<PG_GridMap>();
        List<Change> changes = new List<Change>();
        int gridWidth = grid.m_width - 2;

        for (int h = 0; h < grid.m_height - 2; h++)
        {
            if (h < m_platformYSpace) continue;
            if (h % m_platformYSpace == 0)
            {
                int maxSteps;
                if (m_randomisePlatformXStep)
                {
                    maxSteps = UnityEngine.Random.Range(m_minPlatformRandomXStep, m_maxPlatformRandomXStep);
                }
                else maxSteps = m_DesiredPlatformXStep;
                int x = UnityEngine.Random.Range(0 + maxSteps + 1, grid.m_width - maxSteps - 1);
                bool platformFinished = false;
                Change change = new Change();
                change.x = x;
                change.y = h;
                change.blockType = PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE;
                changes.Add(change);
                int xSize = 1;
                if (maxSteps == 0) platformFinished = true;
                int leftOffset = -1;
                int rightOffset = 1;
                while (!platformFinished)
                {

                    Change changeLeft = new Change();
                    Change changeRight = new Change();

                    changeLeft.x = x + leftOffset;
                    leftOffset--;
                    changeRight.x = x + rightOffset;
                    rightOffset++;
                    changeLeft.y = h;
                    changeRight.y = h;
                    xSize += 2;
                    if (xSize >= 1 + (maxSteps * 2))
                    {
                        platformFinished = true;
                        changeLeft.blockType = PG_GridMap.BLOCK_TYPE.PLATFORM_END;
                        changeRight.blockType = PG_GridMap.BLOCK_TYPE.PLATFORM_END;
                    }
                    else
                    {
                        changeLeft.blockType = PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE;
                        changeRight.blockType = PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE;
                    }
                    changes.Add(changeLeft);
                    changes.Add(changeRight);
                }
            }
        }
        CommitChanges(changes, room);


    }
    void CriticalPath(GameObject room)
    {
        Debug.Log("Critical Path Platforms Not Yet Implemented");

    }

    void CommitChanges(List<Change> changes, GameObject room)
    {
        PG_GridMap grid = room.GetComponent<PG_GridMap>();

        for (int i = 0; i < changes.Count; i++)
        {

            int x = changes[i].x;
            int y = changes[i].y;
            Vector3 coords = grid.CalculateWorldPositionFromCoords(x, y);
            switch (changes[i].blockType)
            {
                case PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE:


                    if (m_middles.Count == 0)
                    {
                        Debug.Log("No Platform Middles Attached");
                        return;
                    }
                    int choice = UnityEngine.Random.Range(0, m_middles.Count);
                    PG_PlatformMiddle middleFab = m_middles[choice];
                    PG_PlatformMiddle middlePlatform = PG_PlatformMiddle.Instantiate(middleFab, this.transform.position + coords, this.transform.rotation);
                    middlePlatform.name = "Platform " + i;
                    grid.m_grid[x, y].SetContents(middlePlatform.gameObject);
                    grid.m_grid[x, y].SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE);
                    middlePlatform.transform.localScale = Vector3.one * m_scale;
                    middlePlatform.transform.SetParent(grid.gameObject.transform, false);
                    middlePlatform.GetComponent<PG_PlatformMiddle>().m_worldScale = m_scale;
                    break;
                case PG_GridMap.BLOCK_TYPE.PLATFORM_END:
                    if (m_ends.Count == 0)
                    {
                        Debug.Log("No Platform Ends Attached");
                        return;
                    }
                    choice = UnityEngine.Random.Range(0, m_middles.Count);
                    PG_PlatformEnd endFab = m_ends[choice];
                    PG_PlatformEnd endPlatform = PG_PlatformEnd.Instantiate(endFab, this.transform.position + coords, this.transform.rotation);
                    endPlatform.name = "Platform " + i;
                    grid.m_grid[x, y].SetContents(endPlatform.gameObject);
                    grid.m_grid[x, y].SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_END);
                    endPlatform.transform.localScale = Vector3.one * m_scale;
                    endPlatform.transform.SetParent(grid.gameObject.transform, false);
                    endPlatform.GetComponent<PG_PlatformEnd>().m_worldScale = m_scale;
                    break;

                default:
                    Debug.Log("Error, tried to commit bad platform spawn");
                    break;
            }


        }

    }
    /// <summary>
    /// Spawns platform from left most point
    /// </summary>
    public void SpawnPlatformAtCoords(int x, int y, int size)
    {
        PG_GridMap roomGrid = m_currentRoom.GetComponent<PG_GridMap>();
        if (!roomGrid)
        {
            Debug.Log("Can't spawn platform, no grid attached to room");
            return;
        }
        if ((x >= roomGrid.m_width && x < 1) || (y >= roomGrid.m_height && y < 1))
        {
            Debug.Log("Can't spawn platform, spawn location not valid");
            return;
        }
        if (x + size >= roomGrid.m_width)
        {
            Debug.Log("Can't spawn platform, Would overlap wall");
            return;
        }
        //loop though and check if platforms overlap
        for (int i = 0; i < size; i++)
        {
            PG_GridMap.Cell currentCell = roomGrid.m_grid[x + i, y];
            if (currentCell.m_blockType != PG_GridMap.BLOCK_TYPE.NONE)
            {
                Debug.Log("Can't spawn platform, overlapping other platform");
                return;
            }
        }
        PG_PlatformEnd endFab = m_endToSpawn;
        PG_PlatformMiddle middleFab = m_middleToSpawn;


        if (!endFab)
        {
            int choice = UnityEngine.Random.Range(0, m_middles.Count);
            endFab = m_ends[choice];
        }
        if (!middleFab)
        {
            int choice = UnityEngine.Random.Range(0, m_middles.Count);
            middleFab = m_middles[choice];
        }
        for (int i = 0; i < size; i++)
        {
            
            if(i == 0 || i == size -1)
            {
                Vector3 coords = roomGrid.CalculateWorldPositionFromCoords(x + i, y);
                PG_PlatformEnd endPlatform = PG_PlatformEnd.Instantiate(endFab, this.transform.position + coords, this.transform.rotation);
                endPlatform.name = "Platform " + i;
                roomGrid.m_grid[x + i, y].SetContents(endPlatform.gameObject);
                roomGrid.m_grid[x + i, y].SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_END);
                //Debug.Log($"Tile number {x + i}, {y} set to {PG_GridMap.BLOCK_TYPE.PLATFORM_END}");
                endPlatform.transform.localScale = Vector3.one * m_scale;
                endPlatform.transform.SetParent(roomGrid.gameObject.transform, false);
                endPlatform.GetComponent<PG_PlatformEnd>().m_worldScale = m_scale;
            }
            else
            {
                Vector3 coords = roomGrid.CalculateWorldPositionFromCoords(x + i, y);
                PG_PlatformMiddle middlePlatform = PG_PlatformMiddle.Instantiate(middleFab, this.transform.position + coords, this.transform.rotation);
                middlePlatform.name = "Platform " + i;
                roomGrid.m_grid[x +i, y].SetContents(middlePlatform.gameObject);
                roomGrid.m_grid[x + i, y].SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE);
                //Debug.Log($"Tile number {x + i}, {y} set to {PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE}");
                middlePlatform.transform.localScale = Vector3.one * m_scale;
                middlePlatform.transform.SetParent(roomGrid.gameObject.transform, false);
                middlePlatform.GetComponent<PG_PlatformMiddle>().m_worldScale = m_scale;
            }
        }

    }
    public enum PLATFORM_GENERATION_METHOD
    {
        NONE,
        RANDOM,
        LAYER,
        CRITICAL_PATH
    }
    struct Change
    {
        public int x, y;
        public PG_GridMap.BLOCK_TYPE blockType;
    }
}


