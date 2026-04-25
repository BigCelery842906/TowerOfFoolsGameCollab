//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------


using UnityEngine;

public class PG_GridBlock : MonoBehaviour
{
    [SerializeField] public Vector2 m_coords;
    [SerializeField] int m_gridNumber;
    float m_worldScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetCoords(Vector2 coords)
    {
        m_coords = coords;
    }
    public void SetGridNumber(int gridNum)
    {
        m_gridNumber = gridNum;
    }
}
