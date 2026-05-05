using System.Collections;
using Unity.VisualScripting;
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
        
        transform.rotation = transform.parent.rotation;
        Vector3 daggerForce = transform.forward * m_speed;
        transform.SetParent(null);
        m_RB.AddForce(daggerForce, ForceMode.Impulse);

        StartCoroutine(C_Rotate());
        StartCoroutine(C_LifetimeTimer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player0") || !collision.gameObject.CompareTag("Player1")) { Destroy(gameObject); }

        IAttackable tempAttackable = collision.gameObject.GetComponentInChildren<IAttackable>();

        if (tempAttackable != null)
        {
            tempAttackable.Stun(m_stunTime);
        }
        Destroy(gameObject);
    }    

    private IEnumerator C_Rotate()
    {
        while (true)
        {
            transform.Rotate(new Vector3(1, 0, 0), 25);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator C_LifetimeTimer()
    {
        yield return new WaitForSeconds(m_lifeTime);
        Destroy(gameObject);
    }
}
