using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PickupInfo
{
    public string Name;
    public string Description;
    public Sprite Image;
}

[CreateAssetMenu(fileName = "SO_PickupInfo", menuName = "Scriptable Objects/SO_PickupInfo")]
public class SO_PickupInfo : ScriptableObject
{
    [SerializeField]public PickupInfo[] m_pickups;
}