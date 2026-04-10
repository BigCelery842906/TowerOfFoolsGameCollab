using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class p_PlayerDataManager : MonoBehaviour
{
    //Member Variables
    p_PlayerData m_PlayerData = null;
    int m_PlayerID = -1;

    void Start()
    {
        //This call below is static so needs no instance of player data existing
        //Retrieve what the PlayerID should be based on the gameobject tag, the tag must be in the format (Player(PlayerNumber), E.G: Player1)
        m_PlayerID = p_PlayerData.ReturnPlayerIDFromTag(gameObject.tag);

        //Create a new instance of PlayerData with the ID fed in.
        m_PlayerData = new p_PlayerData(m_PlayerID);

        //Bind the Event for a player losing a life to the update for their position.
        e_GameEvents.instance.onPlayerDeathAdded += PlayerDeathPositionUpdate;

        //Bind the Event for a player having no lives to the end game function.
        e_GameEvents.instance.onPlayerNoLives += EndGame;
    }

    void PlayerDeathPositionUpdate(int playerID)
    {
        if(playerID == m_PlayerID)
        {
            Vector3 currentPos = gameObject.transform.position;
            Vector3 newPos = currentPos;

            newPos.x = currentPos.x + 10;

            gameObject.transform.position = newPos;
        }
    }

    void EndGame(int playerID)
    {
        // Destroy(gameObject);
        sc_SceneManager.LoadSceneByName("GameOver");
        //Save Data
        //Load End Scene
    }
}
