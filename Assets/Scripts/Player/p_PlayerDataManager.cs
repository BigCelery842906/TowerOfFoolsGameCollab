using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class p_PlayerDataManager : MonoBehaviour
{

    p_PlayerData m_PlayerData = null;

    void Start()
    {
        if(gameObject.CompareTag("Player1"))
        {
            m_PlayerData = new p_PlayerData(0);
            Debug.Log("Player 1 Data Created");
        }
        else if(gameObject.CompareTag("Player2"))
        {
            m_PlayerData = new p_PlayerData(1);
            Debug.Log("Player 2 Data Created");
        }

        e_GameEvents.instance.onPlayerNoLives += DestroyPlayer;
    }

    void DestroyPlayer(int playerID)
    {
        if(m_PlayerData.GetPlayerID() == playerID)
        {
            Destroy(gameObject);
        }
    }
}
