using System.Collections;
using UnityEngine;

public class Marbles : MonoBehaviour
{
    [Tooltip("How long (in seconds) after being spawned do the marbles NOT stun players")]
    [SerializeField] private float m_gracePeriod;
    [SerializeField] private float m_stunTime;

    private Rigidbody m_RB;
    private bool m_canStun;

    private void Awake()
    {
        m_RB ??= GetComponent<Rigidbody>();

        m_canStun = false;

        StartCoroutine(C_GracePeriod());
    }

    private void OnTriggerEnter(Collider other)
    {
        IAttackable tempAttackable = other.gameObject.GetComponentInChildren<IAttackable>();

        if (tempAttackable != null && m_canStun)
        {
            tempAttackable.Stun(m_stunTime);
            Destroy(gameObject);
        }
    }

    private IEnumerator C_GracePeriod()
    {
        yield return new WaitForSeconds(m_gracePeriod);
        m_canStun= true;
    }
}
