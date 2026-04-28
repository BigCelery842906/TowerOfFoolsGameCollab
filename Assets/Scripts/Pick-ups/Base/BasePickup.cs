using UnityEngine;

public class BasePickup : MonoBehaviour
{
    [Tooltip("Dw bout this <3, dont touch it !!!")]
    [SerializeField] private float pickupAnimFloat;

    [Tooltip("The sprite vfx goes here if its being used by the pickup, not used by default")]
    [SerializeField] private GameObject m_VFXObj;

    protected AudioSource m_pickupSound;

    protected p_PlayerPickupManager m_playerOne;
    protected p_PlayerPickupManager m_playerTwo;

    [Tooltip("Player who collided with the pickup")]
    protected p_PlayerPickupManager m_triggeredPlayer;
    protected p_PlayerPickupManager m_otherPlayer;

    private p_playerAnimControl m_playerAnim;

    protected CapsuleCollider m_playerCollider;

    private bool m_isHeld;

    private void Awake()
    {
        m_triggeredPlayer = null;
        m_isHeld = false;

        //grabbing refs to our players
        foreach(p_PlayerPickupManager player in Resources.FindObjectsOfTypeAll(typeof(p_PlayerPickupManager)))
        {
            if (player.gameObject.CompareTag("Player0"))
            {
                m_playerOne = player;
            }
            else
            {
                m_playerTwo = player;
            }
        }

        m_pickupSound = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_isHeld) { return; }

        //m_triggeredPlayer = null;

        if (!other.gameObject.CompareTag("Player")) { return; } //checks that we are actually colliding with a player, early return if not

        m_triggeredPlayer ??= other.gameObject.GetComponentInParent<p_PlayerPickupManager>();
        m_playerAnim ??= other.gameObject.GetComponent<p_playerAnimControl>();
        m_playerCollider ??= other.GetComponent<CapsuleCollider>();

        if (m_triggeredPlayer == null) { return; } //if it fails to get the component it returns, preventing null ref errors <3

        //player is already holding a pickup and i dont think they need a second one >:c
        if (m_triggeredPlayer.GetPlayerHoldingPickup()) { return; }

        m_triggeredPlayer.OnUseInteractablePickup += InteractedPickupEffect;

        if (m_triggeredPlayer.CompareTag("Player0"))
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
        m_isHeld = true;

        AudioManager.instance.PlayPickupCollected();

        PickupEffect();

    }

    //TODO: decide if i wanna fix this? (probs not worth it but depends what design wants)

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (m_isHeld) { return; }

    //    m_triggeredPlayer = null;

    //    if(!other.gameObject.CompareTag("Player")) { return; } //checks that we are actually colliding with a player, early return if not

    //    m_triggeredPlayer ??= other.gameObject.GetComponentInParent<p_PlayerPickupManager>();
    //    m_playerCollider ??= other.GetComponent<CapsuleCollider>();

    //    if(m_triggeredPlayer == null) { return; } //if it fails to get the component it returns, preventing null ref errors <3

    //    player is already holding a pickup and i dont think they need a second one >:c
    //    if(m_triggeredPlayer.GetPlayerHoldingPickup()) { m_triggeredPlayer.OnPickupUsed += DelayedPickUp; return; }

    //    m_triggeredPlayer.OnUseInteractablePickup += InteractedPickupEffect;

    //    if (m_triggeredPlayer.CompareTag("Player0"))
    //    {
    //        PlayerOne Triggered it
    //        m_otherPlayer = m_playerTwo;
    //    }
    //    else
    //    {
    //        PlayerTwo Triggered it
    //        m_otherPlayer = m_playerOne;
    //    }

    //    m_triggeredPlayer.SetPlayerHoldingPickup(true);
    //    m_isHeld = true;

    //    AudioManager.instance.PlayPickupCollected();

    //    PickupEffect();     
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if(m_isHeld) { return; } 

    //    if(!other.gameObject.CompareTag("Player")) { return; } //checks that we are actually colliding with a player, early return if not

    //    if(other.gameObject != m_triggeredPlayer) { return; } //different player than the one that orignially stepped here

    //    m_triggeredPlayer.OnPickupUsed -= DelayedPickUp;
    //    m_triggeredPlayer = null;

    //}

    private void DelayedPickUp()
    {
        if(m_isHeld) { return; }
        if(m_triggeredPlayer == null) { return;}
        m_triggeredPlayer.OnUseInteractablePickup += InteractedPickupEffect;

        if (m_triggeredPlayer.CompareTag("Player0"))
        {
            //PlayerOne Triggered it
            m_otherPlayer = m_playerTwo;
        }
        else
        {
            //PlayerTwo Triggered it
            m_otherPlayer = m_playerOne;
        }

        m_isHeld = true;
        m_triggeredPlayer.SetPlayerHoldingPickup(true);

        AudioManager.instance.PlayPickupCollected();

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
        if(m_pickupSound != null) { AudioManager.instance.PlayAudioClip(m_pickupSound.clip); }

        m_triggeredPlayer.SetPlayerHoldingPickup(false);
        m_triggeredPlayer.SetIsInteractablePickup(false,this);
        m_triggeredPlayer.UsedPickup();
        m_playerAnim.SetPickupAnim(pickupAnimFloat);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if(m_triggeredPlayer != null) 
        {
            m_triggeredPlayer.OnUseInteractablePickup -= InteractedPickupEffect; 
            m_triggeredPlayer.OnPickupUsed -= DelayedPickUp;
        } //unbinding
    }
}
