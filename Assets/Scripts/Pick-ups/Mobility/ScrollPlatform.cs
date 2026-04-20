using System.Collections;
using UnityEngine;

public class ScrollPlatform : MonoBehaviour
{
    [SerializeField] private float m_lifeTime;

    private void Awake()
    {
        StartCoroutine(C_LifetimeTimer());
    }

    private IEnumerator C_LifetimeTimer()
    {
        yield return new WaitForSeconds(m_lifeTime);
        Destroy(gameObject);
    }
}
