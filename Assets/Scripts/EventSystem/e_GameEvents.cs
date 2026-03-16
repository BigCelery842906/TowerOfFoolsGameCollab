using System;
using UnityEngine;

public class e_GameEvents : MonoBehaviour
{

    public static e_GameEvents instance;

    private void Awake()
    {
        instance = this;
    }

    //Bool is true for health up and int is amount
    public event Action<bool, int, int> onPlayerHealthUpdate;
    public event Action<bool, int, int> onPlayerScoreUpdate;
    public event Action<bool, int, int> onPlayerLivesUpdate;

    public event Action<int> onPlayerDeathAdded;

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
        }
    }
}
