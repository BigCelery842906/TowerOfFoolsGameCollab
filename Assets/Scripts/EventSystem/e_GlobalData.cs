using System.Collections.Generic;
using UnityEngine;

public struct PlayerDataInfo
{
    public int deaths;
    public int score;

}

// Written by Connor Saysell
public class e_GlobalData : MonoBehaviour
{

    public static e_GlobalData instance;
    
    [SerializeField] float m_WorldScale = 1;
    [SerializeField] private float m_PlayerScale = 1;
    
    private List<GameObject> m_PlayersToTrack;
    private float m_TimeSpentInCurrentRoom = 0;
    private float m_TimeSpentInGame = 0 ;
    private float m_lavaSpeedMultiplier = 0;
    private float m_lavaInitialSpeed;

    private PlayerDataInfo m_PlayerOneData;
    private PlayerDataInfo m_PlayerTwoData;

    private bool m_IsPaused = false;

    private bool m_HasGameEnded = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        if (m_PlayersToTrack.Count > playerNum){
        }
        if (m_PlayersToTrack[playerNum] != null)
        {
            return  m_PlayersToTrack[playerNum];
        }
        else
        {
            return null;
        }
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
        m_IsPaused = !m_IsPaused;
        Time.timeScale = m_IsPaused ? 0 : 1;
        e_GameEvents.instance.PauseToggle(m_IsPaused);
        return m_IsPaused;
    }

    public void SetPause(bool isPaused)
    {
        m_IsPaused = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        e_GameEvents.instance.PauseToggle(m_IsPaused);
    }
    
    public void SetGameEnded(bool isGameEnded)
    {
        p_PlayerDataManager pdManagerOne = m_PlayersToTrack[0].GetComponent<p_PlayerDataManager>();
        p_PlayerDataManager pdManagerTwo = m_PlayersToTrack[1].GetComponent<p_PlayerDataManager>();

        p_PlayerData pdOne = pdManagerOne.GetPlayerData();
        p_PlayerData pdTwo = pdManagerTwo.GetPlayerData();

        // populate player data info structs
        m_PlayerOneData.score = pdOne.GetScore();
        m_PlayerTwoData.score = pdTwo.GetScore();

        m_HasGameEnded = isGameEnded;
    }

    public bool GetGameEnded()
    {
        return m_HasGameEnded;
    }
    
    public void SetLavaComponents(float lavaSpeedMultiplier, float initialSpeed)
    {
        m_lavaSpeedMultiplier = lavaSpeedMultiplier;
        m_lavaInitialSpeed = initialSpeed;
    }
    
    public float GetCurrentLavaSpeed()
    {
        return m_lavaInitialSpeed + (m_lavaInitialSpeed * m_lavaSpeedMultiplier * GetCurrentTimeSpentInGame()); 
    }

    /// <summary>
    /// Get the player data info struct for the given player
    /// </summary>
    /// <param name="playerId">0 for player 1, 1 for player 2</param>
    public PlayerDataInfo GetPlayerDataInfo(int playerId)
    {
        return playerId == 0 ? m_PlayerOneData : m_PlayerTwoData;
    }

    public void ResetGameStates()
    {
        // reset the player datas to be empty new struct instances
        m_PlayerOneData = new PlayerDataInfo();
        m_PlayerTwoData = new PlayerDataInfo();

        // reset game ended state
        m_HasGameEnded = false;
    }
}
