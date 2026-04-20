using System;
using UnityEngine;

public class e_GameEvents : MonoBehaviour
{

    // This script is global and can be accessed from anywhere by calling e_GameEvents.instance and either firing an event 
    // for example by calling e_GameEvents.instance.PlayerDeathAdded(m_PlayerID); or can be used to bind a function in another script
    // to an event by calling the following in awake of the other script e_GameEvents.instance.onPlayerDeathAdded += IncrementDeaths;
    //
    // Ensure to unbind the function bound to an event in the OnDestory method of said script by calling the following as an example
    // e_GameEvents.instance.onPlayerDeathAdded -= IncrementDeaths;

    public static e_GameEvents instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    //first int is amount and second int is PlayerID
    public event Action<int, int> onPlayerHealthUpdate;
    public event Action<int, int> onPlayerScoreUpdate;
    public event Action<int, int> onPlayerLivesUpdate;

    //PlayerID is the int
    public event Action<int> onPlayerDeathAdded;
    public event Action<int> onPlayerNoLives;

    public void PlayerHealthUpdate(int amount, int playerID)
    {
        if(onPlayerHealthUpdate != null)
        {
            onPlayerHealthUpdate(amount, playerID);
        }
    }

    public void PlayerScoreUpdate(int amount, int playerID)
    {
        if (onPlayerHealthUpdate != null)
        {
            onPlayerScoreUpdate(amount, playerID);
        }
    }

    public void PlayerLivesUpdate(int amount, int playerID)
    {
        if (onPlayerHealthUpdate != null)
        {
            onPlayerLivesUpdate(amount, playerID);
        }
    }

    public void PlayerDeathAdded(int playerID)
    {
        if(onPlayerDeathAdded != null)
        {
            onPlayerDeathAdded(playerID);
            Debug.Log("Death Event Fired");
        }
    }

    public void PlayerNoLives(int playerID)
    {
        if (onPlayerNoLives != null)
        {
            onPlayerNoLives(playerID);
            Debug.Log("No Lives Fired");
        }
    }
}
