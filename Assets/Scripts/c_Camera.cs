using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

//WRITTEN BY CONNOR SAYSELL - DO NOT BREAK PLEASE (IT TOOK FAR TOO LONG TO GET WORKING)

[RequireComponent(typeof(Camera))] // - Make sure a camera exists on this object
public class c_Camera : MonoBehaviour
{
    #region Camera Plan 
    // Camera Plan for Script
    //     Average Position - TODO: DONE
    //          If all active players are within the average boundary, then track average position
    //
    //          If a player is outside the boundary, then zoom the camera out to put both players
    //          inside the boundary, up to a specified value.
    //
    //     Zoomed and tracking Furthest Player - TODO: Need to Implement the lava component x distance below the camera. - Currently waiting for tyler to finish with the lava before going in just in case.
    //          If the specified value is reached, then prioritise the top player.
    //          Keep this top player within the top buffer of the camera, so they can always see
    //          the next platform they need to jump to.
    //
    //          If the bottom player falls too far out of camera, have a lava object below it (0 speed)
    //          - Can set this dynamically based on the max zoom value
    //
    //     Zoomed and returning to average - TODO: DONE
    //          If zoomed out, but then only one player becomes active,
    //          lerp to being zoomed in on the average position
    //      
    //     TODO: Something when all players are dead. Not sure how I missed that - Might be able to get by with this since it just essentially errors to find a new position, so freezes.
    //     TODO: REFACTOR THIS
    #endregion

    // EXPOSED VALUES
    [Header("Buffers")]
    
    [Header("This will not visually update while not in play mode.")]
    [Tooltip("What percentage of the screen the Y buffer should be. This is a half value, as it will apply this to both the top and bottom")]
    [Range(0f, 50f)] [SerializeField] private float m_YBufferPercent = 20.0f;

    [Tooltip("What percentage of the screen the X buffer should be. This is a half value, as it will apply this to both the Left and Right")]
    [Range(0f, 50f)] [SerializeField] private float m_XBufferPercent = 15.0f;

    [Space(50.0f)]
    
    [Header("Camera Values")] 
    [SerializeField] private float m_MinCameraZoom = 5f;
    [SerializeField] private float m_MaxCameraZoom = 30f;

    [SerializeField] private float m_LerpTime = 0.8f;

    [Tooltip("Do you want to draw the bounds of the players and the boundaries? Recommended for sorting values, otherwise can be turned off")]
    [SerializeField] private bool m_DebugDraw;
    
    
    //DEBUG VALUES - NOT EXPOSED UNLESS IN DEBUG MODE
    private Camera m_Camera;
    private Bounds m_PlayerBounds;

    // This gets taken from the global Data Script
    List<GameObject> m_PlayersToTrack;

    // These are the players who are considered 'Active', AKA they are not dead.
    List<GameObject> m_ActivePlayers;

    private int m_NumOfActivePlayersLastFrame = 2;

    // Camera Values
    private float m_CameraZoom = 5f;
    private float m_DesiredCameraZoom = 5f;

    private Vector3 m_camPosAtStartOfLerp = Vector3.zero;
    private float m_CamZoomAtStartOfLerp = 5.0f;
    private Vector3 m_desiredCamPos = Vector3.zero;
    private float m_currentLerpTime = 0.0f;
    private bool m_DoCameraLerp = false;
    
    // Dead Zone Values
    private float m_DeadZoneHeight = 6.0f;
    private float m_DeadZoneWidth = 12.45f;
    private float m_XBufferWorld = 2.67f;
    private float m_YBufferWorld = 2.0f;

    // This gets called at the start of the scene load, to scale with the world. Default 1 in case of no GDF
    private float m_WorldScale = 1.0f;

    void Start()
    {
        m_WorldScale = e_GlobalData.instance.GetWorldScale();

        m_Camera = GetComponent<Camera>();

        m_PlayersToTrack = new List<GameObject>();
        m_PlayersToTrack.Add(e_GlobalData.instance.GetPlayer(0));
        m_PlayersToTrack.Add(e_GlobalData.instance.GetPlayer(1));

        m_ActivePlayers = new List<GameObject>();
    }

    void Update()
    {
        SetActivePlayers();

        DoPlayerBounds();
        
        CheckLerpCondition();
        
        SetCameraValues();

        //Check for camera mode
        if (m_DesiredCameraZoom < m_MaxCameraZoom)
        {
            CalculateCameraPosition();
        }
        else
        {
            FollowHighestPlayer();
        }

        ApplyCameraPosition();

        DoCameraZoom();
        
    }

    void SetActivePlayers()
    {
        m_ActivePlayers.Clear();
        for (int i = 0; i < m_PlayersToTrack.Count; i++)
        {
            if (m_PlayersToTrack[i].activeSelf)
            {
                m_ActivePlayers.Add(m_PlayersToTrack[i]);
            }
        }
    }

    void DoPlayerBounds()
    {
        //Reset the player bounds each frame
        if (m_ActivePlayers.Count == 0) { return; }
        
        m_PlayerBounds = new Bounds(m_ActivePlayers[0].transform.position, Vector3.zero);
        for (int i = 0; i < m_ActivePlayers.Count; i++)
        {
            m_PlayerBounds.Encapsulate(m_ActivePlayers[i].transform.position);
        }
    }

    void CheckLerpCondition()
    {
        if (m_NumOfActivePlayersLastFrame != m_ActivePlayers.Count)
        {
            m_NumOfActivePlayersLastFrame = m_ActivePlayers.Count;
            StartCoroutine(StartCameraLerp());
        }

        if (m_DoCameraLerp)
        {
            m_currentLerpTime += Time.deltaTime;
        }
    }

    void SetCameraValues()
    {
        float m_CamHeight = m_Camera.orthographicSize * 2;
        float m_CamWidth = m_CamHeight * m_Camera.aspect;

        m_YBufferWorld = m_CamHeight * (m_YBufferPercent / 100.0f);
        m_XBufferWorld = m_CamWidth * (m_XBufferPercent / 100.0f);

        m_DeadZoneHeight = m_CamHeight - (m_YBufferWorld * 2f);
        m_DeadZoneWidth = m_CamWidth - (m_XBufferWorld * 2f);
    }

    /// <summary>
    /// Calculate the camera position, based on the average of all active players.
    /// This should be used until the camera is fully zoomed out, at which point the highest player tracking should kick into effect.
    /// Written by Connor Saysell.
    /// </summary>
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

        m_desiredCamPos = new Vector3(newCamPos.x, newCamPos.y, transform.position.z);
    }

    /// <summary>
    /// Calculate the position of the camera when following the highest player while the camera is fully zoomed out and following the average player no longer keeps it in the bounding box
    /// Written by Connor Saysell.
    /// </summary>
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


        m_desiredCamPos = newCamPos;

    }
    
    /// <summary>
    /// This applies the camera position previously calculated, doing a check for the camera lerp if necessary.
    /// Written by Connor Saysell.
    /// </summary>
    void ApplyCameraPosition()
    {
        Vector3 camPos = m_desiredCamPos;
        if (m_DoCameraLerp)
        {
            camPos = DoCameraPositionLerp();
        }

        transform.position = camPos;
    }
    
    /// <summary>
    /// This applies the camera position based on where in the lerp process the camera is in
    /// Written by Connor Saysell.
    /// </summary>
    Vector3 DoCameraPositionLerp()
    {
        // Vector3 cameraPos = Vector3.Lerp(transform.position, m_desiredCamPos, m_currentLerpTime / m_LerpTime);
        Vector3 cameraPos = Vector3.Lerp(m_camPosAtStartOfLerp, m_desiredCamPos, m_currentLerpTime / m_LerpTime);
        return cameraPos;
    }


    void DoCameraZoom()
    {
        float requiredHeight = m_PlayerBounds.size.y / 2f + m_YBufferWorld;
        float requiredWidth = (m_PlayerBounds.size.x / 2f + m_XBufferWorld) / m_Camera.aspect;

        m_DesiredCameraZoom = Mathf.Max(requiredHeight, requiredWidth);

        m_DesiredCameraZoom = Mathf.Clamp(m_DesiredCameraZoom, m_MinCameraZoom * m_WorldScale,
            m_MaxCameraZoom * m_WorldScale);
        
        float camZoom = m_DesiredCameraZoom;
        if (m_DoCameraLerp)
        {
            camZoom = DoCameraZoomLerp();
        }

        m_CameraZoom = Mathf.Clamp(camZoom, m_MinCameraZoom * m_WorldScale, m_MaxCameraZoom * m_WorldScale);
        m_Camera.orthographicSize = m_CameraZoom;
    }
    /// <summary>
    /// This calculates the desired zoom level for the camera at a given point. It does NOT apply it. This should be done with ApplyCameraZoom();
    /// Written by Connor Saysell.
    /// </summary>
    void CalculateCameraZoom()
    {
        

    }
    
    /// <summary>
    /// This applies the camera zoom previously calculated, doing a check for the camera lerp if necessary.
    /// Written by Connor Saysell.
    /// </summary>
    void ApplyCameraZoom()
    {
       
    }

    /// <summary>
    /// This applies the camera zoom based on where in the lerp process the camera is in
    /// Written by Connor Saysell.
    /// </summary>
    private float DoCameraZoomLerp()
    {
        float camZoom = Mathf.Lerp(m_CamZoomAtStartOfLerp, m_DesiredCameraZoom, m_currentLerpTime / m_LerpTime);
        // float camZoom = Mathf.Lerp(m_CameraZoom, m_DesiredCameraZoom, m_currentLerpTime / m_LerpTime);
        return camZoom;
    }

    /// <summary>
    /// This starts the camera lerp process 
    /// Written by Connor Saysell.
    /// </summary>
    IEnumerator StartCameraLerp()
    {
        m_DoCameraLerp = true;
        m_currentLerpTime = 0;
        m_CamZoomAtStartOfLerp = m_CameraZoom;
        m_camPosAtStartOfLerp = transform.position;
        yield return new WaitForSeconds(m_LerpTime);

        // Add the same null check that was added for the player respawning here.
        m_DoCameraLerp = false;
    }
    
    float tempDepth = 0.3f; // Should be the near clipping plane on the camera.

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