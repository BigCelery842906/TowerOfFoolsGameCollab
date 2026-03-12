using UnityEngine;

public class PG_GridMap : MonoBehaviour
{
    public int m_width;
    public int m_height;
    public BLOCK_TYPE[,] m_grid;
    public int m_gridNumber = 0;
    private int m_roomNumber = 1;
    private Vector2[,] m_gridPosition;
    private float m_worldScale = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetWorldScale(float scale)
    {
        m_worldScale = scale;
    }
    public void SetSize(int width, int height, float scale)
    {
        m_width = width;
        m_height = height;
        m_worldScale = scale;
        m_grid = new BLOCK_TYPE[width, height];
        m_gridPosition = new Vector2[width, height];


        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                float vertOffset = m_gridNumber * (m_height * m_worldScale) - m_worldScale;
                m_gridPosition[w, h].x = w * (m_worldScale);
                m_gridPosition[w, h].y = (h * m_worldScale) + vertOffset;
            }
        }
    }


    public Vector2 GetWorldPosFromCell(int cellW, int cellH)
    {
        //float gridHeight = m_height * m_worldScale;
        return m_gridPosition[cellW, cellH];
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
}
