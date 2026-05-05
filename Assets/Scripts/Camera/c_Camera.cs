using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

//WRITTEN BY CONNOR SAYSELL - DO NOT BREAK PLEASE (IT TOOK FAR TOO LONG TO GET WORKING)

[RequireComponent(typeof(Camera))] // - Make sure a camera exists on this object
public class c_Camera : MonoBehaviour
{
    [Header("Buffers")]
    [Header("This will not visually update while not in play mode.")]
    
    [Tooltip("What percentage of the screen the Y buffer should be. This is a half value, as it will apply this to both the Top and Bottom")]
    [Range(0f, 50f)] [SerializeField] private float m_yBufferPercent = 20.0f;

    [Tooltip("What percentage of the screen the X buffer should be. This is a half value, as it will apply this to both the Left and Right")]
    [Range(0f, 50f)] [SerializeField] private float m_xBufferPercent = 15.0f;
    
    [Header("Camera Values")] 
    [SerializeField] private float m_minCameraZoom = 5f;
    [SerializeField] private float m_maxCameraZoom = 30f;

    [SerializeField] private float m_lerpTime = 0.8f;

    [Tooltip("Do you want to draw the bounds of the players and the boundaries? Recommended for sorting values, otherwise can be turned off")]
    [SerializeField] private bool m_debugDraw;
    
    
    //DEBUG VALUES - NOT EXPOSED UNLESS IN DEBUG MODE
    private Camera m_camera;
    private Bounds m_playerBounds;

    // This gets taken from the global Data Script
    List<GameObject> m_playersToTrack;

    // These are the players who are considered 'Active', AKA they are not dead.
    List<GameObject> m_activePlayers;

    private int m_numOfActivePlayersLastFrame = 2;

    // Camera Values
    private float m_desiredCameraZoom = 5f;

    private Vector3 m_camPosAtStartOfLerp = Vector3.zero;
    private float m_camZoomAtStartOfLerp = 5.0f;
    private float m_currentLerpTime = 0.0f;
    private bool m_doCameraLerp = false;
    
    // Dead Zone Values
    private float m_deadZoneHeight = 6.0f;
    private float m_deadZoneWidth = 12.45f;
    private float m_xBufferWorld = 2.67f;
    private float m_yBufferWorld = 2.0f;

    // This gets called at the start of the scene load, to scale with the world. Default 1 in case of no GDF
    private float m_worldScale = 1.0f;

    void Start()
    {
        m_worldScale = e_GlobalData.instance.GetWorldScale();

        m_camera = GetComponent<Camera>();

        m_playersToTrack = new List<GameObject>();
        m_playersToTrack.Add(e_GlobalData.instance.GetPlayer(0));
        m_playersToTrack.Add(e_GlobalData.instance.GetPlayer(1));

        m_activePlayers = new List<GameObject>();
    }

    void Update()
    {
        SetActivePlayers();
        
        if (m_activePlayers.Count == 0) { return; }
        
        DoPlayerBounds();
        
        CheckLerpCondition();
        
        SetCameraValues();

        DoCameraPosition();

        DoCameraZoom();
    }

    void SetActivePlayers()
    {
        m_activePlayers.Clear();
        for (int i = 0; i < m_playersToTrack.Count; i++)
        {
            if (m_playersToTrack[i].activeSelf)
            {
                m_activePlayers.Add(m_playersToTrack[i]);
            }
        }
    }

    /// <summary>
    /// Reset the player bounds each frame
    /// Written by Connor Saysell.
    /// </summary>
    void DoPlayerBounds()
    {
        m_playerBounds = new Bounds(m_activePlayers[0].transform.position, Vector3.zero);
        for (int i = 0; i < m_activePlayers.Count; i++)
        {
            m_playerBounds.Encapsulate(m_activePlayers[i].transform.position);
        }
    }

    /// <summary>
    /// Does a player number check to see whether the camera should be lerping
    /// Written by Connor Saysell.
    /// </summary>
    void CheckLerpCondition()
    {
        if (m_numOfActivePlayersLastFrame != m_activePlayers.Count)
        {
            m_numOfActivePlayersLastFrame = m_activePlayers.Count;
            StartCoroutine(StartCameraLerp());
        }

        if (m_doCameraLerp)
        {
            m_currentLerpTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Set values for the camera such as the dead zone and buffers, ready to be used by other parts of the script.
    /// Written by Connor Saysell.
    /// </summary>
    void SetCameraValues()
    {
        float camHeight = m_camera.orthographicSize * 2;
        float camWidth = camHeight * m_camera.aspect;

        m_yBufferWorld = camHeight * (m_yBufferPercent / 100.0f);
        m_xBufferWorld = camWidth * (m_xBufferPercent / 100.0f);

        m_deadZoneHeight = camHeight - (m_yBufferWorld * 2f);
        m_deadZoneWidth = camWidth - (m_xBufferWorld * 2f);
    }
    
    /// <summary>
    /// This functions handles everything surrounding the camera position, from doing the average position, to the highest and also the lerping when needed.
    /// Written by Connor Saysell.
    /// </summary>
    void DoCameraPosition()
    {
        Vector3 targetCamPos = (m_desiredCameraZoom < m_maxCameraZoom) ? GetAverageCameraPosition() : GetHighestPlayerCamPosition();
        
        if (m_doCameraLerp)
        {
            targetCamPos = Vector3.Lerp(m_camPosAtStartOfLerp, targetCamPos, m_currentLerpTime / m_lerpTime);
        }

        transform.position = targetCamPos;
    }

    /// <summary>
    /// Calculate the camera position, based on the average of all active players.
    /// This should be used until the camera is fully zoomed out, at which point the highest player tracking should kick into effect.
    /// Written by Connor Saysell.
    /// </summary>
    Vector3 GetAverageCameraPosition()
    {
        Vector3 camPos = transform.position;

        float left = camPos.x - m_deadZoneWidth / 2f;
        float right = camPos.x + m_deadZoneWidth / 2f;
        float top = camPos.y + m_deadZoneHeight / 2f;
        float bottom = camPos.y - m_deadZoneHeight / 2f;

        for (int i = 0; i < m_activePlayers.Count; i++)
        {
            Vector3 playerPosition = m_activePlayers[i].transform.position;

            if (playerPosition.x < left)
            {
                camPos.x += playerPosition.x - left;
            }
            else if (playerPosition.x > right)
            {
                camPos.x += playerPosition.x - right;
            }

            if (playerPosition.y > top)
            {
                camPos.y += playerPosition.y - top;
            }
            else if (playerPosition.y < bottom)
            {
                camPos.y += playerPosition.y - bottom;
            }
        }

        return new Vector3(camPos.x, camPos.y, transform.position.z);
    }

    /// <summary>
    /// Calculate the position of the camera when following the highest player while the camera is fully zoomed out and following the average player no longer keeps it in the bounding box.
    /// Written by Connor Saysell.
    /// </summary>
    Vector3 GetHighestPlayerCamPosition()
    {
        float highestYPos = float.MinValue;
        for (int i = 0; i < m_activePlayers.Count; i++)
        {
            highestYPos = Mathf.Max(m_activePlayers[i].transform.position.y, highestYPos);
        }

        Vector3 camPos = transform.position;
        float top = camPos.y + m_deadZoneHeight / 2f;

        camPos.y += highestYPos - top;
        
        return camPos;
    }
    
    /// <summary>
    /// This calculates and applies the camera zoom for the camera.  
    /// Written by Connor Saysell.
    /// </summary>
    void DoCameraZoom()
    {
        float requiredHeight = m_playerBounds.size.y / 2f + m_yBufferWorld;
        float requiredWidth = (m_playerBounds.size.x / 2f + m_xBufferWorld) / m_camera.aspect;

        m_desiredCameraZoom = Mathf.Max(requiredHeight, requiredWidth);

        m_desiredCameraZoom = Mathf.Clamp(m_desiredCameraZoom, m_minCameraZoom * m_worldScale, m_maxCameraZoom * m_worldScale);
        
        if (m_doCameraLerp)
        {
            m_desiredCameraZoom = Mathf.Lerp(m_camZoomAtStartOfLerp, m_desiredCameraZoom, m_currentLerpTime / m_lerpTime);
        }

        m_camera.orthographicSize = m_desiredCameraZoom;
    }

    /// <summary>
    /// This starts the camera lerp process.
    /// Written by Connor Saysell.
    /// </summary>
    IEnumerator StartCameraLerp()
    {
        m_doCameraLerp = true;
        m_currentLerpTime = 0;
        m_camZoomAtStartOfLerp = m_camera.orthographicSize;
        m_camPosAtStartOfLerp = transform.position;
        yield return new WaitForSeconds(m_lerpTime);

        // Add the same null check that was added for the player respawning here.
        m_doCameraLerp = false;
    }
    
    void OnDrawGizmos()
    {
        if (m_debugDraw)
        {
            Vector3 camPos = transform.position;
            float zOffset = 0.3f; //Default Value in case of no camera reference - Should be near clipping plane value

            if (m_camera != null)
            {
                zOffset = m_camera.nearClipPlane;
            }

            camPos.z += zOffset;
            
            // Player Buffer
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(camPos, new Vector3(m_deadZoneWidth, m_deadZoneHeight, 0f));

            // Player Bounds
            Gizmos.color = Color.brown;
            Gizmos.DrawWireCube(m_playerBounds.center, m_playerBounds.size);
        }
    }
}