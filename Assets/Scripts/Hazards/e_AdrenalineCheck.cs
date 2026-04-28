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
        Debug.Log("triggered");

        m_entryPlayerPickupMan ??= other.GetComponentInParent<p_PlayerPickupManager>();

        if(m_entryPlayerPickupMan == null) { return; }

        m_entryPlayerPickupMan.SetMoveSpeed(m_movementBoost);
        m_entryPlayerPickupMan.SetJumpForce(m_jumpBoost);

    }

    private void OnTriggerExit(Collider other)
    {
        m_exitPlayerPickupMan ??= other.GetComponentInParent<p_PlayerPickupManager>();

        if (m_exitPlayerPickupMan == null) { return; }

        StartCoroutine(C_ResetValuesTimer(m_exitPlayerPickupMan));

    }

    private IEnumerator C_ResetValuesTimer(p_PlayerPickupManager player)
    {
        yield return new WaitForSeconds(m_timerLength);

        Debug.Log("reset");

        player.ResetMoveSpeed();
        player.ResetJumpForce();
    }


}
