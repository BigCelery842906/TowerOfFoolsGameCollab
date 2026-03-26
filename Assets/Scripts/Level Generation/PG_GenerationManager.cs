//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------



using UnityEngine;

using Unity.VisualScripting;


public class PG_GenerationManager : MonoBehaviour
{
    [SerializeField]
    public int m_desiredChunkWidth = 16;
    [SerializeField]
    public int m_desiredChunkHeight = 9;
    public int m_chunksPerRoom = 3;
    public int m_chunkSizeMultiplier = 1;
    public GameObject m_currentRoom;

    public float m_worldScale;

    private PG_RoomGenerator m_roomGenerator;
    private PG_PlatformGenerator m_platformGenerator;
    private REGION m_currentRegion = REGION.ONE;


    private void Awake()
    {
        //this needs to be set to default values on generation otherwise the UI has a moment and sets it to zero, causing a bombardment of UI draw and OOB errors
        m_desiredChunkHeight = 9;
        m_desiredChunkWidth = 16;

        m_roomGenerator = GetComponent<PG_RoomGenerator>();
        if (!m_roomGenerator)
        {
            Debug.Log("Room Generator not Loaded on Generation Manager");
        }
        m_platformGenerator = GetComponent<PG_PlatformGenerator>();
        if (!m_roomGenerator)
        {
            Debug.Log("Platform Generator not Loaded on Generation Manager");
        }

        m_currentRoom = m_roomGenerator.GenerateRoom(m_desiredChunkWidth, m_desiredChunkHeight, m_worldScale, m_chunksPerRoom);
        m_currentRoom.transform.SetParent(this.transform, false);
        m_platformGenerator.GeneratePlatforms(m_currentRoom, m_worldScale);


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



