using UnityEngine;

//Written by: Matt Punyer

//All Update functions within this class should be called through the instance of e_GameEvents
//Event system implemented for ease of use with UI

public class p_PlayerData
{
    private int m_Score = 0;
    private int m_Deaths = 0;
    private int m_DistanceUp = 0;
    private int m_Health = 0;
    private int m_Lives = 0;
    private int m_PlayerID = 0;
    private int m_MaxHealth = 0;

    public p_PlayerData(int playerID, int health = 100, int lives = 3, int score = 0, int deaths = 0, int distanceUp = 0)
    {
        m_Score = score;
        m_Deaths = deaths;
        m_DistanceUp = distanceUp;
        m_Health = health;
        m_Lives = lives;
        m_PlayerID = playerID;
        m_MaxHealth = health;

        //Events
        e_GameEvents.instance.onPlayerHealthUpdate += UpdateHealth;
        e_GameEvents.instance.onPlayerScoreUpdate += UpdateScore;
        e_GameEvents.instance.onPlayerLivesUpdate += UpdateLives;

        e_GameEvents.instance.onPlayerDeathAdded += IncrementDeaths;
    }

    ~p_PlayerData()
    {
        e_GameEvents.instance.onPlayerHealthUpdate -= UpdateHealth;
        e_GameEvents.instance.onPlayerScoreUpdate -= UpdateScore;
        e_GameEvents.instance.onPlayerLivesUpdate -= UpdateLives;

        e_GameEvents.instance.onPlayerDeathAdded -= IncrementDeaths;
    }

    #region Score

    //Score Management
    public void UpdateScore(int newScore, int playerID)
    {
        if(playerID == m_PlayerID)
        {
            ChangeScore(newScore);
        }
    }

    private void ChangeScore(int newScore)
    {
        m_Score = newScore;
    }

    public int GetScore(int playerID) 
    {
        if(playerID == m_PlayerID)
        {
           return m_Score; 
        }

        return 0;
    }

    #endregion Score

    #region Deaths
    //Deaths Management

    public void IncrementDeaths(int playerID)
    {
        if(playerID == m_PlayerID)
        {
            m_Deaths++;
        }
    }

    public int GetDeaths() { return m_Deaths; }

    #endregion Deaths

    #region Distance
    //Distance Management

    public void IncrementDistance(int amount)
    {
        m_DistanceUp += amount;
    }

    public int GetDistanceMoved() { return m_DistanceUp; }

    #endregion Distance

    #region Health
    //Health Management
    private void ChangeHealth(int amount)
    {
        m_Health += amount;
    }

    public void UpdateHealth(int amount, int playerID)
    {

        if (playerID == m_PlayerID)
        {

            ChangeHealth(amount);
            Debug.Log(m_Health);

            if (CheckDead())
            {
                e_GameEvents.instance.PlayerLivesUpdate(-1, m_PlayerID);
                Debug.Log("Player " + m_PlayerID + " is Dead");
            }

        }
    }

    private bool CheckDead()
    {
        if(m_Health <= 0)
        {
            m_Health = 0;
            return true;
        }

        return false;
    }

    public int GetHealth() { return m_Health; }

    #endregion Health

    #region Lives
    //Lives Management

    private void ChangeLives(int amount)
    {
        m_Lives += amount;
    }

    public void UpdateLives(int amount, int playerID)
    {
        if (playerID == m_PlayerID)
        {
            ChangeLives(amount);

            if (CheckNoLives())
            {
                e_GameEvents.instance.PlayerNoLives(m_PlayerID);
            }

            e_GameEvents.instance.PlayerDeathAdded(m_PlayerID);

            ChangeHealth(m_MaxHealth);
        }
    }

    private bool CheckNoLives()
    {
        if(m_Lives <= 0)
        {
            return true;
        }

        return false;
    }
    #endregion Lives

    public int GetPlayerID() { return m_PlayerID; }


   
    /// <summary>
    /// Parse through the player tag, which will then return the int value of the player.
    /// Written by Connor Saysell.
    /// </summary>
    /// <returns>The integer ID of the player.</returns>
    /// <example>
    /// Example:
    /// <code>
    /// p_PlayerData.ReturnPlayerIDFromTag(Player1);
    /// </code>
    /// Returns 1 from this argument.
    /// </example>
    public static int ReturnPlayerIDFromTag(string tag)
    {
        if (tag.Contains("Player"))
        {
            string intPart = tag.Replace("Player", ""); //Remove the player part of the string
            int.TryParse(intPart, out int ID);
            return ID;
        }

        //If tag doesn't contain player, return error value
        return -1;
    }
}
