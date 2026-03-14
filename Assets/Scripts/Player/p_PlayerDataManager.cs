using UnityEngine;
using UnityEngine.InputSystem;

public class p_PlayerDataManager : MonoBehaviour
{

    p_PlayerData m_PlayerData = null;

    void Start()
    {
        m_PlayerData = new p_PlayerData();
    }

    void Update()
    {
    }
}
