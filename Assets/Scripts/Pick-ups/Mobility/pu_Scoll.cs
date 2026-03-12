using UnityEngine;

public class pu_Scoll : BasePickup
{
    [SerializeField] private GameObject m_platformPrefab;

    [Tooltip("How far below the player should the platform spawn")]
    [SerializeField, Range(1f,4f)] private float m_distaceBelowPlayer = 2;

    protected override void PickupEffect()
    {
        m_triggeredPlayer.SetIsInteractablePickup(true,this);

        PickedUp();
    }

    protected override void InteractedPickupEffect()
    {
        if(m_triggeredPlayer.GetPlayerGroundedPPM()) { return; } //not gonna spawn a platform if the players grounded

        Instantiate(m_platformPrefab, new Vector3(m_triggeredPlayer.transform.position.x, m_triggeredPlayer.transform.position.y - m_distaceBelowPlayer, 0f), m_platformPrefab.transform.rotation);

        Destroy(gameObject);
    }
}
