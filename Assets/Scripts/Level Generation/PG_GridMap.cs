//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------


using UnityEngine;
using System.Collections.Generic;
using System;


public class PG_GridMap : MonoBehaviour
{
    public int m_width;
    public int m_height;
    public Cell[,] m_grid;
    public List<GameObject> m_backgrounds;
    public int m_gridNumber = 0;
    private int m_roomNumber = 1;

    private float m_worldScale = 1;

    [NonSerialized]
    public bool m_roomComplete = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_backgrounds = new List<GameObject>();
    }
    public void SetWorldScale(float scale)
    {
        m_worldScale = scale;
    }
    public void SetSize(int width, int height, float scale)
    {
        m_width = width;
        m_height = height;
        m_worldScale = scale;
        m_grid = new Cell[width, height];



        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                float vertOffset = m_gridNumber * (m_height * m_worldScale) - m_worldScale;
                Vector2 cellPos = new Vector2(w * (m_worldScale), (h * m_worldScale) + vertOffset);
                Cell cell = new Cell(BLOCK_TYPE.NONE, cellPos, null);
                m_grid[w, h] = cell;
            }
        }
    }
    public float GetWorldScale() { return m_worldScale; }
    public Vector2 CalculateWorldPositionFromCoords(int x, int y)
    {
        Vector2 pos = new Vector2();
        pos.x = x * m_worldScale;
        pos.y = y * m_worldScale;
        return pos;
    }    

    public Vector2 GetWorldPosFromCell(int cellW, int cellH)
    {

        return m_grid[cellW, cellH].m_worldPosition;
    }
    public List<PG_PlatformParent> GetNeighboursOfPlatform(int xCoord, int yCoord)
    {
        List<PG_PlatformParent> neighbours = new();
        if (m_grid[xCoord - 1,yCoord].m_blockType != BLOCK_TYPE.NONE && m_grid[xCoord - 1, yCoord].m_blockType != BLOCK_TYPE.WALL)
        {
            neighbours.Add((PG_PlatformParent)m_grid[xCoord - 1, yCoord].m_contents.GetComponent<PG_PlatformParent>());
        }
        if (m_grid[xCoord + 1, yCoord].m_blockType != BLOCK_TYPE.NONE && m_grid[xCoord + 1, yCoord].m_blockType != BLOCK_TYPE.WALL)
        {
            neighbours.Add((PG_PlatformParent)m_grid[xCoord + 1, yCoord].m_contents.GetComponent<PG_PlatformParent>());
        }
        return neighbours;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum BLOCK_TYPE
    {
        NONE,
        WALL,
        PLATFORM_MIDDLE,
        PLATFORM_END        
    }
    public class Cell
    {
        public Cell(BLOCK_TYPE type,Vector2 worldPos, GameObject contents)
        {
            m_blockType = type;
            m_worldPosition = worldPos;
            m_contents = contents;
        }
        public BLOCK_TYPE m_blockType { get; private set; }
        public Vector2 m_worldPosition { get; private set; }
        public GameObject m_contents { get; private set; }
        public bool IsEmpty() => m_contents == null;
        public void ClearContents()
        {
            m_contents = null;
            m_blockType = BLOCK_TYPE.NONE;
        }
        public void SetContents(GameObject contents)
        {            
            m_contents = contents;
        }
        public void SetType(BLOCK_TYPE type)
        {
            m_blockType = type;
        }
        public void SetWorldPos(Vector2 pos)
        {
            m_worldPosition = pos;
        }

    }
}
