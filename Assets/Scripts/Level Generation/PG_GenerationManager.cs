using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PG_GenerationManager : MonoBehaviour
{
    [SerializeField]
    public int m_desiredChunkWidth;
    [SerializeField]
    public int m_desiredChunkHeight;
    public int m_chunksPerRoom;

    public float m_worldScale;

    private PG_RoomGenerator m_roomGenerator;
    private REGION m_currentRegion = REGION.ONE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_roomGenerator = GetComponent<PG_RoomGenerator>();
        if(!m_roomGenerator)
        {
            Debug.Log("Chunk Generator not Loaded on Generation Manager");
        }
        m_roomGenerator.GenerateRoom(m_desiredChunkWidth, m_desiredChunkHeight, m_worldScale, m_chunksPerRoom);
        //m_roomGenerator.SetWorldScale(m_worldScale);
       // m_roomGenerator.AddValuesToGrid();
    }
    void Start()
    {
        
        //m_chunkGenerator.SpawnStartingRoom(m_desiredChunkWidth, m_desiredChunkHeight);
    }
    void GenerateNewTile()
    {

    }
    void GeneratePlatform()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }


    private enum REGION
    {
        ONE, TWO, THREE, FOUR, FIVE, SIX
    }


}

