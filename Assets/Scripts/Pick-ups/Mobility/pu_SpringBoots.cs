using System.Collections;
using UnityEngine;

public class pu_SpringBoots : BasePickup
{
    [SerializeField] private LayerMask m_groundLayer;

    [Tooltip("How long does the player go up for")]
    [SerializeField] private float m_boostTime;

    [Tooltip("How strong the force applied upwards to the player is")]
    [SerializeField] private float m_boostStrength;

    private Rigidbody m_RB;
    private float m_elapsedTime = 0;

    protected override void PickupEffect()
    {
        m_RB = m_triggeredPlayer.GetComponent<Rigidbody>();
        m_triggeredPlayer.SetIsInteractablePickup(true,this);

        PickedUp();
    }

    protected override void InteractedPickupEffect()
    {
        m_elapsedTime = 0;
        if(m_RB != null) { StartCoroutine(C_BoostPlayer()); }

    }

    private IEnumerator C_BoostPlayer()
    {
        m_RB.excludeLayers = m_groundLayer;
        m_RB.useGravity = false; //means we only have to apply force once and itll apply smoothly for the rest of the boost

        m_RB.AddForce(Vector3.up * m_boostStrength, ForceMode.Impulse);

        yield return new WaitForSeconds(m_boostTime);

        m_RB.useGravity = true;
        m_RB.excludeLayers = 0;

        //boost used so now we can destroy the boots
        PickupUsed();
    }
}
