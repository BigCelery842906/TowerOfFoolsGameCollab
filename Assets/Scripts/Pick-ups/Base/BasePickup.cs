using UnityEngine;

public class BasePickup : MonoBehaviour
{
    protected p_PlayerPickupManager m_playerOne;
    protected p_PlayerPickupManager m_playerTwo;

    [Tooltip("Player who collided with the pickup")]
    protected p_PlayerPickupManager m_triggeredPlayer;

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

        m_triggeredPlayer ??= other.GetComponent<p_PlayerPickupManager>();

        PickupEffect();
    }

    /// <summary>
    /// Override in the children, called when a player 
    /// </summary>
    protected virtual void PickupEffect()
    {
        Debug.Log(gameObject.name + " has no pickup effect assigned and is using the base");
    }

    /// <summary>
    /// Override this if needed, base destroys object
    /// </summary>
    protected virtual void PickedUp()
    {
        Destroy(gameObject);
    }
}
