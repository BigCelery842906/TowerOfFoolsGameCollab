//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------


using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PG_GridMap;


public class PG_GenerationManager : MonoBehaviour
{
    [SerializeField]
    public int m_desiredChunkWidth = 16;
    [SerializeField]
    public int m_desiredChunkHeight = 9;
    public int m_chunksPerRoom = 6;
    public int m_chunkSizeMultiplier = 2;
    public GameObject m_currentRoom;
    public bool m_spawnPowerups = false;
    public float m_powerupSpawnChance = 0.0f;

    public float m_worldScale = 1.5f;

    private PG_RoomGenerator m_roomGenerator;
    private PG_PlatformGenerator m_platformGenerator;
    private REGION m_currentRegion = REGION.ONE;

    [HideInInspector]
    public Action m_actionSpawnPowerups;


    private void Awake()
    {
        //this needs to be set to default values on generation otherwise the UI has a moment and sets it to zero, causing a bombardment of UI draw and OOB errors
        //m_desiredChunkHeight = 9;
        //m_desiredChunkWidth = 16;

        m_roomGenerator = GetComponent<PG_RoomGenerator>();
        if (!m_roomGenerator)
        {
            Debug.Log("Room Generator not Loaded on Generation Manager");
        }
        m_platformGenerator = GetComponent<PG_PlatformGenerator>();
        m_platformGenerator.m_genManager = this;
        if (!m_roomGenerator)
        {
            Debug.Log("Platform Generator not Loaded on Generation Manager");
        }

        m_worldScale = 1.5f;
        if (e_GlobalData.instance)
        {
            m_worldScale = e_GlobalData.instance.GetWorldScale();
        }

        RegenerateRoom();
        m_actionSpawnPowerups += SpawnPowerups;
        //m_currentRoom = m_roomGenerator.GenerateRoom(m_desiredChunkWidth, m_desiredChunkHeight, m_worldScale, m_chunksPerRoom);
        //m_currentRoom.transform.SetParent(this.transform, false);
        //m_platformGenerator.GeneratePlatforms(m_currentRoom, m_worldScale);
        //if (m_spawnPowerups) SpawnPowerups();


    }

    void PrintGrid() // debug
    {
        string room = "";
        PG_GridMap grid = m_currentRoom.GetComponent<PG_GridMap>();
        for (int x = grid.m_height - 1; x > 0; x--)
        {
            for (int y = 0; y < grid.m_width; y++)
            {

                PG_GridMap.BLOCK_TYPE type = grid.m_grid[y, x].m_blockType;
                switch (type)
                {
                    case BLOCK_TYPE.NONE:
                        room += "0";
                        break;
                    case BLOCK_TYPE.WALL:
                        room += "2";
                        break;
                    case BLOCK_TYPE.PLATFORM_MIDDLE:
                        room += "3";
                        break;
                    case BLOCK_TYPE.PLATFORM_END:
                        room += "4";
                        break;
                }
            }

            room += '\n';
        }
        Debug.Log(room);
    }

    public void SpawnPowerups()
    {
        Debug.Log("Spawning Powerups");
        PG_GridMap grid = m_currentRoom.GetComponent<PG_GridMap>();
        if(!grid)
        {
            Debug.Log("Can't find grid for powerup spawns");
        }
        
        for (int x = 0; x < grid.m_width; x++)
        {
            for (int y = 0; y < grid.m_height; y++)
            {
                bool valid = true;
                List<PG_PlatformParent> neighbours = new();
                if (grid.m_grid[x,y].m_blockType == BLOCK_TYPE.PLATFORM_MIDDLE || grid.m_grid[x, y].m_blockType == BLOCK_TYPE.PLATFORM_END)
                {
                    neighbours = grid.GetNeighboursOfPlatform(x, y);
                    foreach (PG_PlatformParent block in neighbours)
                    {
                        if(block.m_hasPowerup == true)
                        {
                            valid = false;
                            break;
                        }
                    }
                    if(valid == false)
                    {
                        continue;
                    }
                    else
                    {
                        float spawnRoll = UnityEngine.Random.Range(0, 100.0f);
                        if (spawnRoll < m_powerupSpawnChance)
                        {
                            GameObject room = transform.GetChild(0).gameObject;
                            grid.m_grid[x, y].m_contents.GetComponent<PG_PlatformParent>().SpawnPowerup(room);
                        }
                    }
                }
            }
        }
    }

    public void RegenerateRoom()
    {
        if (transform.childCount > 0)
        {
            GameObject room = transform.GetChild(0).gameObject;
            DestroyImmediate(room);
        }
        else
        {
            Debug.Log("No child objects to destroy.");
        }


        m_desiredChunkHeight = 9 * m_chunkSizeMultiplier;
        m_desiredChunkWidth = 16 * m_chunkSizeMultiplier;

        m_currentRoom = m_roomGenerator.GenerateRoom(m_desiredChunkWidth, m_desiredChunkHeight, m_worldScale, m_chunksPerRoom);
        m_currentRoom.transform.SetParent(this.transform, false);
        m_platformGenerator.GeneratePlatforms(m_currentRoom, m_worldScale);
        m_platformGenerator.m_xSpawnLocation = 1;
        m_platformGenerator.m_ySpawnLocation = 1;
        if (m_spawnPowerups) SpawnPowerups();
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



