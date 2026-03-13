using System;
using UnityEngine;

public class p_PlayerCombat : MonoBehaviour, IAttackable
{
    [Tooltip("How far away can the player be to attack the other")]
    [SerializeField] private float m_attackRange;

    [Tooltip("How far should the other player be pushed back, 10 is ok?")]
    [SerializeField] private float m_knockbackForce;

    private Rigidbody m_RB;

    private bool m_isShielded;

    private void Awake()
    {
        m_RB = GetComponentInParent<Rigidbody>();
    }

    /// <summary>
    /// Called from input (playerController)
    /// </summary>
    public void Attack()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, m_attackRange))
        {
            IAttackable temp = hit.collider.gameObject.GetComponent<IAttackable>();
            if (temp != null)
            {
                temp.Attacked(hit.point);
            }
        }
    }


    public void Attacked(Vector3 knockbackDir)
    {
        //Changes which direction the player should be pushed in 
        if(knockbackDir.x < transform.position.x)
        {
            m_RB.AddForce(Vector3.right * m_knockbackForce, ForceMode.Impulse);
        }
        else
        {
            m_RB.AddForce(Vector3.left * m_knockbackForce, ForceMode.Impulse);
        }
    }

    public void SetShield(bool shield)
    {
        m_isShielded = shield;
    }
}
