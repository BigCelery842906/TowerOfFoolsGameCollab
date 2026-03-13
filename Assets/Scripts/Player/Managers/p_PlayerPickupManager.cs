using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// The middle man between the pickups and the player scripts it effects, can also do timer stuff here if we wanna
/// </summary>
public class p_PlayerPickupManager : MonoBehaviour
{
    public event Action OnUseInteractablePickup; //Invoked when the player tries to attack while holding an interactable pickup, bound to in the pickups to use their effect

    public event Action<int> OnMaxJumpChange; //for the double jump pick up

    #region Player Scripts
    private p_PlayerMovement m_playerMovement;
    #endregion

    [Tooltip("This is where pickups will go to after the player picks them up, unless they are destroyed")]
    [SerializeField] private Transform m_pickupLocation;

    private bool m_hasInteractablePickup = false;
    private BasePickup interactablePickup; //this is sometimes null

    private bool m_hasShield;

    private void Awake()
    {
        m_playerMovement = GetComponent<p_PlayerMovement>();

        m_hasShield = false;
    }

    public void UseInteractablePickup()
    {
        OnUseInteractablePickup?.Invoke();
    }

    //TODO: SetPlayerDamageShield
    //Todo: invoke stun


    #region Public Set Functions

    public void SetPlayerShield(bool shield, float lavaDisplacement)
    {
        m_hasShield = shield;
    }

    /// <summary>
    /// Sets the players max jumps, starts timer then resets the max jumps to their base value
    /// </summary>
    /// <param name="maxJumps"></param>
    /// <param name="baseValue"></param>
    /// <param name="timerLength"></param>
    public void SetPlayerMaxJumps(int maxJumps,int baseValue ,int timerLength)
    { 
        OnMaxJumpChange?.Invoke(maxJumps);
        StartCoroutine(C_Timer(timerLength, baseValue, OnMaxJumpChange));
    }

    public void SetIsInteractablePickup(bool isInteractable, BasePickup pickup)
    {
        m_hasInteractablePickup = isInteractable;
        interactablePickup = pickup;
    }
    


    #endregion


    #region Public Get Functions
    /// <summary>
    /// Returns a bool, Calls the same function within the player movement  
    /// </summary>
    public bool GetPlayerGroundedPPM() { return m_playerMovement.GetPlayerGrouned(); }

    public bool GetPlayerShield() { return m_hasShield; }

    /// <summary>
    /// Returns true if the player has an interactable pickup;
    /// </summary>
    /// <returns></returns>
    public bool GetPlayerInteractablePickupPPM() { return m_hasInteractablePickup; }

    public Transform GetPickupPlayerPosPPM() { return m_pickupLocation; }
    #endregion

    #region Timer(s)
    //super basic dont keep ts
    //Yell at me if i leave this in a commit to main
    private IEnumerator C_Timer(int seconds, int baseValue, Action<int> InvokedActionInt)
    {
        yield return new WaitForSeconds(seconds);
        InvokedActionInt?.Invoke(baseValue);
    }

    #endregion
}
