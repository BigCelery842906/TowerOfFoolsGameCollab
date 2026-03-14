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
    public event Action<bool, int> onPlayerHealthUpdate;
    public event Action<bool, int> onPlayerScoreUpdate;
    public event Action<bool, int> onPlayerLivesUpdate;

    public event Action onPlayerDeathAdded;

    public void PlayerHealthUpdate(bool up, int amount)
    {
        if(onPlayerHealthUpdate != null)
        {
            onPlayerHealthUpdate(up, amount);
        }
    }

    public void PlayerScoreUpdate(bool up, int amount)
    {
        if (onPlayerHealthUpdate != null)
        {
            onPlayerScoreUpdate(up, amount);
        }
    }

    public void PlayerLivesUpdate(bool up, int amount)
    {
        if (onPlayerHealthUpdate != null)
        {
            onPlayerLivesUpdate(up, amount);
        }
    }

    public void PlayerDeathAdded()
    {
        if(onPlayerDeathAdded != null)
        {
            onPlayerDeathAdded();
        }
    }
}
