using System.Collections;
using UnityEngine;

public class e_AdrenalineCheck : MonoBehaviour
{
    [Tooltip("This is a multiplier for how much the movement speed increases, if you put 2 it will double the move speed")]
    [SerializeField][Range (1f,5f)] private float m_movementBoost;

    [Tooltip("This is a multiplier for how much the jump force increases, if you put 2 it will double the jump force")]
    [SerializeField][Range (1f,5f)] private float m_jumpBoost;

    [Tooltip("This is how long the timer lasts (in seconds) after the player leaves the adrenaline collider")]
    [SerializeField] private float m_timerLength;

    private p_PlayerPickupManager m_entryPlayerPickupMan;
    private p_PlayerPickupManager m_exitPlayerPickupMan; //in case player are in there at the same time

    private void OnTriggerEnter(Collider other)
    {
        //m_entryPlayerPickupMan ??= other.GetComponentInParent<p_PlayerPickupManager>();

        //if(m_entryPlayerPickupMan == null) { return; }

        ////reset it first 
        //m_entryPlayerPickupMan.ResetMoveSpeed();
        //m_entryPlayerPickupMan.ResetJumpForce();

        //m_entryPlayerPickupMan.AdrenalineBoost(true, m_movementBoost, m_jumpBoost);

        ////m_entryPlayerPickupMan.SetMoveSpeed(m_movementBoost);
        ////m_entryPlayerPickupMan.SetJumpForce(m_jumpBoost);

    }

    private void OnTriggerStay(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) { return; }

        m_entryPlayerPickupMan ??= other.GetComponentInParent<p_PlayerPickupManager>();

        if (m_entryPlayerPickupMan == null) { return; }

        //reset it first 
        m_entryPlayerPickupMan.ResetMoveSpeed();
        m_entryPlayerPickupMan.ResetJumpForce();

        Debug.LogWarning("BOOST", m_entryPlayerPickupMan);
        m_entryPlayerPickupMan.AdrenalineBoost(true, m_movementBoost, m_jumpBoost);
    }

    


    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) { return; }
        m_entryPlayerPickupMan ??= other.GetComponentInParent<p_PlayerPickupManager>();

        if (m_entryPlayerPickupMan == null) { return; }

        m_entryPlayerPickupMan.AdrenalineBoost(false, m_movementBoost, m_jumpBoost);

        //StartCoroutine(C_ResetValuesTimer(m_entryPlayerPickupMan));

    }

    private IEnumerator C_ResetValuesTimer(p_PlayerPickupManager player)
    {
        yield return new WaitForSeconds(m_timerLength);

        player.AdrenalineBoost(false, m_movementBoost, m_jumpBoost);

        //player.ResetMoveSpeed();
        //player.ResetJumpForce();
    }


}
