//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------


using System;
using System.Collections;
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
    public int m_platformYSpace = 2;
    [SerializeField]
    public int m_DesiredPlatformXStep = 1;
    [SerializeField]
    public bool m_randomisePlatformXStep = false;
    [SerializeField]
    public int m_maxPlatformRandomXStep = 2;
    [SerializeField]
    public int m_minPlatformRandomXStep = 1;
    [SerializeField]
    int m_ZigZagMaxXVariation = 5;
    [SerializeField]
    public int m_ZigZagMinPlatformSize = 4;
    [SerializeField]
    public int m_ZigZagMaxPlatformSize = 4;
    public int m_numberOfBonusPlatforms = 10;
    public bool m_areBonusPlatformFixedSize = false;
    public int m_bonusPlatformFixedSize = 3;
    public int m_bonusPlatformMinSize = 2;
    public int m_bonusPlatformMaxSize = 5;
    public int m_bonusPlatformXSeperation = 2;

    [HideInInspector]
    public bool m_generationFinished = false;

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
    public int m_platformXSpawnSize = 1;
    [HideInInspector]
    public int m_platformYSpawnSize = 1;
    [HideInInspector]
    public PG_GridMap m_currentGrid;
    [HideInInspector]
    GameObject m_currentRoom = null;
    [HideInInspector]
    public PG_GenerationManager m_genManager;

    List<GameObject> m_platformList;

    Stack<GameObject> m_platformUndoStack;

    public PLATFORM_GENERATION_METHOD m_platformGenMethod = PLATFORM_GENERATION_METHOD.NONE;
    private float m_scale;

    [SerializeField]
    int m_playerJumpDistance = 3; //How far the player can jump, in number of blocks in grid

    public void Awake()
    {
        m_platformUndoStack = new();
        m_platformList = new();

    }
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
            case PLATFORM_GENERATION_METHOD.ZIGZAG:
                ZigZag(room);
                StartCoroutine(SpawnBonusPlatforms());
                break;
        }
    }

    void RandomPlatforms(GameObject room)
    {
        //PG_GridMap grid = room.GetComponent<PG_GridMap>();
        //List<List<Change>> changes = new List<List<Change>>();
        //for (int w = 0; w < grid.m_width; w++)
        //{
        //    for (int h = 0; h < grid.m_height; h++)
        //    {
        //        PG_GridMap.Cell cell = grid.m_grid[w, h];
        //        int random = UnityEngine.Random.Range(0, m_randomHigh);
        //        if (cell.m_blockType == PG_GridMap.BLOCK_TYPE.WALL) continue;
        //        if (random > m_randomChance)
        //            continue;
        //        else
        //        {

        //            Change change = new Change();
        //            change.x = w; change.y = h;
        //            cell.SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE);
        //            change.blockType = PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE;
        //            changes.Add(change);
        //        }
        //    }
        //}
        //CommitChanges(changes, room);

    }
    void LayerPlatforms(GameObject room)
    {
        PG_GridMap grid = room.GetComponent<PG_GridMap>();
        List<List<Change>> changes = new List<List<Change>>();
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
                changes.Add(new List<Change>());
                int changeLen = changes.Count;
                changes[changeLen - 1].Add(change);
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
                    changes[changeLen - 1].Add(changeLeft);
                    changes[changeLen - 1].Add(changeRight);
                }
            }
        }
        CommitChanges(changes, room);


    }

  
    private IEnumerator SpawnBonusPlatforms()
    {
        PG_GridMap roomGrid = m_currentRoom.GetComponent<PG_GridMap>();
        int attempts = 0;
        int platformsSpawned = 0;
        int maxAttempts = m_numberOfBonusPlatforms + 50;

        while (platformsSpawned < m_numberOfBonusPlatforms && attempts < maxAttempts)
        {
            attempts++;
            int platformSize;
            if (m_areBonusPlatformFixedSize)
            {
                platformSize = m_bonusPlatformFixedSize;
            }
            else
            {
                platformSize = UnityEngine.Random.Range(m_bonusPlatformMinSize, m_bonusPlatformMaxSize + 1);
            }

            int innerAttempts = 0;
            const int MaxInnerAttempts = 8;
            bool placedAndValid = false;

            while (!placedAndValid && innerAttempts < MaxInnerAttempts)
            {
                innerAttempts++;
                int xPos = UnityEngine.Random.Range(1, Mathf.Max(m_bonusPlatformXSeperation + 1, roomGrid.m_width - platformSize - m_bonusPlatformXSeperation - 1));
                int yPos = UnityEngine.Random.Range(m_platformYSpace, roomGrid.m_height - (m_platformYSpace * 2));

                if (!SpawnPlatformAtCoords(xPos, yPos, platformSize, 1))
                {

                    continue;
                }

                GameObject platform = m_platformList[m_platformList.Count - 1];
                bool spawnInvalid = false;

                for (int i = 0; i < platform.transform.childCount; i++)
                {
                    GameObject platformBlock = platform.transform.GetChild(i).gameObject;
                    PG_PlatformParent platformScript = platformBlock.GetComponent<PG_PlatformParent>();
                    int blockX = platformScript.m_xCoord;
                    int blockY = platformScript.m_yCoord;

                    if (i == 0) // left-most: check entire left separation box
                    {
                        bool internalInvalid = false;
                        for (int x = 1; x <= m_bonusPlatformXSeperation && !internalInvalid; x++)
                        {
                            int checkX = blockX - x;
                            if (checkX < 0) break;
                            // check same-row neighbor first
                            if (roomGrid.m_grid[checkX, blockY].m_contents != null)
                            {
                                internalInvalid = true;
                                break;
                            }
                            // check vertical band for this x within +/- m_platformYSpace
                            for (int y = -m_platformYSpace; y <= m_platformYSpace; y++)
                            {
                                int checkY = blockY + y;
                                if (checkY < 0 || checkY >= roomGrid.m_height) continue;
                                if (roomGrid.m_grid[checkX, checkY].m_contents != null)
                                {
                                    internalInvalid = true;
                                    break;
                                }
                            }
                        }
                        if (internalInvalid)
                        {
                            spawnInvalid = true;
                            break;
                        }
                    }

                    if (i == platform.transform.childCount - 1) // right-most: check entire right separation box
                    {
                        bool internalInvalid = false;
                        for (int x = 1; x <= m_bonusPlatformXSeperation && !internalInvalid; x++)
                        {
                            int checkX = blockX + x;
                            if (checkX >= roomGrid.m_width) break;
                            if (roomGrid.m_grid[checkX, blockY].m_contents != null)
                            {
                                internalInvalid = true;
                                break;
                            }
                            for (int y = -m_platformYSpace; y <= m_platformYSpace; y++)
                            {
                                int checkY = blockY + y;
                                if (checkY < 0 || checkY >= roomGrid.m_height) continue;
                                if (roomGrid.m_grid[checkX, checkY].m_contents != null)
                                {
                                    internalInvalid = true;
                                    break;
                                }
                            }
                        }
                        if (internalInvalid)
                        {
                            spawnInvalid = true;
                            break;
                        }
                    }




                    for (int j = 1; j < m_platformYSpace; j++) // check up and down based on y step value
                {
                    if (blockY + j < roomGrid.m_height && roomGrid.m_grid[blockX, blockY + j].m_contents != null)
                    {
                        spawnInvalid = true;
                        break;
                    }
                    if (blockY - j >= 0 && roomGrid.m_grid[blockX, blockY - j].m_contents != null)
                    {
                        spawnInvalid = true;
                        break;
                    }
                }

                if (spawnInvalid) break;
            }

            if (spawnInvalid)
            {

                UndoPlatformPlacement();

                continue;
            }
            placedAndValid = true;
            platformsSpawned++;
                yield return null;
            }
    }


        if (attempts >= maxAttempts)
        {
            Debug.Log("Too Many Failed Bonus Platform Spawns");
        }
        if(m_genManager.m_spawnPowerups)
        {
            m_genManager.m_actionSpawnPowerups.Invoke();
        }
        yield return null;
    }


    void CriticalPath(GameObject room)
{
    LayerPlatforms(room);
    int attempts = 1;
    while (!IsPathCompleteable(room))
    {
        foreach (GameObject plat in m_platformList)
        {
            //clear incorrect path
            UndoPlatformPlacement();
        }
        if (attempts > 50)
        {
            Debug.Log("Unable to create Critical Path, LayerPlatform method used instead");
            LayerPlatforms(room);
            break;
        }
        m_platformList.Clear();
        //for now generate layer platforms
        LayerPlatforms(room);
        attempts++;
    }
    Debug.Log("Critical Path Platforms Not Yet Completed");
}
void ZigZag(GameObject room)
{
    PG_GridMap roomGrid = room.GetComponent<PG_GridMap>();
    int roomW = roomGrid.m_width;
    int roomH = roomGrid.m_height;
    bool platformsFinished = false;


    //set OOB for checking
    int exitX = roomGrid.m_width + 1;

    int exitY = roomGrid.m_height;
    for (int i = 0; i < roomGrid.m_width; i++)
    {
        if (roomGrid.m_grid[i, exitY - 1].IsEmpty())
        {
            exitX = i;
            break;
        }
    }
    if (exitX > roomGrid.m_width)
    {
        Debug.Log("Can't Find Room Exit");
        return;
    }
    int xPointer = exitX - 1;
    int yPointer = exitY - (m_platformYSpace + 1);
    SpawnPlatformAtCoords(xPointer, yPointer, 3, 1);
    while (!platformsFinished)
    {
        int xVariation = UnityEngine.Random.Range(1, m_ZigZagMaxXVariation);
        int platformSize = UnityEngine.Random.Range(m_ZigZagMinPlatformSize, m_ZigZagMaxPlatformSize);
        bool reverse = false;
        if (UnityEngine.Random.Range(0, 2) == 0) reverse = true;
        switch (reverse)
        {
            case true:
                xPointer += xVariation;
                break;
            case false:
                xPointer -= xVariation;
                break;
        }
        //xPointer += xVariation;
        if (xPointer <= 0)
        {
            xPointer = m_ZigZagMaxXVariation;
        }
        if (xPointer + platformSize >= roomW - 1)
        {
            xPointer = roomW - platformSize - 1 - m_ZigZagMaxXVariation;
        }
        yPointer -= m_platformYSpace;
        if (yPointer - 1 /*- (m_platformYSpace *2)*/ <= 0)
        {
            platformsFinished = true;
            continue;
        }
        SpawnPlatformAtCoords(xPointer, yPointer, platformSize, 1);

    }
}


void CommitChanges(List<List<Change>> changes, GameObject room)
{
    PG_GridMap grid = room.GetComponent<PG_GridMap>();
    foreach (List<Change> changeList in changes)
    {
        GameObject platformContainer = new();
        platformContainer.name = "Platform Container";
        platformContainer.AddComponent<PG_PlatformContainer>();
        platformContainer.GetComponent<PG_PlatformContainer>().m_generator = this;
        platformContainer.GetComponent<PG_PlatformContainer>().m_room = grid;
        for (int i = 0; i < changeList.Count; i++)
        {

            int x = changeList[i].x;
            int y = changeList[i].y;
            Vector3 coords = grid.CalculateWorldPositionFromCoords(x, y);
            switch (changeList[i].blockType)
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
                    middlePlatform.SetCoordinates(x, y);
                    middlePlatform.transform.localScale = Vector3.one * m_scale;
                    middlePlatform.transform.SetParent(platformContainer.gameObject.transform, false);
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
                    endPlatform.SetCoordinates(x, y);
                    endPlatform.name = "Platform " + i;
                    grid.m_grid[x, y].SetContents(endPlatform.gameObject);
                    grid.m_grid[x, y].SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_END);
                    endPlatform.transform.localScale = Vector3.one * m_scale;
                    endPlatform.transform.SetParent(platformContainer.gameObject.transform, false);
                    endPlatform.GetComponent<PG_PlatformEnd>().m_worldScale = m_scale;
                    break;

                default:
                    Debug.Log("Error, tried to commit bad platform spawn");
                    break;
            }


        }
        m_platformUndoStack.Push(platformContainer);
        platformContainer.transform.SetParent(grid.gameObject.transform, false);
        if (m_platformGenMethod == PLATFORM_GENERATION_METHOD.CRITICAL_PATH) m_platformList.Add(platformContainer);

    }

}

/// <summary>
/// Spawns platform from left most point
/// </summary>
public bool SpawnPlatformAtCoords(int x, int y, int xSize, int ySize)
{
    PG_GridMap roomGrid = m_currentRoom.GetComponent<PG_GridMap>();
    if (!roomGrid)
    {
        Debug.Log("Can't spawn platform, no grid attached to room");
        return false;
    }
    if ((x >= roomGrid.m_width && x < 1) || (y >= roomGrid.m_height && y < 1))
    {
        //Debug.Log("Can't spawn platform, spawn location not valid");
        return false;
    }
    if (x + xSize >= roomGrid.m_width)
    {
        //Debug.Log("Can't spawn platform, Would overlap wall");
        return false;
    }
    //loop though and check if platforms overlap
    for (int i = 0; i < ySize; i++)
    {
        for (int j = 0; j < xSize; j++)
        {
            PG_GridMap.Cell currentCell = roomGrid.m_grid[x + j, y + i];
            if (currentCell.m_blockType != PG_GridMap.BLOCK_TYPE.NONE)
            {
                //Debug.Log("Can't spawn platform, overlapping other platform");
                return false;
            }
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
    GameObject platformContainer = new();
    platformContainer.name = "Platform Container";
    platformContainer.AddComponent<PG_PlatformContainer>();
    platformContainer.GetComponent<PG_PlatformContainer>().m_generator = this;
    platformContainer.GetComponent<PG_PlatformContainer>().m_room = roomGrid;
    for (int i = 0; i < ySize; i++)
    {
        for (int j = 0; j < xSize; j++)
        {

            if (j == 0 || j == xSize - 1)
            {
                Vector3 coords = roomGrid.CalculateWorldPositionFromCoords(x + j, y + i);
                PG_PlatformEnd endPlatform = PG_PlatformEnd.Instantiate(endFab, this.transform.position + coords, this.transform.rotation);
                endPlatform.name = "Platform " + j;
                roomGrid.m_grid[x + j, y + i].SetContents(endPlatform.gameObject);
                roomGrid.m_grid[x + j, y + i].SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_END);
                endPlatform.SetCoordinates(x + j, y + i);
                //Debug.Log($"Tile number {x + i}, {y} set to {PG_GridMap.BLOCK_TYPE.PLATFORM_END}");
                endPlatform.transform.localScale = Vector3.one * m_scale;
                endPlatform.transform.SetParent(platformContainer.gameObject.transform, false);
                endPlatform.GetComponent<PG_PlatformEnd>().m_worldScale = m_scale;
                endPlatform.SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_END);
            }
            else
            {
                Vector3 coords = roomGrid.CalculateWorldPositionFromCoords(x + j, y + i);
                PG_PlatformMiddle middlePlatform = PG_PlatformMiddle.Instantiate(middleFab, this.transform.position + coords, this.transform.rotation);
                middlePlatform.name = "Platform " + j;
                roomGrid.m_grid[x + j, y + i].SetContents(middlePlatform.gameObject);
                roomGrid.m_grid[x + j, y + i].SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE);
                middlePlatform.SetCoordinates(x + j, y + i);
                //Debug.Log($"Tile number {x + i}, {y} set to {PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE}");
                middlePlatform.transform.localScale = Vector3.one * m_scale;
                middlePlatform.transform.SetParent(platformContainer.gameObject.transform, false);
                middlePlatform.GetComponent<PG_PlatformMiddle>().m_worldScale = m_scale;
                middlePlatform.SetType(PG_GridMap.BLOCK_TYPE.PLATFORM_MIDDLE);
            }
        }
    }
    platformContainer.transform.SetParent(roomGrid.gameObject.transform, false);

    m_platformUndoStack.Push(platformContainer);
    m_platformList.Add(platformContainer);
    return true;

}
public void UndoPlatformPlacement()
{
    GameObject toDelete = null;

    while (toDelete == null)
    {
        if (m_platformUndoStack.Count == 0)
        {
            Debug.Log("No Platforms To Remove");
            return;
        }
        toDelete = m_platformUndoStack.Pop();
        if (toDelete == null)
        {
            Debug.Log("Tried to Undo platform that has already been deleted, skipped to next in stack");
        }
    }
    PG_GridMap roomGrid = m_currentRoom.GetComponent<PG_GridMap>();

    for (int i = 0; i < toDelete.transform.childCount; i++)
    {
        GameObject platform = toDelete.transform.GetChild(i).gameObject;
        PG_PlatformParent platformScript = (PG_PlatformParent)platform.GetComponent<PG_PlatformParent>();
        platformScript.ClearReferenceInGrid(roomGrid);


    }
    Destroy(toDelete);

}
bool IsPathCompleteable(in GameObject room)
{
    //get current level exit coords
    PG_GridMap roomGrid = room.GetComponent<PG_GridMap>();

    //set OOB for checking
    int exitX = roomGrid.m_width + 1;

    int exitY = roomGrid.m_height;
    for (int i = 0; i < roomGrid.m_width; i++)
    {
        if (roomGrid.m_grid[i, exitY - 1].IsEmpty())
        {
            exitX = i;
            break;
        }
    }
    if (exitX > roomGrid.m_width)
    {
        Debug.Log("Can't Find Room Exit");
        return false;
    }

    //check player can reach exit from final platform
    bool playerCanReachEndFromFinal = false;
    GameObject finalPlatform = m_platformList[m_platformList.Count - 1];

    for (int i = 0; i < finalPlatform.transform.childCount; i++)
    {
        GameObject plat = finalPlatform.transform.GetChild(i).gameObject;
        PG_PlatformParent s = plat.GetComponent<PG_PlatformParent>();
        float squDist = roomGrid.GetSquaredUnitDistanceBetweenCells(exitX, exitY, s.m_xCoord, s.m_yCoord);
        Debug.Log("squ distance" + squDist);
        Debug.Log("squ jump" + (m_playerJumpDistance * m_playerJumpDistance));
        if (squDist < m_playerJumpDistance)
        {
            playerCanReachEndFromFinal = true;
            break;
        }
    }

    //check each platform is not directly above previous

    //check player can reach platform from previous

    //check player can reach first platform from floor

    if (playerCanReachEndFromFinal)
    {
        return true;
    }
    else return false;
}
public enum PLATFORM_GENERATION_METHOD
{
    NONE,
    RANDOM,
    LAYER,
    CRITICAL_PATH,
    ZIGZAG
}
struct Change
{
    public int x, y;
    public PG_GridMap.BLOCK_TYPE blockType;
}
}


