using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PG_TransitionManager : MonoBehaviour
{
    public GameObject m_bottomRoom;
    PG_GenerationManager m_bottomRoomGenerator;
    public GameObject m_middleRoom;
    PG_GenerationManager m_middleRoomGenerator;
    public GameObject m_topRoom;
    PG_GenerationManager m_topRoomGenerator;

    public List<GameObject> m_designedRooms;

    public GameObject m_generationManager;
    PG_GenerationManager m_generatorScript;

    private float m_currentYHeight; //top most location of top room
    public RoomGenValues m_generationValues;
    int m_lastDesignedRoomIndex;
    float m_worldScale;
    int m_roomNumber = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_bottomRoom = new GameObject();
        m_bottomRoom.name = "Bottom";
        m_bottomRoom.transform.SetParent(this.transform);
        m_middleRoom = new GameObject();
        m_middleRoom.name = "Middle";
        m_middleRoom.transform.SetParent(this.transform);
        m_topRoom = new GameObject();
        m_topRoom.name = "Top";
        m_topRoom.transform.SetParent(this.transform);


        PopulateDefaultGenerationValues();
        //m_generatorScript = m_generationManager.GetComponent<PG_GenerationManager>();
        //m_generatorScript.PopulateData(ref m_generationValues);



        GameObject bottomGenerator = GameObject.Instantiate(m_generationManager);
        m_bottomRoomGenerator = bottomGenerator.GetComponent<PG_GenerationManager>();
        Vector3 bottomPos = Vector3.zero;
        bottomGenerator.transform.position = bottomPos;
        bottomGenerator.transform.SetParent(m_bottomRoom.transform, false);
        m_worldScale = m_bottomRoomGenerator.m_worldScale;
        GameObject spawnedRoom = m_bottomRoomGenerator.RegenerateRoom();
       spawnedRoom.transform.SetParent(m_bottomRoom.transform, false);
        //m_bottomRoomGenerator.RegenerateRoom();

        PG_GridMap bottomGrid = spawnedRoom.GetComponent<PG_GridMap>();
        m_currentYHeight = bottomGrid.m_height * m_worldScale;

        if (m_designedRooms.Count > 0)
        {
            GameObject middlePrefab = m_designedRooms[UnityEngine.Random.Range(0, m_designedRooms.Count)];
            GameObject middleRoom = GameObject.Instantiate(middlePrefab);
            middleRoom.transform.SetParent(m_middleRoom.transform, false);
            Vector3 middlePos = Vector3.zero;

            middlePos.y += m_currentYHeight;
            
            m_middleRoom.transform.position = middlePos;

            m_currentYHeight += m_middleRoom.GetComponentInChildren<PG_GridMap>().m_height * m_worldScale;
        }
        GameObject topGenerator = GameObject.Instantiate(m_generationManager);
        m_topRoomGenerator = topGenerator.GetComponent<PG_GenerationManager>();
        Vector3 topPos = Vector3.zero;
        topPos.y += m_currentYHeight;
        topGenerator.transform.position = topPos;
        topGenerator.transform.SetParent(m_topRoom.transform, false);
        m_worldScale = m_topRoomGenerator.m_worldScale;
        GameObject topRoom = m_topRoomGenerator.RegenerateRoom();
        topRoom.transform.SetParent(m_topRoom.transform, false);
        //m_topRoomGenerator.RegenerateRoom();

        PG_GridMap topGrid = topRoom.GetComponent<PG_GridMap>();
        m_currentYHeight += topGrid.m_height * m_worldScale;

    }

    void PopulateDefaultGenerationValues()
    {
        m_generationValues = new();
        m_generationValues._spawnPowerups = true;

        m_generationValues._minPowerups = 10;
        m_generationValues._criticalPlatformXVariation = 5;
        m_generationValues._criticalPlatformSize = 5;
        m_generationValues._fixedBonusPlatformSize = false;
        m_generationValues._minBonusPlatformSize = 2;
        m_generationValues._maxBonusPlatformSize = 5;
        m_generationValues._bonusPlatformSize = 5;
        m_generationValues._bonusPlatformXSeparation = 2;
        m_generationValues._bonusPlatformSpawnAttempts = 50;
        m_generationValues._bonusPlatformNumber = 20;

        m_generationValues._entrance = -1;
        m_generationValues._exit = UnityEngine.Random.Range(1, 31); //dont judge me for the magic number
    }

    private void GenerateRoom()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public struct RoomGenValues
    {
        public bool _spawnPowerups;
        public int _minPowerups;

        public int _criticalPlatformXVariation;
        public int _criticalPlatformSize;

        public int _bonusPlatformNumber;
        public bool _fixedBonusPlatformSize;
        public int _minBonusPlatformSize;
        public int _maxBonusPlatformSize;
        public int _bonusPlatformSize;
        public int _bonusPlatformXSeparation;

        public int _bonusPlatformSpawnAttempts;

        public int _entrance;
        public int _exit;

    }
}
