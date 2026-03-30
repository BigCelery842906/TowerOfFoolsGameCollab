using UnityEngine;

[CreateAssetMenu(fileName = "SO_PickupList", menuName = "Scriptable Objects/Pickups/PickupList")]
public class SO_PickupList : ScriptableObject
{
    [Tooltip("This holds a list of all pickups ")]
    public GameObject[] m_pickups;
}
