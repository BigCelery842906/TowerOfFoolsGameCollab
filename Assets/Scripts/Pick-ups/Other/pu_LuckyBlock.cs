using UnityEngine;

public class pu_LuckyBlock : BasePickup
{
    [SerializeField] private SO_PickupList m_pickupList;

    private void OnTriggerEnter(Collider other)
    {
        int rand = Random.Range(0, m_pickupList.m_pickups.Length);

        GameObject tempObj = Instantiate(m_pickupList.m_pickups[rand]);
        tempObj.transform.position = transform.position;

        Destroy(gameObject);
    }
}
