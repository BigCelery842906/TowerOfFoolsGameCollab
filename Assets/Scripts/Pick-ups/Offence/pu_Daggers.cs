using UnityEngine;

public class pu_Daggers : BasePickup
{
    [Tooltip("Put the dagger object here, if you want to control lifetime/stun time/dagger speed, thats done on the prefab itself")]
    [SerializeField] private GameObject m_daggerPrefab;

    protected override void PickupEffect()
    {
        m_triggeredPlayer.SetIsInteractablePickup(true,this);

        PickedUp();
    }

    protected override void InteractedPickupEffect()
    {
        Instantiate(m_daggerPrefab, m_triggeredPlayer.GetFiringPlayerPosPPM().position, m_triggeredPlayer.transform.rotation);
        PickupUsed();
    }
} 