using UnityEngine;

public class c_Camera : MonoBehaviour
{

    [SerializeField] GameObject[] m_PlayersToTrack;

    
    
    [Header("Buffers")] 
    [SerializeField]private float m_YBuffer = 50f;
    // [SerializeField]private float m_BottomBuffer = 50f;
    [SerializeField]private float m_XBuffer = 50f;
    // [SerializeField]private float m_RightBuffer = 50f;

    [SerializeField] private float tempDepth = 0.5f;
    
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
        Vector3 camPos = transform.position;
        Vector3 avg = returnAveragePosition();

// --- STEP 1: Get bounds of players ---
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var player in m_PlayersToTrack)
        {
            Vector3 pos = player.transform.position;

            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxY = Mathf.Max(maxY, pos.y);
        }

// --- STEP 2: Dead zone bounds ---
        float left = camPos.x - m_XBuffer;
        float right = camPos.x + m_XBuffer;
        float bottom = camPos.y - m_YBuffer;
        float top = camPos.y + m_YBuffer;

// --- STEP 3: Check if ALL players are inside ---
        bool insideDeadZone =
            minX >= left &&
            maxX <= right &&
            minY >= bottom &&
            maxY <= top;

        float targetX = camPos.x;
        float targetY = camPos.y;

        if (insideDeadZone)
        {
            // Follow average ONLY when safe
            targetX = avg.x;
            targetY = avg.y;
        }
        else
        {
            // --- Horizontal: keep everyone in view ---
            if (minX < left)
                targetX += minX - left;
            else if (maxX > right)
                targetX += maxX - right;

            // --- Vertical: PRIORITISE TOP PLAYER ---
            if (maxY > top)
            {
                // push camera up first
                targetY += maxY - top;
            }
            else if (minY < bottom)
            {
                // only go down if no top pressure
                targetY += minY - bottom;
            }
        }

        transform.position = new Vector3(targetX, targetY, camPos.z);
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
    
    
    
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 camPos = transform.position;

        camPos.z = transform.position.z + 0.3f;
        //Assume 16:9
        float width = m_XBuffer / 1920 / 0.615625f;
        float height = m_YBuffer/ 1080 / 0.615625f;

        // At 1080 x 1920
        // Y = 374
        // X = 1182
        
        // At / 1920 / 0.615625f and / 1080 / 0.615625f;
        // Y = 727
        // X = 230
        
        
        Gizmos.DrawWireCube(camPos, new Vector3(width, height, 0f));
        
        
        
        #region BS
        
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




