using UnityEngine;

public class p_Health : MonoBehaviour
{
    private p_PlayerPickupManager m_pickupManager;

    private int m_health = 1; // For now this is set to 1
    private int m_maxHealth = 1; // In case we decide to change the health value later

    void Start()
    {
        m_pickupManager = GetComponentInParent<p_PlayerPickupManager>();

        m_health = m_maxHealth;
    }

    public void TouchedLava()
    {
        if (m_pickupManager.GetPlayerShield())
        {
            //player has shield ignore one instance of dmg
            //since this is lava the player is teleported up
            Transform temp = m_pickupManager.gameObject.transform; //more readable, this is the parent player obj transform
            temp.position = new Vector3(temp.position.x, temp.position.y + 10f, temp.position.z);

            return;
        }

        Debug.Log("Touched Lava", this);
        m_health--;
        CheckHealth();
    }

    private void CheckHealth()
    {        
        if (m_health <= 0)
        {
            m_health = 0;
            Destroy(transform.parent.gameObject);
        }
        else if (m_health > m_maxHealth)
        {
            m_health = m_maxHealth;
        }
    }    
}
