using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
//WRITTEN BY CONNOR SAYSELL - DO NOT BREAK PLEASE (IT TOOK FAR TOO LONG TO GET WORKING)
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
    //      
    //     TODO: Something when all players are dead. Not sure how I missed that


    
    [Header("Buffers")]
    [Header("This will not visually update while not in play mode.")]
    [Tooltip("What percentage of the screen the Y buffer should be. This is a half value, as it will apply this to both the top and bottom")]
    [Range(0f, 50f)] [SerializeField] private float m_YBufferPercent = 20.0f;

    //Might want a bottom buffer later on
    
    [Tooltip("What percentage of the screen the X buffer should be. This is a half value, as it will apply this to both the Left and Right")]
    [Range(0f, 50f)] [SerializeField] private float m_XBufferPercent = 15.0f;
    

    [Header("Camera Values")] 
    private Camera m_Camera;
    [SerializeField] private float m_MinCameraZoom = 5f;
    [SerializeField] private float m_MaxCameraZoom = 30f;
    

    private Bounds m_PlayerBounds;
    
    
    [Tooltip("Do you want to draw the bounds of the players and the boundaries? Recommended for sorting values, otherwise can be turned off")]
    [SerializeField] private bool m_DebugDraw;
    
    [Header("Debug Values - Not Editable")]
    
    [Tooltip("The players that you intend to track. It will auto populate on start, overriding anything put in here previously. This gets taken from the global Data Script, edit from there.")]
    [SerializeField]
    List<GameObject> m_PlayersToTrack;
    
    [Tooltip("These are the players who are considered 'Active', AKA they are not dead. These are the players the camera is trying to track.")]
    [SerializeField] List<GameObject> m_ActivePlayers;

    [SerializeField] private bool m_TrackingAverage = true;
    private float tempDepth = 0.3f; // This should not be exposed, should be the near clipping plane on the camera.
    
    [Header("Camera Values")] 
    [SerializeField] private float m_CameraZoom = 5f;
    [SerializeField] private float m_DesiredCameraZoom = 5f;

    //Might need a float for desired X and Y zoom, then take max from that.
    [SerializeField] private float m_CamHeight;
    [SerializeField] private float m_CamWidth;

    
    [Header("Dead Zone Values")] 
    [SerializeField] private float m_DeadZoneHeight = 6.0f;
    [SerializeField] private float m_DeadZoneWidth = 12.45f;
    [SerializeField] private float m_XBufferWorld = 2.67f;
    [SerializeField] private float m_YBufferWorld = 2.0f;

    [Tooltip("This gets called at the start of the scene load, to scale with the world. If you want to change how much the camera sees, use the min and max camera zoom.")]
    [SerializeField] private float m_WorldScale = 1.0f;

    enum CameraStates
    {
        trackingHighest,
        trackingAverage,
        transitioning,
    }

    void Start()
    {
        m_Camera = GetComponent<Camera>();

        m_PlayersToTrack.Add(e_GlobalData.instance.GetPlayer(0));
        m_PlayersToTrack.Add(e_GlobalData.instance.GetPlayer(1));

        m_ActivePlayers.Clear();
        for (int i = 0; i < m_PlayersToTrack.Count; i++)
        {
            if (m_PlayersToTrack[i].activeSelf)
            {
                m_ActivePlayers.Add(m_PlayersToTrack[i]);
            }
        }

        m_PlayerBounds = new Bounds(m_ActivePlayers[0].transform.position, Vector3.zero);
        
        m_WorldScale = e_GlobalData.instance.GetWorldScale();
    }

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
        m_PlayerBounds = new Bounds(m_ActivePlayers[0].transform.position, Vector3.zero);
        for (int i = 0; i < m_ActivePlayers.Count; i++)
        {
            m_PlayerBounds.Encapsulate(m_ActivePlayers[i].transform.position);
        }

        m_CamHeight = m_Camera.orthographicSize * 2;
        m_CamWidth = m_CamHeight * m_Camera.aspect;

        m_YBufferWorld = m_CamHeight * (m_YBufferPercent / 100.0f);
        m_XBufferWorld = m_CamWidth * (m_XBufferPercent / 100.0f);

        m_DeadZoneHeight = m_CamHeight - (m_YBufferWorld * 2f);
        m_DeadZoneWidth = m_CamWidth - (m_XBufferWorld * 2f);

        if (m_DesiredCameraZoom < m_MaxCameraZoom)
        {
            CalculateCameraPosition();
            m_TrackingAverage = true;
        }
        else
        {
            FollowHighestPlayer();
            m_TrackingAverage = false;
        }

        CalculateCameraZoom();


    }

    // Calculate the camera position, based on the average of all active players.
    // This should be used until the camera is fully zoomed out, at which point the highest player tracking should kick into effect.
    void CalculateCameraPosition()
    {
        Vector3 camPos = transform.position;

        float left = camPos.x - m_DeadZoneWidth / 2f;
        float right = camPos.x + m_DeadZoneWidth / 2f;
        float top = camPos.y + m_DeadZoneHeight / 2f;
        float bottom = camPos.y - m_DeadZoneHeight / 2f;

        Vector3 newCamPos = camPos;

        for (int i = 0; i < m_ActivePlayers.Count; i++)
        {
            Vector3 playerPosition = m_ActivePlayers[i].transform.position;

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

        float highestYPos = float.MinValue;
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
        float top = camPos.y + m_DeadZoneHeight / 2f;

        newCamPos.y += highestYPos - top;


        transform.position = newCamPos;

    }

    void CalculateCameraZoom()
    {
        m_PlayerBounds = new Bounds(m_ActivePlayers[0].transform.position, Vector3.zero);

        for (int i = 0; i < m_ActivePlayers.Count; i++)
        {
            m_PlayerBounds.Encapsulate(m_ActivePlayers[i].transform.position);
        }

        float requiredHeight = m_PlayerBounds.size.y / 2f + m_YBufferWorld;
        float requiredWidth = (m_PlayerBounds.size.x / 2f + m_XBufferWorld) / m_Camera.aspect;

        m_DesiredCameraZoom = Mathf.Max(requiredHeight, requiredWidth);

        m_Camera.orthographicSize = Mathf.Clamp(m_DesiredCameraZoom, m_MinCameraZoom * m_WorldScale, m_MaxCameraZoom * m_WorldScale);
    }


    void OnDrawGizmos()
    {
        if (m_DebugDraw)
        {
            Vector3 camPos = transform.position;
            float zOffset = tempDepth; //Default Value in case of no camera reference

            if (m_Camera != null)
            {
                zOffset = m_Camera.nearClipPlane;
            }

            camPos.z += zOffset;


            // Player Buffer
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(camPos, new Vector3(m_DeadZoneWidth, m_DeadZoneHeight, 0f));

            // Player Bounds
            Gizmos.color = Color.brown;
            Gizmos.DrawWireCube(m_PlayerBounds.center, m_PlayerBounds.size);
        }
    }

}