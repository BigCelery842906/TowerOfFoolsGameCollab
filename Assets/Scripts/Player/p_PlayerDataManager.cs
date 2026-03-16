using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class p_PlayerDataManager : MonoBehaviour
{

    p_PlayerData m_PlayerData = null;

    void Start()
    {
        m_PlayerData = new p_PlayerData(p_PlayerData.ReturnPlayerIDFromTag(gameObject.tag));
        
        e_GameEvents.instance.onPlayerNoLives += DestroyPlayer;
    }

    void Update()
    {
    }

    void DestroyPlayer(int playerID)
    {
        //Kill

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
