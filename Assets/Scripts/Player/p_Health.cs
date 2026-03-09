using UnityEngine;

public class p_Health : MonoBehaviour
{
    private int health = 1; // For now this is set to 1
    private int maxHealth = 1; // In case we decide to change the health value later
    
    void Start()
    {
        health = maxHealth;
    }

    public void TouchedLava()
    {
        Debug.Log("Touched Lava", this);
        health--;
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            health = 0;
            Destroy(transform.parent.gameObject);
        }
        else if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
