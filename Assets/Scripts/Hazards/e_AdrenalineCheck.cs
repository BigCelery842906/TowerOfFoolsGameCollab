using System.Collections.Generic;
using UnityEngine;

public class e_AdrenalineCheck : MonoBehaviour
{
    [Tooltip("This is a multiplier for how much the movement speed increases, if you put 2 it will double the move speed")]
    [SerializeField][Range (1f,5f)] private float m_movementBoost;

    [Tooltip("This is a multiplier for how much the jump force increases, if you put 2 it will double the jump force")]
    [SerializeField][Range (1f,5f)] private float m_jumpBoost;

    [Tooltip("This is how long the timer lasts (in seconds) after the player leaves the adrenaline collider")]
    [SerializeField] private float m_timerLength;

    private List<p_PlayerPickupManager> m_players = new List<p_PlayerPickupManager>();
     

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) { return; }

        p_PlayerPickupManager temp = other.GetComponentInParent<p_PlayerPickupManager>();

        if (temp == null) { return; }

        //see if this player is already in the list
        if (m_players.Contains(temp)) { return; }

        m_players.Add(temp);

        for(int i = 0; i < m_players.Count; i++)
        {
            m_players[i].AdrenalineBoost(true, m_movementBoost, m_jumpBoost, m_timerLength);
        }

        AudioManager.instance.PlayAudio("Lava_Close");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) { return; }

        p_PlayerPickupManager temp = other.GetComponentInParent<p_PlayerPickupManager>();

        if (temp == null) { return; }

        //see if this player is already in the list
        if (m_players.Contains(temp)) { temp.AdrenalineBoost(false, m_movementBoost, m_jumpBoost, m_timerLength); }

        m_players.Remove(temp);
    }
}
