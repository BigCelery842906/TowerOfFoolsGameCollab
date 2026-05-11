using UnityEngine;

public class pu_Shield : BasePickup
{
    [SerializeField] private float m_lavaDisplacement;

    protected override void PickupEffect()
    {
        m_triggeredPlayer.SetIsInteractablePickup(true,this);

        AudioManager.instance.PlayAudio("Shield_Equip");

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
        AudioManager.instance.PlayAudio("Shield_Ambient");

        p_PlayerDataManager triggeredPlayerDataManager = m_triggeredPlayer.gameObject.GetComponent<p_PlayerDataManager>();
        if (triggeredPlayerDataManager)
        {
            Debug.Log("Found Player Data Manager");
            triggeredPlayerDataManager.DoRespawnNoTimer();
            AudioManager.instance.PlayAudio("Shield_Break");
        }
        m_triggeredPlayer.OnShieldUsed -= ShieldUsed;
        PickupUsed();
    }
}
