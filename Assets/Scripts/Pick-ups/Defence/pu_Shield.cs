using UnityEngine;

public class pu_Shield : BasePickup
{
    [SerializeField] private float m_lavaDisplacement;

    protected override void PickupEffect()
    {
        m_triggeredPlayer.SetIsInteractablePickup(true,this);

        m_triggeredPlayer.SetPlayerShield(true, m_lavaDisplacement);
        m_triggeredPlayer.OnShieldUsed += ShieldUsed;

        PickedUp();
    }

    protected override void InteractedPickupEffect()
    {
        //nothing 
    }

    private void ShieldUsed()
    {
        m_triggeredPlayer.OnShieldUsed -= ShieldUsed;
        Destroy(gameObject);
    }
}
