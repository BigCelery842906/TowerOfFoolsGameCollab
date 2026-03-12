using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class c_Camera : MonoBehaviour
{

    [SerializeField] GameObject[] m_PlayersToTrack;

    
    
    [Header("Buffers")] 
    [SerializeField]private float m_TopBuffer = 50f;
    [SerializeField]private float m_BottomBuffer = 50f;
    [SerializeField]private float m_LeftBuffer = 50f;
    [SerializeField]private float m_RightBuffer = 50f;
    
    
    [SerializeField] private int furthestPlayer = -1;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        GameObject[] tempGO = GameObject.FindGameObjectsWithTag("Player");
        // This would be the capsule component of the player, so I need to get the parent for the actual physical player

        m_PlayersToTrack = new GameObject[tempGO.Length];
        for(int i = 0; i < tempGO.Length; i++)
        {
            m_PlayersToTrack[i] = tempGO[i].transform.parent.gameObject;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        float totalXPosition = 0f;
        float furthestXPosition = 0f;

        float totalYPosition = 0f;
        float highestYPosition = 0f;
        for (int i = 0; i < m_PlayersToTrack.Length; i++)
        {
            float objectXPosition = m_PlayersToTrack[i].transform.position.x;
            totalXPosition += objectXPosition;

            if (objectXPosition > furthestXPosition)
            {
                furthestXPosition = objectXPosition;
            }
            
            float objectYPosition = m_PlayersToTrack[i].transform.position.y;
            totalYPosition += objectYPosition;

            if (objectYPosition > highestYPosition)
            {
                highestYPosition = objectYPosition;
                furthestPlayer = i;
            }
        }

        Vector3 averagePosition = returnAveragePosition();

        if (furthestXPosition - averagePosition.x > m_RightBuffer)
        {
            transform.position = new Vector3(furthestXPosition - m_RightBuffer, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(averagePosition.x, transform.position.y, transform.position.z);
        }

        if (highestYPosition - averagePosition.y > m_TopBuffer)
        {
            transform.position = new Vector3(transform.position.x,  highestYPosition - m_TopBuffer, transform.position.z );
        }
        else
        {
            transform.position = new Vector3(transform.position.x, averagePosition.y, transform.position.z);
        }
    }

// float totalXPosition = 0f;
//
// for (int i = 0; i < turtleList.Count; i++)
// {
//     turtleList[i].isInFirst(false);
//     float turtleXposition = turtleList[i].transform.position.x;
//     totalXPosition += turtleXposition;
//
//     if (turtleXposition > furthestXPosition)
//     {
//         furthestXPosition = turtleXposition;
//         furthestTurtle = i;
//     }
// }
//
// float averageXPosition = totalXPosition / turtleList.Count;
//
// if (furthestXPosition - averageXPosition > cameraBufferDistance)
// {
//     camera.transform.position = new Vector3(furthestXPosition - cameraBufferDistance, camera.transform.position.y, camera.transform.position.z);
// }
// else
// {
//     camera.transform.position = new Vector3(averageXPosition, camera.transform.position.y, camera.transform.position.z);
// }
//
// }

    Vector3 returnAveragePosition()
    {
        if (m_PlayersToTrack.Length == 0)
        {
            return Vector3.zero;
        }

        Vector3 totalPosition = Vector3.zero;
        for (int i = 0; i < m_PlayersToTrack.Length; i++)
        {
            totalPosition += m_PlayersToTrack[i].transform.position;
        }
        
        if (totalPosition.magnitude == 0)
        {
            return Vector3.zero;
        }
        totalPosition /= m_PlayersToTrack.Length;
        
        return totalPosition;
    } 
    
    
    
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube();
    }

    
    
    
    
    
    
    
    
    
    

}




