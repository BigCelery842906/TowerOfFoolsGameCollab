using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class c_Camera : MonoBehaviour
{
    
    // Camera Plan for Script
    //     Average Position - TODO: DONE
    //          If all active players are within the average boundary, then track average position
    //
    //          If a player is outside the boundary, then zoom the camera out to put both players
    //          inside the boundary, up to a specified value.
    //
    //     Zoomed and tracking Furthest Player - TODO: Need to Implement the lava component x distance below the camera.
    //          If the specified value is reached, then prioritise the top player.
    //          Keep this top player within the top buffer of the camera, so they can always see
    //          the next platform they need to jump to.
    //
    //          If the bottom player falls too far out of camera, have a lava object below it (0 speed)
    //          - Can set this dynamically based on the max zoom value
    //
    //     Zoomed and returning to average - TODO: All of this part
    //          If zoomed out, but then only one player becomes active,
    //          lerp to being zoomed in on the average position

    
    
    [SerializeField] List<GameObject> m_PlayersToTrack;
    [SerializeField] List<GameObject> m_ActivePlayers;
    [SerializeField] private bool trackingAverage = true;
    
    [Header("Buffers")] 
    [Range(0f,0.5f)][SerializeField]private float m_YBuffer = 0.2f;
    //Might want a bottom buffer later on
    [Range(0f,0.5f)][SerializeField]private float m_XBuffer = 0.15f;

    [SerializeField] private float scaledYbuffer = 10f; // The buffers are calibrated with 5 zoom
    [SerializeField] private float scaledXbuffer = 17.75f; // The buffers are calibrated with 5 zoom

    [SerializeField] private float tempDepth = 0.3f;
    
    [SerializeField] private int furthestPlayer = -1;
    [SerializeField] private float highestYPos = float.MinValue;
    [SerializeField] private float furthestXPos = float.MinValue;

    
    [Header("Camera Values")]
    private Camera _camera;
    [SerializeField] private float cameraZoom = 5f;
    [SerializeField] private float minCameraZoom = 5f;
    [SerializeField] private float maxCameraZoom = 30f;
    [SerializeField] private float desiredCameraZoom = 5f;
    //Might need a float for desired X and Y zoom, then take max from that.
    [SerializeField] private float camHeight;
    [SerializeField] private float camWidth;


    [Header("Dead Zone Values")] 
    [SerializeField] private float deadZoneHeight = 6.0f;
    [SerializeField] private float deadZoneWidth = 12.45f;
    [SerializeField] private float xBufferWorld = 2.67f;
    [SerializeField] private float yBufferWorld = 2.0f;
    
    [SerializeField] private float yBufferScale = 2.5f;
    private Bounds playerBounds;
    
    
    
    
    
    
    enum CameraStates
    {
        trackingHighest,
        trackingAverage,
        transitioning,
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = GetComponent<Camera>();

        GameObject[] tempGO = GameObject.FindGameObjectsWithTag("Player");
        // This would be the capsule component of the player, so I need to get the parent for the actual physical player

        m_PlayersToTrack = new List<GameObject>();
        for(int i = 0; i < tempGO.Length; i++)
        {
            m_PlayersToTrack.Add(tempGO[i].transform.parent.gameObject);
        }
        
        m_ActivePlayers.Clear();
        for (int i = 0; i < m_PlayersToTrack.Count; i++)
        {
            if (m_PlayersToTrack[i].activeSelf)
            {
                m_ActivePlayers.Add(m_PlayersToTrack[i]);
            }
        }
        
        playerBounds = new Bounds(m_ActivePlayers[0].transform.position, Vector3.zero);

        
    }

    // Update is called once per frame
    void Update()
    {
        
        m_ActivePlayers.Clear();
        for (int i = 0; i < m_PlayersToTrack.Count; i++)
        {
            if (m_PlayersToTrack[i].activeSelf)
            {
                m_ActivePlayers.Add(m_PlayersToTrack[i]);
            }
        }

        //Reset the player bounds each frame
        playerBounds = new Bounds(m_ActivePlayers[0].transform.position, Vector3.zero);
        for (int i = 0; i < m_ActivePlayers.Count; i++)
        {
            playerBounds.Encapsulate(m_ActivePlayers[i].transform.position);
        }

        camHeight = _camera.orthographicSize * 2;
        camWidth = camHeight * _camera.aspect;

        #region playerBounds MinMax - Doesn't work 100% of time
        //Doesn't work due to sometimes player 1 is not the min, instead player 2 is the min, which means it doesn't work properly
        // if (m_ActivePlayers.Count == 2)
        // {
        //     playerBounds.SetMinMax(m_ActivePlayers[0].transform.position, m_ActivePlayers[1].transform.position);
        // }
        // else if (m_ActivePlayers.Count == 1)
        // {
        //     playerBounds.SetMinMax(m_ActivePlayers[0].transform.position, m_ActivePlayers[0].transform.position);
        // }
        // else if (m_ActivePlayers.Count == 0)
        // {
        //     Debug.Log("No active players");
        // }
        #endregion
        yBufferWorld = camHeight * m_YBuffer;
        xBufferWorld = camWidth * m_XBuffer;
        
        deadZoneHeight = camHeight - (yBufferWorld * 2f);
        deadZoneWidth = camWidth - (xBufferWorld * 2f);

        if (desiredCameraZoom < maxCameraZoom)
        {
            CalculateCameraPosition();
            trackingAverage = true;
        }
        else
        {
            FollowHighestPlayer();
            trackingAverage = false;
        }
        
        CalculateCameraZoom();

        // Vector3 camPos = transform.position;
        //
        // Vector3 avg = returnAveragePosition();
        //
        //
        // highestYPos = float.MinValue;
        //
        // for (int i = 0; i < m_ActivePlayers.Count; i++)
        // {
        //     float Yposition = m_ActivePlayers[i].transform.position.y;
        //
        //     if (Yposition > highestYPos)
        //     {
        //         highestYPos = Yposition;
        //         furthestPlayer = i;
        //     }
        // }
        //
        // float avgYPos = avg.y;
        //
        // trackingAverage = true;
        // camPos.y = avgYPos;
        //
        //
        // Debug.Log("Furthest player on Y axis is:" + furthestPlayer);
        //
        // furthestXPos = float.MinValue;
        //
        //
        // for (int i = 0; i < m_ActivePlayers.Count; i++)
        // {
        //     float XPosition = m_ActivePlayers[i].transform.position.x;
        //
        //     if (XPosition > furthestXPos)
        //     {
        //         furthestXPos = XPosition;
        //         furthestPlayer = i;
        //     }
        // }
        //
        //
        // float avgXPos = avg.x;
        // camPos.x = avgXPos;
        //
        // Debug.Log("Furthest player on X axis is:" + furthestPlayer);
        //
        // transform.position = camPos;


        #region commented

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

        #endregion

    }

    #region commented2
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
#endregion

    Vector3 returnAveragePosition()
    {
        if (m_ActivePlayers.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 totalPosition = Vector3.zero;
        for (int i = 0; i < m_ActivePlayers.Count; i++)
        {
            totalPosition += m_ActivePlayers[i].transform.position;
        }
        
        totalPosition /= m_ActivePlayers.Count;
        
        return totalPosition;
    }

    // Calculate the camera position, based on the average of all active players.
    // This should be used until the camera is fully zoomed out, at which point the highest player tracking should kick into effect.
    void CalculateCameraPosition()
    {
        Vector3 camPos = transform.position;
        
        float left = camPos.x - deadZoneWidth / 2f;
        float right = camPos.x + deadZoneWidth / 2f;
        float top = camPos.y + deadZoneHeight / 2f;
        float bottom = camPos.y - deadZoneHeight / 2f;

        Vector3 newCamPos = camPos;

        for (int i = 0; i < m_ActivePlayers.Count; i++)
        {
            Vector3 playerPosition =  m_ActivePlayers[i].transform.position;

            if (playerPosition.x < left)
            {
                newCamPos.x += playerPosition.x - left;
            }
            else if (playerPosition.x > right)
            {
                newCamPos.x += playerPosition.x - right;
            }

            if (playerPosition.y > top)
            {
                newCamPos.y += playerPosition.y - top;
            }
            else if (playerPosition.y < bottom)
            {
                newCamPos.y += playerPosition.y - bottom;
            }
        }
        
        transform.position = new Vector3(newCamPos.x, newCamPos.y, transform.position.z);
    }

    void FollowHighestPlayer()
    {
        //Rewrite this logic, something isn't going right here, causing unnecessary jitter

        highestYPos = float.MinValue;
        int highestPlayer = -1;
        for (int i = 0; i < m_ActivePlayers.Count; i++)
        {
            float playerYPos = m_ActivePlayers[i].transform.position.y;

            if (playerYPos > highestYPos)
            {
                highestYPos = playerYPos;
                highestPlayer = i;
            }
        }

        if (highestPlayer == -1) // Essentially a null check
        {
            return;
        }

        Vector3 camPos = transform.position;
        Vector3 newCamPos = camPos;
        float top = camPos.y + deadZoneHeight / 2f;

        newCamPos.y += highestYPos - top;


        transform.position = newCamPos;






        #region Old Logic

        // Vector3 camPos = transform.position;
        //
        // float top = camPos.y + deadZoneHeight / 2f;
        // Vector3 newCamPos = camPos;
        //
        // // transform.position = new Vector3(newCamPos.x, newCamPos.y, transform.position.z);
        //
        //
        // float totalYPosition = 0f;
        // float furthestYPosition = float.MinValue;
        // int furthestTurtle = furthestPlayer;
        // for (int i = 0; i < m_ActivePlayers.Count; i++)
        // {
        //     float turtleYposition = m_ActivePlayers[i].transform.position.y;
        //     totalYPosition += turtleYposition;
        //
        //     if (turtleYposition > furthestYPosition)
        //     {
        //         furthestYPosition = turtleYposition;
        //         furthestTurtle = i;
        //     }
        // }
        //
        // float averageYPosition = totalYPosition / m_ActivePlayers.Count;
        //
        // if (furthestYPosition - averageYPosition > m_YBuffer)
        // {
        //     transform.position = new Vector3(transform.position.x,
        //         furthestYPosition - top, transform.position.z);
        // }
        // // else
        // // {
        // //     transform.position =
        // //         new Vector3(averageXPosition, transform.position.y, transform.position.z);
        // // }

        #endregion

    }

    void CalculateCameraZoom()
    {
        // float requiredHeight = float.MinValue;
        // float dynamicTVal = 10f;
        // float dynamicYBuffer = Mathf.Lerp(0f, m_YBuffer, playerBounds.size.y / dynamicTVal);
        // requiredHeight = Mathf.Abs(playerBounds.size.y / 2f + dynamicYBuffer);
        //
        // float requiredWidth = Mathf.Abs(playerBounds.size.x / 2f + m_XBuffer) / _camera.aspect;
        //
        // float requiredSize = Mathf.Max(requiredHeight, requiredWidth);
        //
        // //Scaled Buffers
        //
        // cameraZoom = Mathf.Clamp(requiredSize, minCameraZoom, maxCameraZoom);
        //
        // _camera.orthographicSize = cameraZoom;
        //
        // scaledXbuffer = m_XBuffer * cameraZoom / 5f; //Buffers are calibrated to be with 5 zoom
        // scaledYbuffer = m_YBuffer * cameraZoom / 5f; //Buffers are calibrated to be with 5 zoom
        
        // Recalculate bounds AFTER camera adjustment
        playerBounds = new Bounds(m_ActivePlayers[0].transform.position, Vector3.zero);
        foreach (var p in m_ActivePlayers)
            playerBounds.Encapsulate(p.transform.position);

        float requiredHeight = playerBounds.size.y / 2f + yBufferWorld;
        float requiredWidth  = (playerBounds.size.x / 2f + xBufferWorld) / _camera.aspect;

        desiredCameraZoom = Mathf.Max(requiredHeight, requiredWidth);

// Only zoom out if necessary
        // if (requiredSize > _camera.orthographicSize)
        // {
            _camera.orthographicSize = Mathf.Clamp(desiredCameraZoom, minCameraZoom, maxCameraZoom);
        // }
    }
    
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 camPos = transform.position;
       
        float zOffset = tempDepth;
        
        if (_camera != null)
        {
            zOffset = _camera.nearClipPlane;
        }
        camPos.z += zOffset;
        
        
        //Assume 16:9
        // float width = scaledXbuffer; /// 1920 / 0.615625f;
        // float height = scaledYbuffer * yBufferScale; /// 1080 / 0.615625f;

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
        
        // Player Buffer
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(camPos, new Vector3(deadZoneWidth, deadZoneHeight, 0f));
        
        // Player Bounds
        Gizmos.color = Color.brown;
        Gizmos.DrawWireCube(playerBounds.center, playerBounds.size);
        
        
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




