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
        
        transform.rotation = new Quaternion(0,90,0,0);
        Vector3 daggerForce = transform.forward * m_speed;
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
        //TODO:::
        //transform.rotation = Quaternion.LookRotation(transform.forward);


        //Transform dagger = GetComponentInChildren<Transform>();
        while (true)
        {
            //dagger.Rotate(dagger.rotation.x + 25f,  0, 0, Space.World);
            //transform.Rotate(transform.rotation.x + 25f, 0, 0);
            //transform.Rotate(new Vector3(0, 1, 0), 25);
            //transform.rotation = new Quaternion(transform.rotation.x + 25f, 0f, 0f, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator C_LifetimeTimer()
    {
        yield return new WaitForSeconds(m_lifeTime);
        Destroy(gameObject);
    }
}
