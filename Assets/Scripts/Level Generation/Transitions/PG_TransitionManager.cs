using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

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

    public GameObject m_transitionDetector;

    private float m_currentYHeight; //top most location of top room
    public RoomGenValues m_generationValues;


    int m_lastDesignedRoomIndex;


    float m_worldScale;
    int m_roomNumber = 0;
    bool m_initComplete;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();




    }
    private void Init()
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


        //-----------------------------------------
        //     all below here is placeholder
        // this should just call generate 3 times
        //-----------------------------------------


        //m_generatorScript = m_generationManager.GetComponent<PG_GenerationManager>();
        //m_generatorScript.PopulateData(ref m_generationValues);
        int middleRoomidx = 0;
        if (m_designedRooms.Count > 0)
        {
            middleRoomidx = UnityEngine.Random.Range(0, m_designedRooms.Count);
        }
        int nextRoomEntrance = m_designedRooms[middleRoomidx].GetComponent<PG_Room>().m_entrance;
        m_generationValues._exit = nextRoomEntrance;


        //bottom
        GameObject bottomGenerator = GameObject.Instantiate(m_generationManager);
        m_bottomRoomGenerator = bottomGenerator.GetComponent<PG_GenerationManager>();
        m_bottomRoomGenerator.PopulateData(ref m_generationValues);
        Vector3 bottomPos = Vector3.zero;
        bottomGenerator.transform.position = bottomPos;
        bottomGenerator.transform.SetParent(m_bottomRoom.transform, false);
        m_worldScale = m_bottomRoomGenerator.m_worldScale;
        GameObject spawnedRoom = m_bottomRoomGenerator.RegenerateRoom();
        spawnedRoom.transform.SetParent(m_bottomRoom.transform, false);
        //m_bottomRoomGenerator.RegenerateRoom();

        PG_GridMap bottomGrid = spawnedRoom.GetComponent<PG_GridMap>();
        m_currentYHeight = bottomGrid.m_height * m_worldScale;

        int exitXPos = nextRoomEntrance;
        int exitYPos = bottomGrid.m_height - 1;
        Vector2 exitWorldPos = bottomGrid.GetWorldPosFromCell(exitXPos, exitYPos);
        exitWorldPos.y += (m_worldScale * 2);


        GameObject bottomCollider = GameObject.Instantiate(m_transitionDetector);
        bottomCollider.GetComponent<PG_DetectorTrigger>().m_triggerNextRoom += GenerateRoom;
        bottomCollider.name = "000000";
        bottomCollider.transform.rotation = Quaternion.identity;
        bottomCollider.transform.position = exitWorldPos;
        bottomCollider.transform.SetParent(m_bottomRoom.transform, false);



        //middle
        if (m_designedRooms.Count > 0)
        {
            GameObject middlePrefab = m_designedRooms[middleRoomidx];
            middlePrefab.transform.localPosition = Vector3.zero;
            GameObject middleRoom = GameObject.Instantiate(middlePrefab);
            PG_GridMap middleGrid = middleRoom.GetComponent<PG_GridMap>();
            middleRoom.transform.SetParent(m_middleRoom.transform, false);
            Vector3 middlePos = Vector3.zero;

            middlePos.y += m_currentYHeight;

            m_middleRoom.transform.position = middlePos;



            exitXPos = middleRoom.GetComponent<PG_Room>().m_exit;
            exitYPos = middleGrid.m_height - 1;
            exitWorldPos = Vector2.zero;
            exitWorldPos.x = exitXPos * m_worldScale;
            exitWorldPos.y += m_currentYHeight + m_worldScale;
            GameObject middleCollider = GameObject.Instantiate(m_transitionDetector);
            middleCollider.GetComponent<PG_DetectorTrigger>().m_triggerNextRoom += GenerateRoom;
            middleCollider.name = "000000";
            middleCollider.transform.rotation = Quaternion.identity;
            middleCollider.transform.position = exitWorldPos;
            middleCollider.transform.SetParent(m_middleRoom.transform, false);
            m_currentYHeight += m_middleRoom.GetComponentInChildren<PG_GridMap>().m_height * m_worldScale;
        }


        //top
        int previousRoomExit = m_designedRooms[middleRoomidx].GetComponent<PG_Room>().m_exit;
        m_generationValues._entrance = previousRoomExit;
        m_generationValues._exit = UnityEngine.Random.Range(1, 31);

        GameObject topGenerator = GameObject.Instantiate(m_generationManager);
        m_topRoomGenerator = topGenerator.GetComponent<PG_GenerationManager>();
        m_topRoomGenerator.PopulateData(ref m_generationValues);
        Vector3 topPos = Vector3.zero;
        topPos.y += m_currentYHeight;
        topGenerator.transform.position = topPos;
        topGenerator.transform.SetParent(m_topRoom.transform, false);
        m_worldScale = m_topRoomGenerator.m_worldScale;
        GameObject topRoom = m_topRoomGenerator.RegenerateRoom();
        PG_GridMap topGrid = topRoom.GetComponent<PG_GridMap>();
        topRoom.transform.SetParent(m_topRoom.transform, false);
        //m_topRoomGenerator.RegenerateRoom();


        m_currentYHeight += topGrid.m_height * m_worldScale;
        exitXPos = topRoom.GetComponent<PG_Room>().m_exit;
        exitYPos = topGrid.m_height - 1;
        exitWorldPos = Vector2.zero;
        exitWorldPos.x = exitXPos * m_worldScale;
        exitWorldPos.y += m_currentYHeight;
        GameObject topCollider = GameObject.Instantiate(m_transitionDetector);
        topCollider.GetComponent<PG_DetectorTrigger>().m_triggerNextRoom += GenerateRoom;
        topCollider.name = "000000";
        topCollider.transform.rotation = Quaternion.identity;
        topCollider.transform.position = exitWorldPos;
        topCollider.transform.SetParent(m_topRoom.transform, false);

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




    public void GenerateRoom()
    {
        Debug.Log("Room Collider Hit");
        GameObject room = null;
        if (m_roomNumber % 2 == 0) // designed
        {
            //find room
            room = m_designedRooms[UnityEngine.Random.Range(0, m_designedRooms.Count)];

        }
        else //proc gen
        {
            //generate room
        }

        //get world height for each room
        float bottomWorldHeight = m_bottomRoom.GetComponent<PG_GridMap>().m_height * m_worldScale;
        float middleWorldHeight = m_middleRoom.GetComponent<PG_GridMap>().m_height * m_worldScale;
        float topWorldHeight = m_topRoom.GetComponent<PG_GridMap>().m_height * m_worldScale;

        //swap room containers
        for (int i = 0; i < m_bottomRoom.transform.childCount; i++) //clear bottom room container
        {
            Destroy(m_bottomRoom.transform.GetChild(i)); 
        }
        for(int i = 0; i < m_middleRoom.transform.childCount; i++) // move middle children to bottom
        {
            m_middleRoom.transform.GetChild(i).transform.SetParent(m_bottomRoom.transform, false); 
        }
        for(int i = 0; i < m_topRoom.transform.childCount; i++)// move top to middle
        {
            m_topRoom.transform.GetChild(i).transform.SetParent(m_middleRoom.transform,false);
        }



        //move position of room containers up

        Vector3 bottomTemp = m_bottomRoom.transform.position;
        bottomTemp.y += bottomWorldHeight;
        m_bottomRoom.transform.position = bottomTemp;

        Vector3 middleTemp = m_middleRoom.transform.position;
        middleTemp.y += middleWorldHeight;
        m_middleRoom.transform.position = middleTemp;

        Vector3 topTemp = m_topRoom.transform.position;
        topTemp.y += topWorldHeight;
        m_topRoom.transform.position = topTemp;

        //create instance of room

        //move room to top transform

        //parent to new top container

        m_roomNumber++;
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
