using UnityEngine;

public class p_PlayerData
{
    private int m_Score = 0;
    private int m_Deaths = 0;
    private int m_DistanceUp = 0;

    public p_PlayerData(int score, int deaths, int distanceMoved)
    {
        m_Score = score;
        m_Deaths = deaths;
        m_DistanceUp = distanceMoved;
    }

    public p_PlayerData() 
    {
        m_Score = 0;
        m_Deaths = 0;
        m_DistanceUp = 0;
    }

    //Score Management
    public void UpdateScore(int amount)
    {
        m_Score = amount;
    }

    public void IncrementScore(int amount)
    {
        m_Score += amount;
    }

    public void DecrementScore(int amount) 
    { 
        m_Score -= amount; 
    }

    public int GetScore() { return m_Score; }

    //Deaths Management

    public void IncrementDeaths()
    {
        m_Deaths++;
    }

    public int GetDeaths() { return m_Deaths; }

    //Distance Management

    public void IncrementDistance(int amount)
    {
        m_DistanceUp += amount;
    }

    public int GetDistanceMoved() { return m_DistanceUp; }
}
