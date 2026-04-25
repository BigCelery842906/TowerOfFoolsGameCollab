using System.Collections;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class p_PlayerDataManager : MonoBehaviour
{
    //Member Variables
    p_PlayerData m_PlayerData = null;
    int m_PlayerID = -1;

    private float m_PlayerScale = 1.0f;
    
    [Header("Death Reset Values")]
    [SerializeField] private float m_radius = 10.0f;
    [SerializeField] private float m_deathPositionCorrection = 10.0f;
    [SerializeField] private bool m_drawDeathReset = false;
    [SerializeField] private float m_respawnTimer = 3.0f;
    void Start()
    {
        //This call below is static so needs no instance of player data existing
        //Retrieve what the PlayerID should be based on the gameobject tag, the tag must be in the format (Player(PlayerNumber), E.G: Player1)
        m_PlayerID = p_PlayerData.ReturnPlayerIDFromTag(gameObject.tag);

        //Create a new instance of PlayerData with the ID fed in.
        m_PlayerData = new p_PlayerData(m_PlayerID);

        m_PlayerScale = e_GlobalData.instance.GetPlayerScale();
        transform.localScale = new Vector3(m_PlayerScale, m_PlayerScale, m_PlayerScale);
        //Bind the Event for a player losing a life to the update for their position.
        e_GameEvents.instance.onPlayerDeathAdded += PlayerDeathPositionUpdate;

        //Bind the Event for a player having no lives to the end game function.
        e_GameEvents.instance.onPlayerNoLives += EndGame;
    }

    void PlayerDeathPositionUpdate(int playerID)
    {
        if (playerID == m_PlayerID)
        {
            //Don't even ask. I need an active instance of monobehaviour otherwise the player won't do the reset to active.
            MonoBehaviour camMono = Camera.main.GetComponent<MonoBehaviour>();
            camMono.StartCoroutine(RespawnTimer());
            // StartCoroutine(RespawnTimer());
        }
    }

    IEnumerator RespawnTimer()
        {
            gameObject.SetActive(false);
            yield return new WaitForSeconds(m_respawnTimer);

            // Written by Connor, shout if you need to 
            gameObject.SetActive(true);
            Vector3 currentPos = gameObject.transform.position;
            Vector3 newPos = currentPos;

            newPos.y = currentPos.y + m_deathPositionCorrection;

            Collider[] potentialPlatforms = Physics.OverlapSphere(newPos, m_radius); //Get all objects in range that have a collider
            List<GameObject> platforms = new List<GameObject>(); // Make a gameobject list (You need the transforms not the collider component now), and lists are just easier to add to
            float closestDistance = float.MaxValue; // Reset to large value
            int closestPlatformID = -1;
            foreach (Collider platform in potentialPlatforms)
            {
                if (platform.gameObject.CompareTag("PlatformEnd") || platform.gameObject.CompareTag("PlatformMiddle"))
                {
                    //Platforms only have these 2 tags - Can extend to include floors if needed
                    platforms.Add(platform.gameObject);
                }
            }

            for (int i = 0; i < platforms.Count; i++)
            {
                float distance = (platforms[i].transform.position - newPos).magnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlatformID = i;
                }
            }

            if (closestPlatformID != -1) //If valid platform
            {
                newPos = platforms[closestPlatformID].transform.position;
                newPos.y += 2;
            }

            gameObject.transform.position = newPos;
        }

    void EndGame(int playerID)
    {
        // Destroy(gameObject);
        sc_SceneManager.LoadSceneByName("GameOver");
        //Save Data
        //Load End Scene
    }
    
    private void OnDrawGizmos()
    {
        if (m_drawDeathReset)
        {
            Vector3 newPos = gameObject.transform.position;
            newPos.y += m_deathPositionCorrection;
            Gizmos.DrawWireSphere(newPos, m_radius);
        }
    }
}
