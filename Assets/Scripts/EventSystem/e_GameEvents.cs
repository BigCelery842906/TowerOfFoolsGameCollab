using System;
using UnityEngine;

public class e_GameEvents : MonoBehaviour
{

    public static e_GameEvents instance;

    private void Awake()
    {
        instance = this;
    }

    //Bool is true for health up, first int is amount and second int is PlayerID

    //Change this to get rid of bool and take positives or negatives.

    public event Action<bool, int, int> onPlayerHealthUpdate;
    public event Action<bool, int, int> onPlayerScoreUpdate;
    public event Action<bool, int, int> onPlayerLivesUpdate;

    //PlayerID is the int
    public event Action<int> onPlayerDeathAdded;
    public event Action<int> onPlayerNoLives;

    public void PlayerHealthUpdate(bool up, int amount, int playerID)
    {
        if(onPlayerHealthUpdate != null)
        {
            onPlayerHealthUpdate(up, amount, playerID);
        }
    }

    public void PlayerScoreUpdate(bool up, int amount, int playerID)
    {
        if (onPlayerHealthUpdate != null)
        {
            onPlayerScoreUpdate(up, amount, playerID);
        }
    }

    public void PlayerLivesUpdate(bool up, int amount, int playerID)
    {
        if (onPlayerHealthUpdate != null)
        {
            onPlayerLivesUpdate(up, amount, playerID);
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
