using System;
using UnityEngine;

public class e_Lava : MonoBehaviour
{
    [Tooltip("How fast the lava should rise up the screen - Default is 0.5")]
    [SerializeField] private float m_movementSpeed = 0.5f;

    [Header("Starting Position")]
    [Tooltip("Whether or not the lava should start from it's current position in the editor - This is useful if you have positioned it in a certain way, or are testing something")]
    [SerializeField] private bool m_startFromPosition = false;
    [SerializeField] private float m_startingYPosition = -5f;

    void Start()
    {
        if (!m_startFromPosition)
        {
            transform.position = new Vector3(transform.position.x, m_startingYPosition, transform.position.z);
        }
    }

    void Update()
    {
        transform.position += Vector3.up * (m_movementSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.gameObject.tag == "Player1")
        {
            // "If you finger the lava, you're dead" - Connor Holt 2026
            Debug.Log("Lava Fingered", other.gameObject);
            other.GetComponent<p_Health>().TouchedLava();
        }
    }
}
