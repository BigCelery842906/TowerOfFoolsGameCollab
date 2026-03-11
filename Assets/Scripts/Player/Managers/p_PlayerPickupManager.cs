using System;
using UnityEngine;

/// <summary>
/// The middle man between the pickups and the player scripts it effects, can also do timer stuff here if we wanna
/// </summary>
public class p_PlayerPickupManager : MonoBehaviour
{
    public event Action<int> OnMaxJumpChange; //for the double jump pick up

    #region Player Scripts
    private p_PlayerMovement m_playerMovement;
    //private p_PlayerCombat m_playerCombat; //might need this later?
    #endregion

    public void SetPlayerMaxJumps(int maxJumps) { OnMaxJumpChange?.Invoke(maxJumps); }

    //TODO: SetPlayerDamageShield
    //Todo: invoke stun

    #region Public Get Functions

    /// <summary>
    /// Returns a bool, Calls the same function within the player movement  
    /// </summary>
    public bool GetPlayerGroundedPPM() { return m_playerMovement.GetPlayerGrouned(); }



    #endregion
}
