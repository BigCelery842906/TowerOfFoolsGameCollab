using System.Collections;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [SerializeField] private float m_lifeTime;
    [SerializeField] private float m_stunTime;
    [SerializeField] private float m_speed;

    private Rigidbody m_RB;

    private void Awake()
    {
        m_RB ??= GetComponent<Rigidbody>();
        
        Vector3 daggerForce = transform.forward * m_speed;
        m_RB.AddForce(daggerForce, ForceMode.Impulse);

        StartCoroutine(C_LifetimeTimer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player1") || !collision.gameObject.CompareTag("Player2")) { Destroy(gameObject); }
        Debug.Log(collision.gameObject.name);

        IAttackable tempAttackable = collision.gameObject.GetComponentInChildren<IAttackable>();

        if (tempAttackable != null)
        {
            tempAttackable.Stun(m_stunTime);
        }
        Destroy(gameObject);
    }    

    private IEnumerator C_LifetimeTimer()
    {
        yield return new WaitForSeconds(m_lifeTime);
        Destroy(gameObject);
    }
}
