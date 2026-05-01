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

    private GameObject bottomCollider;
    private GameObject middleCollider;

    public GameObject m_newRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();




    }
    public void Init()
    {
        if (e_GlobalData.instance)
        {
            m_worldScale = e_GlobalData.instance.GetWorldScale();
        }

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

        bottomCollider = SpawnCollider();
        middleCollider = SpawnCollider();
        
        MoveColliderToNewPosition(m_newRoom);

        //m_generatorScript = m_generationManager.GetComponent<PG_GenerationManager>();
        //m_generatorScript.PopulateData(ref m_generationValues);



    }
    
    // //bottom
    // GameObject bottomGenerator = GameObject.Instantiate(m_generationManager);
    // m_bottomRoomGenerator = bottomGenerator.GetComponent<PG_GenerationManager>();
    // m_bottomRoomGenerator.PopulateData(ref m_generationValues);
    // Vector3 bottomPos = Vector3.zero;
    // bottomGenerator.transform.position = bottomPos;
    // bottomGenerator.transform.SetParent(m_bottomRoom.transform, false);
    // // m_worldScale = m_bottomRoomGenerator.m_worldScale;
    // GameObject spawnedRoom = m_bottomRoomGenerator.RegenerateRoom();
    // spawnedRoom.transform.SetParent(m_bottomRoom.transform, false);
    // //m_bottomRoomGenerator.RegenerateRoom();
    //
    // PG_GridMap bottomGrid = spawnedRoom.GetComponent<PG_GridMap>();
    // m_currentYHeight = bottomGrid.m_height * m_worldScale;
    //
    // int exitXPos = nextRoomEntrance;
    // int exitYPos = bottomGrid.m_height - 1;
    // Vector2 exitWorldPos = bottomGrid.GetWorldPosFromCell(exitXPos, exitYPos);
    // exitWorldPos.y += (m_worldScale * 2);
    //
    //
    // GameObject bottomCollider = GameObject.Instantiate(m_transitionDetector);
    // bottomCollider.GetComponent<PG_DetectorTrigger>().m_triggerNextRoom += GenerateRoom;
    // bottomCollider.name = "000000";
    // bottomCollider.transform.rotation = Quaternion.identity;
    // bottomCollider.transform.position = exitWorldPos;
    // bottomCollider.transform.SetParent(m_bottomRoom.transform, false);

    GameObject SpawnCollider()
    {
        GameObject colliderObject = GameObject.Instantiate(m_transitionDetector, this.gameObject.transform);
        colliderObject.GetComponent<PG_DetectorTrigger>().m_triggerNextRoom += GenerateRoom;
        colliderObject.name = "000000 - Collider";
        colliderObject.transform.rotation = Quaternion.identity;
        colliderObject.transform.localScale = new Vector3(m_worldScale, m_worldScale, m_worldScale);
        return colliderObject;
    }

    void MoveColliderToNewPosition(GameObject newRoom)
    {
        GameObject collider = NextCollider();
        Debug.Log(collider, collider);

        Vector2 newPosition = new Vector2();
        
        
        // newRoom.GetComponent<PG_GridMap>().GetWorldPosFromCell(newRoom.GetComponent<PG_GridMap>().m_yHeight)
        
        //Get to the PG_Room component somehow
        
        int exit = newRoom.GetComponent<PG_Room>().m_exit;
        int entrance = newRoom.GetComponent<PG_Room>().m_entrance;

        int heightOfRoom = newRoom.GetComponent<PG_GridMap>().m_height;
        
        Debug.Log("Exit: " + exit + ", height: " + heightOfRoom);
        
        // Vector2 xPos1 = newRoom.GetComponent<PG_GridMap>().CalculateWorldPositionFromCoords(exit - 1, heightOfRoom - 1);
        // Vector2 xPos2 = newRoom.GetComponent<PG_GridMap>().CalculateWorldPositionFromCoords(exit + 1, heightOfRoom - 1);

        // Vector2 newPos = new Vector2((xPos1.x + xPos2.x) / 2, xPos1.y);
        
        Vector2 newPos = new Vector2(exit, heightOfRoom - 1);
        Debug.Log("Exit: " + exit + ", height: " + heightOfRoom + ", Position: " + newPos);
        
        Vector3 newPositionLocal = newPos * m_worldScale;
        collider.transform.position = newRoom.transform.position + newPositionLocal;
    }

    GameObject NextCollider()
    {
        if (m_roomNumber % 2 == 0)
        {
            return bottomCollider;
        }
        else
        {
            return middleCollider;
        }
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
