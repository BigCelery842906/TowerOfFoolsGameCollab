using NUnit.Framework;
using System;
using UnityEngine;


public class PG_PlatformContainer : MonoBehaviour
{
    [NonSerialized]
    public PG_PlatformGenerator m_generator;
    [NonSerialized]
    public PG_GridMap m_room;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void DeletePlatform()
    {
        for(int i = 0; i < transform.gameObject.transform.childCount; i++)
        {
            PG_PlatformParent platScript = transform.gameObject.transform.GetChild(i).GetComponent<PG_PlatformParent>();
            platScript.ClearReferenceInGrid(m_room);
            Destroy(platScript.gameObject);
        }
        Destroy(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
