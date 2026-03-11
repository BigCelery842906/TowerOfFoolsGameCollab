using NUnit.Framework;

using UnityEngine;

public class PG_GenerationManager : MonoBehaviour
{
    [SerializeField]
    public int m_desiredChunkWidth;
    [SerializeField]
    public int m_desiredChunkHeight;

    public int m_startRoomWidth;
    public int m_startRoomHeight;

    public float m_worldScale;

    private PG_ChunkGenerator m_chunkGenerator;
    private REGION m_currentRegion = REGION.ONE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

    }
    void Start()
    {
        m_chunkGenerator = GetComponent<PG_ChunkGenerator>();
        if (!m_chunkGenerator)
        {
            Debug.Log("Chunk Generator not Loaded on Generation Manager");
        }
        m_chunkGenerator.SetWorldScale(m_worldScale);
        m_chunkGenerator.SpawnStartingRoom(m_startRoomWidth, m_startRoomHeight);
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
