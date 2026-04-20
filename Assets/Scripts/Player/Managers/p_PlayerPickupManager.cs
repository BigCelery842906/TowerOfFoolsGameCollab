using System;
using System.Collections;
using UnityEngine;

//CRAFTED BY TYLER

/// <summary>
/// The middle man between the pickups and the player scripts it effects, can also do timer stuff here if we wanna
/// </summary>
public class p_PlayerPickupManager : MonoBehaviour
{
    public event Action OnPickupUsed;
    public event Action OnUseInteractablePickup; //Invoked when the player tries to attack while holding an interactable pickup, bound to in the pickups to use their effect

    public event Action OnShieldUsed; 

    public event Action<float> OnMaxJumpChange; //for the double jump pick up
    public event Action<float> OnStunStateChange; //for stun

    #region Player Scripts
    private p_PlayerMovement m_playerMovement;
    #endregion

    [Tooltip("This is where pickups will go to after the player picks them up (if they are interactable :scroll/daggers/swap), unless they are destroyed")]
    [SerializeField] private Transform m_pickupLocation;

    [Tooltip("This is where projectile like daggers will be fired from")]
    [SerializeField] private Transform m_firingPosition;

    private bool m_isHoldingPickup = false;
    private bool m_hasInteractablePickup = false;
    private BasePickup interactablePickup; //this is sometimes null

    private bool m_hasShield = false;
    private float m_shieldLavaDis;

    private float m_baseMoveSpeed;

    private void Awake()
    {
        m_playerMovement = GetComponent<p_PlayerMovement>();
    }

    public void UseInteractablePickup()
    {
        OnUseInteractablePickup?.Invoke();
    }

    public void UsedPickup()
    {
        //broke if i put it in InteractablePickUp
        OnPickupUsed?.Invoke();
    }


    #region Public Set Functions

    /// <summary>
    /// Called after the player picks up a pickup, is checked in base pickup to prevent them picking up two
    /// </summary>
    public void SetPlayerHoldingPickup(bool isHoldingPickup)
    {
        m_isHoldingPickup = isHoldingPickup;

        //just a fail safe x
        if(!m_isHoldingPickup) { m_hasInteractablePickup = false;}
    }

    /// <summary>
    /// So the manager has a accurate copy of what the base move speed should be so it can be set to normal after stun or if a boost is added
    /// </summary>
    public void SetBaseMoveSpeed(float speed) { m_baseMoveSpeed = speed; }

    public void SetPlayerShield(bool shield, float lavaDisplacement)
    {
        m_hasShield = shield;
        m_shieldLavaDis = lavaDisplacement;

        if(!shield)
        {
            OnShieldUsed?.Invoke();
            OnPickupUsed?.Invoke();
        }
    }

    /// <summary>
    /// Sets the players max jumps, starts timer then resets the max jumps to their base value
    /// </summary>
    public void SetPlayerMaxJumps(int maxJumps,int baseValue ,int timerLength)
    { 
        OnMaxJumpChange?.Invoke(maxJumps);
        StartCoroutine(C_Timer(timerLength, baseValue, OnMaxJumpChange));
    }

    /// <summary>
    /// Sets if the pickup the player is currently holding is interactable, if it is the player controlls can pass to it properly 
    /// </summary>
    public void SetIsInteractablePickup(bool isInteractable, BasePickup pickup)
    {
        m_hasInteractablePickup = isInteractable;
        interactablePickup = pickup;
    }
    
    public void SetStun(float timerLength)
    {
        OnStunStateChange?.Invoke(0);
        StartCoroutine(C_Timer(timerLength, m_baseMoveSpeed, OnStunStateChange));
    }

    #endregion


    #region Public Get Functions

    public bool GetPlayerHoldingPickup()
    {
        return m_isHoldingPickup;
    }

    /// <summary>
    /// Returns a bool, Calls the same function within the player movement  
    /// </summary>
    public bool GetPlayerGroundedPPM() { return m_playerMovement.GetPlayerGrouned(); }

    public bool GetPlayerShield() { return m_hasShield; }

    /// <summary>
    /// Gets how high the player should be moved upwards after touching lava while holding shield, distance controlled in shield
    /// </summary>
    public float GetShieldLavaOffset() { return m_shieldLavaDis; }

    /// <summary>
    /// Returns true if the player has an interactable pickup;
    /// </summary>
    /// <returns></returns>
    public bool GetPlayerInteractablePickupPPM() { return m_hasInteractablePickup; }

    public Transform GetPickupPlayerPosPPM() { return m_pickupLocation; }

    public Transform GetFiringPlayerPosPPM() {return m_firingPosition;}
    #endregion

    #region Timer(s)
    //will wait for set seconds then invoke an action passing in the ususal value for it, check out double jumps/player movement to see how this works
    private IEnumerator C_Timer(float seconds, float baseValue, Action<float> InvokedActionInt)
    {
        yield return new WaitForSeconds(seconds);
        InvokedActionInt?.Invoke(baseValue);
    }

    #endregion
}
