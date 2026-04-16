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
        transform.rotation = new Quaternion(0,90,0,0);

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
            transform.Rotate(0,0, transform.rotation.z + 25f,Space.World);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator C_LifetimeTimer()
    {
        yield return new WaitForSeconds(m_lifeTime);
        Destroy(gameObject);
    }
}
