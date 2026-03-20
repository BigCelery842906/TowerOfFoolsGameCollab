//-------------------------------------
//      Property of Dan.
//      Break it and you suffer.
//      Respectfully of course...
//-------------------------------------


using UnityEngine;

public class PG_BackGround : MonoBehaviour
{
    public float m_width = 0;
    public float m_height = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ResizeMesh(float w, float h)
    {
        m_width = w;
        m_height = h;
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = new();

        Vector3[] verts = new Vector3[4]
        {
            new Vector3(- m_width * 0.5f, - m_height * 0.5f, 0), //bottom left
            new Vector3(m_width * 0.5f, - m_height * 0.5f, 0), // bottom right
            new Vector3(- m_width * 0.5f, m_height * 0.5f, 0), //top left
            new Vector3(m_width * 0.5f, m_height * 0.5f, 0) // top right
        };
        mesh.vertices = verts;

        int[] ind = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = ind;

        mesh.RecalculateNormals();

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;
        meshFilter.mesh = mesh;
    }
}
