using UnityEngine;

public class BasePickup : MonoBehaviour
{
    protected p_PlayerPickupManager m_playerOne;
    protected p_PlayerPickupManager m_playerTwo;

    [Tooltip("Player who collided with the pickup")]
    protected p_PlayerPickupManager m_triggeredPlayer;
    protected p_PlayerPickupManager m_otherPlayer;

    protected CapsuleCollider m_playerCollider;

    private void Awake()
    {
        m_triggeredPlayer = null;

        //grabbing refs to our players
        foreach(p_PlayerPickupManager player in Resources.FindObjectsOfTypeAll(typeof(p_PlayerPickupManager)))
        {
            if(m_playerOne == player || m_playerTwo == player) { continue; } //prevents assigning them / checking tags if the player refs are already assinged

            if (player.gameObject.CompareTag("Player1"))
            {
                m_playerOne = player;

            }
            else
            {
                m_playerTwo = player;
            }
        }        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) { return; } //checks that we are actually colliding with a player, early return if not

        m_triggeredPlayer ??= other.gameObject.GetComponentInParent<p_PlayerPickupManager>();
        m_playerCollider ??= other.GetComponent<CapsuleCollider>();

        if(m_triggeredPlayer == null) { return; } //if it fails to get the component it returns, preventing null ref errors <3

        //player is already holding a pickup and i dont think they need a second one >:c
        if(m_triggeredPlayer.GetPlayerHoldingPickup()) { return; }

        m_triggeredPlayer.OnUseInteractablePickup += InteractedPickupEffect;

        if (m_triggeredPlayer.CompareTag("Player1"))
        {
            //PlayerOne Triggered it
            m_otherPlayer = m_playerTwo;
        }
        else
        {
            //PlayerTwo Triggered it
            m_otherPlayer = m_playerOne;
        }

        m_triggeredPlayer.SetPlayerHoldingPickup(true);
        PickupEffect();     
    }

    /// <summary>
    /// Override in the children, called when a player collides with pickup, call "PickedUp()" at the end, also use this to set it as an interactable pickup
    /// </summary>
    protected virtual void PickupEffect()
    {
        Debug.LogAssertion(gameObject.name + " has no pickup effect assigned and is using the base");
    }

    /// <summary>
    /// Override in children, called from player controller when attack is pressed
    /// </summary>
    protected virtual void InteractedPickupEffect()
    {
        Debug.LogAssertion(gameObject.name + " You have passed this to the PPM as an interactable pickup but you have not overriden the base effect");
    }

    /// <summary>
    /// Override this if needed, base destroys object (if its not interactable) if its interactable it sets its new position
    /// </summary>
    protected virtual void PickedUp()
    {
        if (m_triggeredPlayer.GetPlayerInteractablePickupPPM())
        {
            //shouldnt destroy an interactable pickup
            transform.parent = m_triggeredPlayer.GetPickupPlayerPosPPM();
            transform.localPosition = new Vector3(0,0,0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Should be called after the pickup does its effect, it tells the pickup manager that the players not holding a pickup and deletes the pickup
    /// </summary>
    protected void PickupUsed()
    {
        m_triggeredPlayer.SetPlayerHoldingPickup(false);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if(m_triggeredPlayer != null) { m_triggeredPlayer.OnUseInteractablePickup -= InteractedPickupEffect; } //unbinding
    }
}
