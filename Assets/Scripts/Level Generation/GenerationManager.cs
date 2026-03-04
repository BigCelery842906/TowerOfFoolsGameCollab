using NUnit.Framework;

using UnityEngine;

public class GenerationManager : MonoBehaviour
{
    [SerializeField]
    public float m_desiredChunkWidth;
    [SerializeField]
    public float m_desiredChunkHeight;


    ChunkGenerator m_chunkGenerator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_chunkGenerator = GetComponent<ChunkGenerator>();
        if(!m_chunkGenerator)
        {
            Debug.Log("Chunk Generator not Loaded on Generation Manager");
        }
    }
    void Start()
    {
        m_chunkGenerator.SpawnStartingRoom();
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
}
