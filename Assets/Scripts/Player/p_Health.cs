using UnityEngine;

public class p_Health : MonoBehaviour
{
    private int m_health = 1; // For now this is set to 1
    private int m_maxHealth = 1; // In case we decide to change the health value later
    
    void Start()
    {
        m_health = m_maxHealth;
    }

    public void TouchedLava()
    {
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
