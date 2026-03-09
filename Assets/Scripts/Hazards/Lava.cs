using System;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [Tooltip("How fast the lava should rise up the screen - Default is 0.5")]
    [SerializeField] private float m_movementSpeed = 0.5f;

    [Header("Starting Position")]
    [Tooltip("Whether or not the lava should start from it's current position in the editor - This is useful if you have positioned it in a certain way, or are testing something")]
    [SerializeField] private bool startFromPosition = false;
    [SerializeField] private float m_startingYPosition = -5f;

    void Start()
    {
        if (!startFromPosition)
        {
            transform.position = new Vector3(transform.position.x, m_startingYPosition, transform.position.z);
        }
    }

    void Update()
    {
        transform.position += Vector3.up * (m_movementSpeed * Time.deltaTime);
    }


    private void OnCollisionEnter(Collision other)
    { // Waiting on Player being pushed to main before testing this aspect
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Hit Player, this is where it would send the kill/damage for player");
        }
    }
}
