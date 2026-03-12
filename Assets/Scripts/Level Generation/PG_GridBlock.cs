using UnityEngine;
using UnityEngine.InputSystem.Composites;

public class GridBlock : MonoBehaviour
{
    [SerializeField] Vector2 m_coords;
    [SerializeField] int m_gridNumber;

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
