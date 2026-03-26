using UnityEngine;
using System.Collections.Generic;
[ExecuteInEditMode]
public class PG_Room : MonoBehaviour
{

    List<GameObject> m_gridMaps;
    float m_worldScale;


    private void Awake()
    {

        m_gridMaps = new List<GameObject>();
        GameObject room =this.gameObject;
        for(int i = 0; i < room.transform.childCount; i++)
        {
            if(room.transform.GetChild(i).CompareTag("Background"))
            {
                PG_BackGround bg = room.transform.GetChild(i).GetChild(0).GetComponent<PG_BackGround>();
                bg.ResizeMesh(bg.m_width, bg.m_height);
            }
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }
    public void SetGrids(List<GameObject> grids)
    {
        m_gridMaps = grids;
    }
    public void SetWorldScale(float scale)
    {
        m_worldScale = scale;
    }
 


    // Update is called once per frame
    void Update()
    {
        
    }
}
