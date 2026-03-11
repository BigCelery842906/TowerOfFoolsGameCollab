using UnityEngine;

public class JumpBoots : BasePickup
{
    [Tooltip("How many times the player can jump, for double jump set this to 2")]
    [SerializeField] private int m_baseJumps = 1;

    [Tooltip("How many times the player can jump, for double jump set this to 2")]
    [SerializeField] private int m_maxJumps;

    [Tooltip("How long should the player have this effect? (in seconds)")]
    [SerializeField] private int m_effectLength;

    protected override void PickupEffect()
    {
        Debug.Log("boots pickup");

        m_triggeredPlayer.SetPlayerMaxJumps(m_maxJumps, m_baseJumps ,m_effectLength);

        PickedUp();
    }
}
