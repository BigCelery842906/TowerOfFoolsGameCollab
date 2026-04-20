using System.Collections.Generic;
using UnityEngine;

// Written by Connor Saysell
public class e_GlobalData : MonoBehaviour
{

    public static e_GlobalData instance;
    [SerializeField] float m_WorldScale;
    [SerializeField] private float m_PlayerScale;
    [Tooltip("The players that you intend to track. It will auto populate on start, overriding anything put in here previously. It can be changed to not do this if needed.")]
    [SerializeField] private List<GameObject> m_PlayersToTrack;
    [SerializeField] private float m_TimeSpentInCurrentRoom;
    [SerializeField] private float m_TimeSpentInGame;

    [SerializeField] private bool m_IsPaused;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        GameObject[] tempGO = GameObject.FindGameObjectsWithTag("Player");
        // This would be the capsule component of the player, so I need to get the parent for the actual physical player
        
        m_PlayersToTrack = new List<GameObject>();
        for (int i = 0; i < tempGO.Length; i++)
        {
            m_PlayersToTrack.Add(tempGO[i].transform.parent.gameObject);
        }
        
        m_TimeSpentInCurrentRoom = 0;
        m_TimeSpentInGame = 0;
    }
    
    void Update()
    {
        m_TimeSpentInCurrentRoom += Time.deltaTime;
        m_TimeSpentInGame += Time.deltaTime;
    }

    public float GetWorldScale()
    {
        return m_WorldScale;
    }

    public float GetPlayerScale()
    {
        return m_PlayerScale;
    }

    public Vector3 GetPlayerPosition(int playerNum)
    {
        if (playerNum is 0 or 1)
        {
            return m_PlayersToTrack[playerNum].transform.position;
        }
        else return Vector3.zero;
    }

    public GameObject GetPlayer(int playerNum)
    {
        if (playerNum is 0 or 1)
        {
            return m_PlayersToTrack[playerNum];
        }
        else return null;
    }
    
    public void ResetRoomTimer()
    {
        m_TimeSpentInCurrentRoom = 0;
    }
    
    public float GetCurrentTimeSpentInRoom()
    {
        return m_TimeSpentInCurrentRoom;
    }

    public float GetCurrentTimeSpentInGame()
    {
        return m_TimeSpentInGame;
    }

    public bool TogglePause()
    {
        if (m_IsPaused)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        m_IsPaused = !m_IsPaused;
        return m_IsPaused;
    }

    public void SetPause(bool isPaused)
    {
        m_IsPaused = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }
}
