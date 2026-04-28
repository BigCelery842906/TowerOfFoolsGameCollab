using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Written by Connor Saysell

public class e_Lava : MonoBehaviour
{
    [Tooltip("How fast the lava should rise up the screen - Default is 0.5")]
    [SerializeField] private float m_currentMovementSpeed = 0.5f;
    [SerializeField] private float m_baseMovementSpeed = 0.5f;
    [SerializeField] private float m_speedMultiplier = 0.5f;

    [SerializeField] private float m_lavaWaitPeriod = 5.0f;

    [Header("Starting Position")]
    [Tooltip("Whether or not the lava should start from it's current position in the editor - This is useful if you have positioned it in a certain way, or are testing something")]
    [SerializeField] private bool m_startFromPosition = false;
    [SerializeField] private float m_startingYPosition = -5f;
    
    [SerializeField]  e_AdrenalineCheck m_AdrenalineCheck;

    void Start()
    {
        if (!m_startFromPosition)
        {
            transform.position = new Vector3(transform.position.x, m_startingYPosition, transform.position.z);
        }
        StartCoroutine(DelayLavaStartMove());

        if (m_AdrenalineCheck == null)
        {
            m_AdrenalineCheck = GetComponentInChildren<e_AdrenalineCheck>();
        }
        //TODO: DISCUSS WHETHER THIS GETS PUTS IN.
        
        // Vector3 currentScale = transform.localScale; 
        // float scale = e_GlobalData.instance.GetWorldScale();
        // transform.localScale = new Vector3(currentScale.x * scale, currentScale.y * scale, currentScale.z * scale);
    }

    void Update()
    {
        UpdateLavaSpeed();
        transform.position += Vector3.up *  (m_currentMovementSpeed * Time.deltaTime);
    }

    void UpdateLavaSpeed()
    {
        m_currentMovementSpeed = m_baseMovementSpeed + (m_baseMovementSpeed * m_speedMultiplier * e_GlobalData.instance.GetCurrentTimeSpentInGame());
    }

    IEnumerator DelayLavaStartMove()
    {
        float holdSpeed = m_baseMovementSpeed;
        m_baseMovementSpeed = 0;
        
        m_AdrenalineCheck.gameObject.SetActive(false);
        yield return new WaitForSeconds(m_lavaWaitPeriod);
        m_baseMovementSpeed = holdSpeed;
        m_AdrenalineCheck.gameObject.SetActive(true);
        
        e_GlobalData.instance.SetLavaComponents(m_speedMultiplier, m_baseMovementSpeed);
        
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.tag == "Player")
        {
            // "If you finger the lava, you're dead" - Connor Holt 2026
            Debug.Log("Lava Fingered", other.gameObject);
            
            //player touched lava so we should see if they have a shield  //Tyler xx
            p_PlayerPickupManager playerPickupMan = other.gameObject.GetComponentInParent<p_PlayerPickupManager>();
            if (playerPickupMan != null)
            {
                if (playerPickupMan.GetPlayerShield())
                {
                    //player has shield, they should be moved up?

                    Vector3 tempTransfrom = playerPickupMan.gameObject.transform.position; //for readability sake 
                    float offset = playerPickupMan.GetShieldLavaOffset();
                    playerPickupMan.gameObject.transform.position = new Vector3(tempTransfrom.x,tempTransfrom.y + offset,tempTransfrom.z);

                    //players been moved now remove their shield
                    playerPickupMan.SetPlayerShield(false, 0f);

                    return;
                }
            }
            
            //The parent of the gameobject has the player ID within the tag, use this to get the player ID
            int playerID = -1; //default to -1 in case of error
            playerID = p_PlayerData.ReturnPlayerIDFromTag(other.gameObject.transform.parent.tag);

            if (playerID == -1)
            {
                Debug.LogError("Player found but ID not returned", other.gameObject);
                return;
            }
            e_GameEvents.instance.PlayerHealthUpdate(-100, playerID);
        }
    }
}
