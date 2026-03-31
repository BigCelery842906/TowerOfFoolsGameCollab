using System.Collections;
using UnityEngine;

public class pu_SpringBoots : BasePickup
{
    [SerializeField] private float m_boostTime;

    [SerializeField] private float m_boostStrength;

    [SerializeField] private float m_timeInterval;
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
        m_playerCollider.enabled = false; //turn off their collider so they can go through platforms
        m_RB.useGravity = false; //means we only have to apply force once and itll apply smoothly for the rest of the boost

        m_RB.AddForce(Vector3.up * m_boostStrength, ForceMode.Impulse);

        yield return new WaitForSeconds(m_boostTime);

        m_playerCollider.enabled = true; //turn them colliders/grav back on
        m_RB.useGravity = true;

        //boost used so now we can destroy the boots
        Destroy(gameObject);
    }
}
