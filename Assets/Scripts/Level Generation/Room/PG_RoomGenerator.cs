//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------



using System.Collections.Generic;
using UnityEngine;

public class PG_RoomGenerator : MonoBehaviour
{

    [SerializeField]
    public List<PG_GridBlock> m_regionOneWallPool;
    public List<PG_GridBlock> m_regionOneBlockPool;
    public List<PG_BackGround> m_regionOneBackgroundPool;

    int m_previousBackgroundIndex = int.MaxValue;


    public int m_nextRoomEntrance = int.MaxValue;
    public int m_previousRoomExit = int.MaxValue;

    private float m_worldScale = 1;
    private GameObject m_grid;
    private List<GameObject> m_grids;
    private int m_numGrids;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public GameObject GenerateRoom(int w, int h, float worldScale, int chunks)
    {
        m_numGrids = chunks;
        m_grids = new List<GameObject>();
        m_worldScale = worldScale;
        for (int i = 0; i < chunks; i++)
        {
            GameObject gridObj = GenerateGrid(w, h, worldScale, i);
            PG_GridMap grid = gridObj.GetComponent<PG_GridMap>();


            m_grids.Add(gridObj);
            SpawnBackGround(ref grid);
        }
        GameObject room = StitchRoom();
        PG_GridMap roomGrid = room.GetComponent<PG_GridMap>();
        AddValuesToGrid(ref roomGrid);
        SpawnBlocksInGrid(ref roomGrid);
        room.AddComponent<PG_Room>();
        PG_Room roomScript = room.GetComponent<PG_Room>();
        roomScript.SetGrids(m_grids);
        for (int i = 0; i < m_grids.Count; i++)
        {
            Destroy(m_grids[i]);
        }
        room.GetComponent<PG_GridMap>().m_roomComplete = true;
        return room;
    }
    public GameObject GenerateGrid(int w, int h, float worldScale, int gridNumber)
    {
        GameObject gridObj = new GameObject("Room");
        gridObj.transform.SetParent(this.transform, false);
        PG_GridMap grid = gridObj.AddComponent<PG_GridMap>();
        grid.m_gridNumber = gridNumber;
        if (!grid)
        {
            Debug.Log("No grid attached");
        }

        grid.SetSize(w, h, worldScale);
        return gridObj;

    }
    GameObject StitchRoom()
    {
        PG_GridMap baseGrid = m_grids[0].GetComponent<PG_GridMap>();
        int roomHeight = 0;
        int roomWidth = baseGrid.m_width; // grids will be the same width
        for (int g = 0; g < m_grids.Count; g++)
        {
            PG_GridMap grid = m_grids[g].GetComponent<PG_GridMap>();
            int gridHeight = grid.m_height;
            roomHeight += gridHeight;

        }
        GameObject room = GenerateGrid(roomWidth, roomHeight, m_worldScale, 0);
        PG_GridMap roomGrid = room.GetComponent<PG_GridMap>();

        int culOffset = 0;
        float worldYOffset = 0;
        for (int g = 0; g < m_grids.Count; g++)
        {
            PG_GridMap grid = m_grids[g].GetComponent<PG_GridMap>();
            int gridHeight = grid.m_height;
            int gridWidth = grid.m_width;

            for (int w = 0; w < gridWidth; w++)
            {
                for (int h = 0; h < gridHeight; h++)
                {
                    int newHeight = h + culOffset;

                    roomGrid.m_grid[w, newHeight].SetType(grid.m_grid[w, h].m_blockType);
                    roomGrid.m_grid[w, newHeight].SetContents(grid.m_grid[w, h].m_contents);
                    Vector2 adjustedWorldPos;
                    adjustedWorldPos.x = grid.m_grid[w, h].m_worldPosition.x;
                    adjustedWorldPos.y = grid.m_grid[w, h].m_worldPosition.y + (roomGrid.m_grid[w, newHeight].m_worldPosition.y * g);
                    roomGrid.m_grid[w, newHeight].SetWorldPos(new Vector2(w * m_worldScale, newHeight * m_worldScale));
                }

            }
            culOffset += gridHeight;
            worldYOffset += (gridHeight * m_worldScale);
            roomGrid.m_backgrounds.Add(grid.m_backgrounds[0]); //individual grids should never have more than one background in the list
        }
        for (int i = 0; i < roomGrid.m_backgrounds.Count; i++)
        {
            roomGrid.m_backgrounds[i].transform.SetParent(room.transform, false);

        }
        return room;
    }



    public void AddValuesToGrid(ref PG_GridMap grid)
    {
        int exitPos = m_nextRoomEntrance;
        int entrancePos = m_previousRoomExit;
        if (exitPos == int.MaxValue)
        {
            exitPos = UnityEngine.Random.Range(1, grid.m_width - 1);
        }

        for (int w = 0; w < grid.m_width; w++)
        {
            for (int h = 0; h < grid.m_height; h++)
            {
                if ((h == 0 || h == grid.m_height - 1)
                    || (w == 0 || w == grid.m_width - 1))
                {
                    grid.m_grid[w, h].SetType(PG_GridMap.BLOCK_TYPE.WALL);
                }
                if (h == grid.m_height - 1 && w == exitPos)
                {
                    grid.m_grid[w, h].SetType(PG_GridMap.BLOCK_TYPE.NONE);
                }
                if(h == 0 && w == entrancePos)
                {
                    grid.m_grid[w, h].SetType(PG_GridMap.BLOCK_TYPE.NONE);
                }
            }
        }
    }
    void SpawnBlocksInGrid(ref PG_GridMap grid)
    {
        int numOfPossibleWallBlocks = m_regionOneWallPool.Count;
        int numOfPossibleBlocks = m_regionOneBlockPool.Count;
        Vector2 coords = Vector2.zero;
        for (int w = 0; w < grid.m_width; w++)
        {
            for (int h = 0; h < grid.m_height; h++)
            {
                switch (grid.m_grid[w, h].m_blockType)
                {
                    case PG_GridMap.BLOCK_TYPE.NONE:
                        break;
                    case PG_GridMap.BLOCK_TYPE.WALL:
                        coords.x = w; coords.y = h;
                        int id = UnityEngine.Random.Range(0, numOfPossibleWallBlocks);
                        SpawnBlock(w, h, id, ref grid);

                        break;
                    default:
                        break;
                }
            }
        }
    }
    public void SpawnBackGround(ref PG_GridMap grid)
    {
        int randNum = UnityEngine.Random.Range(0, m_regionOneBackgroundPool.Count);

        while(m_previousBackgroundIndex == randNum)
        {
            randNum = UnityEngine.Random.Range(0, m_regionOneBackgroundPool.Count);
        }
        PG_BackGround fab = m_regionOneBackgroundPool[randNum];
        m_previousBackgroundIndex = randNum;
        if (!fab)
        {
            Debug.Log("background not found");
            return;
        }
        float gridHeight = grid.m_height * m_worldScale;
        float gridWidth = grid.m_width * m_worldScale;
        float vertOffset = gridHeight * grid.m_gridNumber - (m_worldScale * 0.5f);

        Vector3 pos;
        pos.x = gridWidth * 0.5f - (m_worldScale * 0.5f); //offset to fit
        pos.y = gridHeight * 0.5f + vertOffset;
        pos.z = m_worldScale * 0.5f;

        GameObject backgroundObj = new GameObject("Background " + (grid.m_gridNumber + 1));
        PG_BackGround background = PG_BackGround.Instantiate(fab, this.transform.position + pos, this.transform.rotation);
        backgroundObj.tag = "Background";

        background.transform.SetParent(backgroundObj.transform, false);
        background.ResizeMesh(gridWidth - (m_worldScale * 2), gridHeight);
        background.m_width = gridWidth - (m_worldScale * 2);
        background.m_height = gridHeight;


        background.transform.localScale = Vector3.one;


        if (grid.m_backgrounds.Count > 0)
        {
            Debug.Log("Grid already has a background");
        }
        else grid.m_backgrounds.Add(backgroundObj);
    }

    public void SetWorldScale(float scale)
    {
        for (int i = 0; i < m_grids.Count; i++)
        {
            m_grid.GetComponent<PG_GridMap>().SetWorldScale(scale);

        }
        m_worldScale = scale;
    }
    void SpawnBlock(int x, int y, int id, ref PG_GridMap grid)
    {

        Vector3 coords = grid.CalculateWorldPositionFromCoords(x, y);
        PG_GridBlock fab = m_regionOneWallPool[id];
        PG_GridBlock block = PG_GridBlock.Instantiate(fab, this.transform.position + coords, this.transform.rotation);
        block.name = "GridBlock " + (x + 1) + ", " + (y + 1);
        grid.m_grid[x, y].SetContents(block.gameObject);
        block.transform.localScale = Vector3.one * m_worldScale;
        Vector2 gridLoc = new Vector2(x, y);
        block.SetCoords(gridLoc);
        block.transform.SetParent(grid.gameObject.transform, false);
        if (y == 0 || y == grid.m_height - 1)
        {
            block.gameObject.layer = LayerMask.NameToLayer( "Ground" );
        }

    }


    // Update is called once per frame
    void Update()
    {

    }


}
