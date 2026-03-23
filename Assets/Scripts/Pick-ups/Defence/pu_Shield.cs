using UnityEngine;

public class pu_Shield : BasePickup
{
    [SerializeField] private float m_lavaDisplacement;

    protected override void PickupEffect()
    {
        m_triggeredPlayer.SetIsInteractablePickup(false,this);

        m_triggeredPlayer.SetPlayerShield(true, m_lavaDisplacement);

        PickedUp();
    }
}
