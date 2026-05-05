using System.Collections;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class PG_TransitionManager : MonoBehaviour
{
    public static PG_TransitionManager instance;
    [SerializeField] public GameObject m_previousRoom;
    [SerializeField] PG_GenerationManager m_previousRoomGenerator;
    [SerializeField] public GameObject m_currentRoom;
    [SerializeField] PG_GenerationManager m_currentRoomGenerator;
    [SerializeField] public GameObject m_nextRoom;
    [SerializeField] PG_GenerationManager m_nextRoomGenerator;

    public List<GameObject> m_designedRooms;

    public GameObject m_generationManager;

    public GameObject m_transitionDetector;

    private float m_currentYHeight; //top most location of top room
    public RoomGenValues m_generationValues;


    int m_lastDesignedRoomIndex;

    private int m_lastExitIndex = -1;

    float m_worldScale;
    int m_roomNumber = 1;
    bool m_initComplete = false;

    private GameObject colliderNum1;
    // private GameObject colliderNum2;

    public GameObject m_newRoom;

    public int designRoomID = 0;
    private int nextDesignRoomEntrance = -1;
    private bool m_isLastRoomDesign = false;
    private bool isNextDesign = false;

    [SerializeField] int chanceOfProcGen = 5;
    [SerializeField] int chanceOfDesign = 1;

    bool isSpawning = false;
    private bool useFirstCollider = true;


    //Logic Order
    // For initialisation - COMPLETE
    //Spawn Room 1 with entrance at -1
    //Spawn Room 2 with entrance at exit of room 1
    //Spawn Room 3 with entrance at exit of room 2
    //Spawn Collider 1 - WORKS
    //Spawn Collider 2 - WORKS
    //Set Collider 1 pos to exit of room 2
    //Set Collider 2 pos to exit of room 3
    //Boot into normal logic for triggering

    //When Lower Collider triggered, delete the lowest room, spawn another room at exit of highest room
    //Move lower collider to exit of new highest room


    //For room spawning logic - Current Working on
    //Decide if design room or proc gen
    //Spawn Room
    //Set collider position

    //If proc gen
    // Check if next room is design or not.
    //If next room is design, get the entrance of this room
    //Set the next entrance as the current exit
    //Load parameters for spawning
    //Generate room
    //Get position it should spawn at
    //Set position

    //If design room
    //Select random from list
    //Get position it should spawn at
    //Set position
    //

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        m_currentYHeight = transform.position.y;

        //Do we need to spawn previous straight away? I'm thinking spawn current and next, then spawn another when current exit reached, which will then populate previous
        m_previousRoom = new GameObject("Previous Room");
        m_previousRoom.transform.SetParent(this.transform);
        m_currentRoom = new GameObject("Current Room");
        m_currentRoom.transform.SetParent(this.transform);
        m_nextRoom = new GameObject("Next Room");
        m_nextRoom.transform.SetParent(this.transform);


        PopulateDefaultGenerationValues();


        //-----------------------------------------
        //     all below here is placeholder
        // this should just call generate 3 times
        //-----------------------------------------

        colliderNum1 = SpawnCollider(1);
        // colliderNum2 = SpawnCollider(2);
        RollRandomDesignRoom();
        StartCoroutine(InitCoRoutine());

        // SpawnRooms(false, false);
        // SpawnRooms(true, false);
        // SpawnRooms(true, false);
        // SpawnRooms(true, false);
        // SpawnRooms(true, false);
        // SpawnRooms(true, false);

        //SpawnRooms(); //Not Needed yet, use for testing

        //m_generatorScript = m_generationManager.GetComponent<PG_GenerationManager>();
        //m_generatorScript.PopulateData(ref m_generationValues);



    }

    IEnumerator InitCoRoutine()
    {
        yield return SpawnRooms(true, false);
        yield return SpawnRooms(true, false);
        yield return SpawnRooms(true, false);
        yield return null;
        MoveColliderToNewPosition(m_nextRoom);
        m_initComplete = true;
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

    IEnumerator SpawnRooms(bool isProcGen = true, bool isNextRoomDesign = false)
    {

        MoveRoomObjects();
        yield return null;

        if (isProcGen)
        {
            if (isNextRoomDesign)
            {
                SpawnProcGenRoom(m_lastExitIndex, nextDesignRoomEntrance);
            }
            else
            {
                SpawnProcGenRoom(m_lastExitIndex);
            }
        }
        else
        {
            SpawnDesignRoom();
        }

        m_roomNumber++;

        yield return null;

        if (m_initComplete) MoveColliderToNewPosition(m_nextRoom);

        isSpawning = false;

    }

    void SpawnProcGenRoom(int entrancePosition = -1, int exitPosition = -1)
    {
        GameObject nextGenerator = GameObject.Instantiate(m_generationManager);
        m_nextRoomGenerator = nextGenerator.GetComponent<PG_GenerationManager>();
        m_nextRoomGenerator.PopulateData(ref m_generationValues);
        if (exitPosition != -1)
        {
            m_nextRoomGenerator.GetRoomGenerator().m_nextRoomEntrance = exitPosition;
        }
        else
        {
            exitPosition = m_nextRoomGenerator.GetRoomGenerator().m_nextRoomEntrance;
        }

        if (entrancePosition != -1)
        {
            m_nextRoomGenerator.GetRoomGenerator().m_previousRoomExit = entrancePosition;
        }
        else
        {
            entrancePosition = m_nextRoomGenerator.GetRoomGenerator().m_previousRoomExit;
        }

        Vector3 position = Vector3.zero; //Pivot is on bottom left for the room so this works
        position.y += m_currentYHeight;
        nextGenerator.transform.position = position;
        nextGenerator.transform.SetParent(m_nextRoom.transform, false);

        GameObject spawnedRoom = m_nextRoomGenerator.RegenerateRoom();

        spawnedRoom.transform.SetParent(m_nextRoom.transform, false);


        PG_GridMap grid = spawnedRoom.GetComponent<PG_GridMap>();
        m_currentYHeight += (grid.m_height * m_worldScale) - m_worldScale;

        m_lastExitIndex = exitPosition;
    }

    void SpawnDesignRoom()
    {
        if (m_designedRooms.Count > 0)
        {
            GameObject designRoom = GameObject.Instantiate(m_designedRooms[designRoomID]);
            PG_GridMap grid = designRoom.GetComponentInChildren<PG_GridMap>();

            Vector3 position = Vector3.zero;
            position.y += m_currentYHeight;
            designRoom.transform.position = position;

            designRoom.transform.SetParent(m_nextRoom.transform, false);

            int exitXPos = designRoom.GetComponent<PG_Room>().m_exit;
            int exitYPos = grid.m_height - 1;

            m_currentYHeight += (grid.m_height * m_worldScale) - m_worldScale;

            m_lastExitIndex = exitXPos;
        }
    }

    void GetNextDesignEntrance()
    {
        GameObject nextDesignRoom = m_designedRooms[designRoomID];
        nextDesignRoomEntrance = nextDesignRoom.GetComponent<PG_Room>().m_entrance;
    }

    #region Collider Logic

    GameObject SpawnCollider(int num)
    {
        GameObject colliderObject = GameObject.Instantiate(m_transitionDetector, this.gameObject.transform);
        // colliderObject.GetComponent<PG_DetectorTrigger>().m_triggerNextRoom += GenerateRoom;
        colliderObject.name = "00000" + num + " - Collider";
        colliderObject.transform.rotation = Quaternion.identity;
        colliderObject.transform.localScale = new Vector3(m_worldScale, m_worldScale, m_worldScale);
        return colliderObject;
    }

    //void MoveColliderToNewPosition(GameObject newRoom)
    //{
    //    GameObject collider = colliderNum1;
    //    Debug.Log(collider, collider);

    //    Vector2 newPosition = new Vector2();

    //    //Get to the PG_Room component somehow
    //    //Hierarchy is as follows
    //    //newRoom object
    //    //Children are: PG_GenerationManager and Room
    //    //Room holds the component I want

    //    GameObject RoomObjectInChild = newRoom.GetComponentInChildren<PG_Room>().gameObject;

    //    PG_Room curPG_Room = RoomObjectInChild.GetComponent<PG_Room>();
    //    PG_GridMap curPG_GridMap = RoomObjectInChild.GetComponent<PG_GridMap>();

    //    int exit = curPG_Room.m_exit;
    //    int entrance = curPG_Room.m_entrance;

    //    int heightOfRoom = curPG_GridMap.m_height;

    //    Debug.Log("Exit: " + exit + ", height: " + heightOfRoom);

    //    // Vector2 xPos1 = newRoom.GetComponent<PG_GridMap>().GetWorldPosFromCell(exit - 1, heightOfRoom - 1);
    //    // Vector2 xPos2 = newRoom.GetComponent<PG_GridMap>().GetWorldPosFromCell(exit + 1, heightOfRoom - 1);
    //    //  
    //    // Vector2 newPos = new Vector2((xPos1.x + xPos2.x) / 2, xPos1.y);


    //    Debug.Log("Exit:" + exit);
    //    Vector2 left = curPG_GridMap.GetWorldPosFromCell(exit - 1, heightOfRoom - 1);
    //    Vector2 right = curPG_GridMap.GetWorldPosFromCell(exit + 1, heightOfRoom - 1);

    //    Debug.Log("Room world pos: " + RoomObjectInChild.transform.position);
    //    Debug.Log("Left world: " + left + " Right world: " + right);


    //    Vector2 newPositionWorld = Vector2.zero;

    //    if (left == Vector2.zero)
    //    {
    //        newPositionWorld = new Vector2( right.x - m_worldScale, right.y);
    //    }
    //    else if (right == Vector2.zero)
    //    {
    //        newPositionWorld = new Vector2( left.x + m_worldScale, left.y);
    //    }
    //    else
    //    {
    //        newPositionWorld = new Vector2((left.x + right.x) / 2f, left.y);
    //    }

    //    Vector2 newPos = new Vector2(exit, heightOfRoom - 1);
    //    Debug.Log("Exit: " + exit + ", height: " + heightOfRoom + ", Position: " + newPos);

    //    Vector3 newPositionLocal = newPos * m_worldScale;
    //    // Vector3 newPositionWorld = m_nextRoomGenerator.transform.position + newPositionLocal;
    //    // Vector3 newPositionWorld = RoomObjectInChild.transform.position + newPositionLocal;
    //    // m_currentYHeight = newPositionWorld.y;
    //    collider.transform.position = newPositionWorld;
    //}
    void MoveColliderToNewPosition(GameObject newRoom)
    {
        PG_Room roomScript = newRoom.GetComponentInChildren<PG_Room>();
        PG_GridMap roomGrid = null;

        //find the actual valid gridmap
        //getcomponentinchildren gets the dummy gridmap from the generator
        for (int i = 0; i < newRoom.transform.childCount; i++)
        {
            PG_GridMap tempGrid = newRoom.transform.GetChild(i).GetComponent<PG_GridMap>();
            if (tempGrid.m_height > 0)
            {
                roomGrid = tempGrid;
                break;
            }
        }
        if(roomGrid == null)
        {
            Debug.Log(newRoom.name + " Cannot find gridmap");
            return;
        }

        Vector3 colliderPos = Vector3.zero; //init to zero

        //we track currentyheight already, so just subtract the height of the next room
        colliderPos.y = m_currentYHeight - ((roomGrid.m_height - 1) * m_worldScale); 
        colliderPos.x += roomScript.m_entrance * m_worldScale;
        colliderNum1.transform.position = colliderPos;

    }


    // GameObject NextCollider()
    // {
    //     GameObject result = useFirstCollider ? colliderNum1 : colliderNum2;
    //     useFirstCollider = !useFirstCollider;
    //     return result;
    // }

    #endregion

    #region Room Move Logic
    void MoveRoomObjects()
    {
        //Logic order
        // Can safely discard the previous room - Destroy children of object
        //Move this empty gameobject to a temporary reference which is used until both the other rooms have been moved
        //'Move' current room to be previous
        //Rename object to be 'previous'
        //'Move' next room to be current
        //Rename object to be 'current'
        //Move the old previous (currently in temp ref), and set that as the next
        //Rename object to be 'next'
        //Spawn new room as next - Done in gen script

        KillChildrenOfObject(m_previousRoom);

        GameObject tempRef = m_previousRoom;

        m_previousRoom = m_currentRoom;
        m_previousRoom.name = "Previous Room";
        m_previousRoomGenerator = m_currentRoomGenerator;

        m_currentRoom = m_nextRoom;
        m_currentRoom.name = "Current Room";
        m_currentRoomGenerator = m_nextRoomGenerator;

        m_nextRoom = tempRef;
        m_nextRoom.name = "Next Room";
        if (m_nextRoomGenerator != null)
        {
            Destroy(m_nextRoomGenerator.gameObject);
            m_nextRoomGenerator = null;
        }

        Debug.Log("Children in NEXT: " + m_nextRoom.transform.childCount + " Children in CURRENT: " + m_currentRoom.transform.childCount + " Children in PREVIOUS: " + m_previousRoom.transform.childCount);

    }

    void KillChildrenOfObject(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

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
        m_generationValues._exit = UnityEngine.Random.Range(2, 30); //dont judge me for the magic number
    }




    public void GenerateNextRoom()
    {
        if (isSpawning) return;

        isSpawning = true;
        Debug.Log("Room Collider Hit");

        bool isCurProcGen = true;
        if (isNextDesign) //Next room is now current
        {
            isCurProcGen = false;
            isNextDesign = false;
        }
        if (isCurProcGen)
        {
            int nextRoomRandom = Random.Range(0, chanceOfDesign + chanceOfProcGen);
            if (nextRoomRandom < chanceOfDesign) // designed
            {
                Debug.Log("Rolling random design room");
                isNextDesign = true;
                RollRandomDesignRoom();
            }
            else
            {
                isNextDesign = false;
            }
        }


        StartCoroutine(SpawnRooms(isCurProcGen, isNextDesign));
    }

    void RollRandomDesignRoom()
    {
        designRoomID = Random.Range(0, m_designedRooms.Count);
        GetNextDesignEntrance();

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
