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
    public int m_minimumPowerups;
    public float m_powerupSpawnChance = 0.0f;

    public float m_worldScale = 1.5f;

    private PG_RoomGenerator m_roomGenerator;
    private PG_PlatformGenerator m_platformGenerator;
    private REGION m_currentRegion = REGION.ONE;
    private List<GameObject> m_spawnedBonusPlatforms;
    private bool m_powerupsSpawned = false;

    [HideInInspector]
    public Action m_actionSpawnPowerups;


    private void Awake()
    {
        //this needs to be set to default values on generation otherwise the UI has a moment and sets it to zero, causing a bombardment of UI draw and OOB errors
        //m_desiredChunkHeight = 9;
        //m_desiredChunkWidth = 16;
        m_spawnedBonusPlatforms = new();
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

        m_actionSpawnPowerups += SpawnPowerups;
        RegenerateRoom();
        m_spawnedBonusPlatforms = m_platformGenerator.GetBonusPlatforms();
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
        if (m_powerupsSpawned) return;
        m_powerupsSpawned = true;
        if (m_minimumPowerups > m_spawnedBonusPlatforms.Count) m_minimumPowerups = m_spawnedBonusPlatforms.Count;
        int powerupsSpawned = 0;
        int failedSpawns = 0;
        while (powerupsSpawned < m_minimumPowerups && failedSpawns < 50)
        {
            bool alreadySpawned = true;
            PG_PlatformContainer platformContainerScript = null;
            GameObject platformContainer = null;
            while (alreadySpawned && failedSpawns < 50)
            {
                int platformToSpawn = UnityEngine.Random.Range(0, m_spawnedBonusPlatforms.Count);
                platformContainer = m_spawnedBonusPlatforms[platformToSpawn];
                platformContainerScript = platformContainer.GetComponent<PG_PlatformContainer>();
                alreadySpawned = platformContainerScript.m_powerupSpawned;
                if (alreadySpawned) failedSpawns++;
            }
            if (alreadySpawned) break;
            //float spawnChance = UnityEngine.Random.Range(0, m_powerupSpawnChance);
            int blockToSpawnOn = UnityEngine.Random.Range(0, platformContainer.transform.childCount);
            platformContainerScript.m_powerupSpawned = true;
            if (platformContainer.transform.GetChild(blockToSpawnOn).GetComponent<PG_PlatformParent>().SpawnPowerup(m_currentRoom))
            {

                powerupsSpawned++;

            }
            else platformContainerScript.m_powerupSpawned = false;
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

        m_powerupsSpawned = false;

        m_platformGenerator.ClearLists();
        m_currentRoom = m_roomGenerator.GenerateRoom(m_desiredChunkWidth, m_desiredChunkHeight, m_worldScale, m_chunksPerRoom);
        m_currentRoom.transform.SetParent(this.transform, false);
        m_platformGenerator.GeneratePlatforms(m_currentRoom, m_worldScale);
        m_platformGenerator.m_xSpawnLocation = 1;
        m_platformGenerator.m_ySpawnLocation = 1;

    }


    // Update is called once per frame
    void Update()
    {
        if (m_platformGenerator.m_generationFinished && !m_powerupsSpawned)
        {
            SpawnPowerups();
            m_powerupsSpawned = true;
        }
    }


    private enum REGION
    {
        ONE, TWO, THREE, FOUR, FIVE, SIX
    }


}



