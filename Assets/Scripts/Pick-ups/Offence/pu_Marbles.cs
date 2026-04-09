using UnityEngine;

public class pu_marbles : BasePickup
{
    [Tooltip("Put the marble object here, if you want to control lifetime or stun time, thats done on the prefab itself")]
    [SerializeField] private GameObject m_marbelPrefab;

    protected override void PickupEffect()
    {
        m_triggeredPlayer.SetIsInteractablePickup(true, this);

        PickedUp();
    }

    protected override void InteractedPickupEffect()
    {
        Instantiate(m_marbelPrefab, m_triggeredPlayer.GetFiringPlayerPosPPM().position, m_triggeredPlayer.transform.rotation);
        PickupUsed();
    }
}
