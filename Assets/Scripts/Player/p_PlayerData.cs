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

    public p_PlayerData(int score, int deaths, int distanceMoved, int health, int lives)
    {
        m_Score = score;
        m_Deaths = deaths;
        m_DistanceUp = distanceMoved;
        m_Health = health;
        m_Lives = lives;
    }

    public p_PlayerData() 
    {
        m_Score = 0;
        m_Deaths = 0;
        m_DistanceUp = 0;
        m_Lives = 3;
        m_Health = 100;

        //Events
        e_GameEvents.instance.onPlayerHealthUpdate += UpdateHealth;
        e_GameEvents.instance.onPlayerScoreUpdate += UpdateScore;
        e_GameEvents.instance.onPlayerLivesUpdate += UpdateLives;

        e_GameEvents.instance.onPlayerDeathAdded += IncrementDeaths;
    }

    #region Score

    //Score Management
    public void UpdateScore(bool up, int amount)
    {
        if(up)
        {
            IncrementScore(amount);
        }
        else
        {
            DecrementScore(amount);
        }
    }

    private void IncrementScore(int amount)
    {
        m_Score += amount;
    }

    private void DecrementScore(int amount) 
    { 
        m_Score -= amount; 
    }

    public int GetScore() { return m_Score; }

    #endregion Score

    #region Deaths
    //Deaths Management

    public void IncrementDeaths()
    {
        m_Deaths++;
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
    private void DecreaseHealth(int amount)
    {
        m_Health -= amount;
    }

    private void IncreaseHealth(int amount)
    {
        m_Health += amount;
    }

    public void UpdateHealth(bool up, int amount)
    {
        if(up)
        {
            IncreaseHealth(amount);
        }
        else
        {
            DecreaseHealth(amount);
            
            if (CheckDead())
            {
                e_GameEvents.instance.PlayerLivesUpdate(false, 1);
            }
        }
    }

    private bool CheckDead()
    {
        if(m_Health <= 0)
        {
            return true;
        }

        return false;
    }

    public void KillPlayer()
    {
        e_GameEvents.instance.PlayerHealthUpdate(false, GetHealth());
    }

    public int GetHealth() { return m_Health; }

    #endregion Health

    #region Lives
    //Lives Management

    private void AddLives(int amount)
    {
        m_Lives += amount;
    }

    private void DecreaseLives(int amount)
    {
        m_Lives -= amount;
    }

    public void UpdateLives(bool up, int amount)
    {
        if(up)
        {
            AddLives(amount);
        }
        else
        {
            DecreaseLives(amount);

            if(CheckNoLives())
            {
                //Do Something (End Game)
            }
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
}
