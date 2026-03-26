using System.Collections.Generic;
using UnityEngine;

public class c_Camera : MonoBehaviour
{

    [SerializeField] List<GameObject> m_PlayersToTrack;

    
    
    [Header("Buffers")] 
    [SerializeField]private float m_YBuffer = 10f;
    // [SerializeField]private float m_BottomBuffer = 50f;
    [SerializeField]private float m_XBuffer = 17.75f;
    // [SerializeField]private float m_RightBuffer = 50f;

    [SerializeField] private float tempDepth = 0.5f;
    
    [SerializeField] private int furthestPlayer = -1;
    [SerializeField] private float highestYPos = float.MinValue;
    [SerializeField] private float furthestXPos = float.MinValue;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        GameObject[] tempGO = GameObject.FindGameObjectsWithTag("Player");
        // This would be the capsule component of the player, so I need to get the parent for the actual physical player

        m_PlayersToTrack = new List<GameObject>();
        for(int i = 0; i < tempGO.Length; i++)
        {
            m_PlayersToTrack.Add(tempGO[i].transform.parent.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camPos = transform.position;

        Vector3 avg = returnAveragePosition();


        float totalYPosition = 0f;
       

        for (int i = 0; i < m_PlayersToTrack.Count; i++)
        {
            float Yposition = m_PlayersToTrack[i].transform.position.y;
            totalYPosition += Yposition;

            if (Yposition > highestYPos)
            {
                highestYPos = Yposition;
                furthestPlayer = i;
            }
        }

        float avgYPos = totalYPosition / m_PlayersToTrack.Count;

        if (highestYPos - avgYPos > m_YBuffer)
        {
            camPos.y = highestYPos;
        }
        else
        {
            camPos.y = avgYPos;
        }

        Debug.Log("Furthest player on Y axis is:" + furthestPlayer);

        float totalXPosition = 0f;
       

        for (int i = 0; i < m_PlayersToTrack.Count; i++)
        {
            float XPosition = m_PlayersToTrack[i].transform.position.x;
            totalXPosition += XPosition;

            if (XPosition > furthestXPos)
            {
                furthestXPos = XPosition;
                furthestPlayer = i;
            }
        }

        float avgXPos = totalXPosition / m_PlayersToTrack.Count;

        if (furthestXPos - avgXPos > m_XBuffer)
        {
            camPos.x = furthestXPos;
        }
        else
        {
            camPos.x = avgXPos;
        }

        Debug.Log("Furthest player on X axis is:" + furthestPlayer);

        transform.position = camPos;



        // float highestYPos = float.MinValue;
        //
        // for (int i = 0; i < m_PlayersToTrack.Count; i++)
        // {
        //     float yPos = m_PlayersToTrack[i].transform.position.y;
        //     if (yPos> highestYPos)
        //     {
        //         highestYPos = yPos;
        //         furthestPlayer = i;
        //     }
        // }
        //
        //
        // float deltaY = highestYPos - avg.y;
        //
        // float topEdge = avg.y + m_YBuffer;
        // Debug.Log($"DeltaY: {deltaY} | topEdge: {topEdge}");
        // if (highestYPos > topEdge)
        // {
        //     Debug.Log("Highest Player Tracking");
        //     camPos.y = highestYPos - m_YBuffer;
        // }
        // else
        // {
        //     Debug.Log("Average Player Tracking");
        //     camPos.y = avg.y;
        // }
        //
        

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
        if (m_PlayersToTrack.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 totalPosition = Vector3.zero;
        for (int i = 0; i < m_PlayersToTrack.Count; i++)
        {
            totalPosition += m_PlayersToTrack[i].transform.position;
        }
        
        // if (totalPosition.magnitude == 0)
        // {
        //     return Vector3.zero;
        // }
        totalPosition /= m_PlayersToTrack.Count;
        
        return totalPosition;
    } 
    
    
    
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 camPos = transform.position;
       
        float zOffset = tempDepth;
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            zOffset = cam.nearClipPlane;
        }
        camPos.z += zOffset;
        
        
        //Assume 16:9
        float width = m_XBuffer; /// 1920 / 0.615625f;
        float height = m_YBuffer; /// 1080 / 0.615625f;

        // To be at screen boundary in 16:9:
        // Y buffer at 10
        // X Buffer at 17.75
        
        // When half boundary
        // Y Buffer at 20
        // X Buffer at 35.5
        
        // At 1080 x 1920
        // Y = 374
        // X = 1182
        
        // At / 1920 / 0.615625f and / 1080 / 0.615625f;
        // Y = 727
        // X = 230
        
        
        Gizmos.DrawWireCube(returnAveragePosition(), new Vector3(width, height, 0f));
        
        
        
        #region Manual Line Drawing
        
        // Center of the rectangle (important!)
        // Vector3 center = new Vector3(
        //     camPos.x + (m_RightBuffer - m_XBuffer) / 2f,
        //     camPos.y + (m_YBuffer - m_BottomBuffer) / 2f,
        //     camPos.z + (0.5f)
        // );
        
        
        //
        // //Top Left = Top Buffer.y, LeftBuffer.x, 0
        // Vector3 TL = new Vector3(camPos.x + m_LeftBuffer, camPos.y + m_TopBuffer, camPos.z);
        // //Top right = Top Buffer.y, RightBuffer.x, 0
        // Vector3 TR = new Vector3(camPos.x - m_RightBuffer, camPos.y + m_TopBuffer, camPos.z);
        // //Bottom Left = BottomBuffer.y, LeftBuffer.x, 0
        // Vector3 BL = new Vector3(camPos.x + m_LeftBuffer, camPos.y - m_BottomBuffer, camPos.z);
        // //Bottom Right = BottomBuffer.y, RightBuffer.x, 0
        // Vector3 BR = new Vector3(camPos.x - m_RightBuffer, camPos.y - m_BottomBuffer, camPos.z);
        //
        // //Lines to Draw
        // // TL - TR
        // // TR - BR
        // // BR - BL
        // // BL - TL
        // Debug.DrawLine(TL, TR, Color.blue);
        // Debug.DrawLine(TR, BR, Color.blue);
        // Debug.DrawLine(BR, BL, Color.blue);
        // Debug.DrawLine(TL, BL, Color.blue);
        
        #endregion
        
    }
    
    
    
    
    
    
    
    
    
    

}




