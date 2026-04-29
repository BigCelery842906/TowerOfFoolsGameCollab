using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PG_TransitionManager : MonoBehaviour
{
    public GameObject m_bottomRoom;
    public GameObject m_middleRoom;
    public GameObject m_topRoom;

    public List<GameObject> m_designedRooms;

    public GameObject m_generationManager;
    PG_GenerationManager m_generatorScript;

    private float m_currentYHeight; //top most location of top room
    public RoomGenValues m_generationValues;
    int m_lastDesignedRoomIndex;
    float m_worldScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PopulateDefaultGenerationValues();
        m_generatorScript = m_generationManager.GetComponent<PG_GenerationManager>();
        m_generatorScript.PopulateData(ref m_generationValues);

        m_worldScale = m_generatorScript.m_worldScale;

        //GameObject bottomGenerator = GameObject.Instantiate(m_bottomRoom);
        //Vector3 bottomPos = Vector3.zero;
        //bottomGenerator.transform.position = bottomPos;
        //bottomGenerator.transform.parent

        m_bottomRoom = m_generatorScript.RegenerateRoom();
        m_currentYHeight = m_bottomRoom.GetComponent<PG_GridMap>().m_height * m_worldScale;

        GameObject middlePrefab = m_designedRooms[UnityEngine.Random.Range(0, m_designedRooms.Count)];
        m_middleRoom = GameObject.Instantiate(middlePrefab);
        Vector3 middlePos = m_middleRoom.transform.position;
        middlePos.y += m_currentYHeight;
        m_middleRoom.transform.position = middlePos;

        m_currentYHeight += m_middleRoom.GetComponent<PG_GridMap>().m_height * m_worldScale;

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
