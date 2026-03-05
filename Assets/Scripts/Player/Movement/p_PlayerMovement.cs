using System.Collections;
using UnityEngine;

public class p_PlayerMovement : MonoBehaviour
{
    [Tooltip("How fast the player moves, 5 as a base is good")]
    [SerializeField] private float m_moveSpeed;

    [Tooltip("How high the player jumps, 8 feels good?")]
    [SerializeField] private float m_jumpForce;

    [Tooltip("You shouldn't need to touch this, this is where the grounded check for the jump is coming from")]
    [SerializeField] private Transform m_groundCheckTransform; //gives us more control over where the ground check is coming from, should be useful once theres a mesh in the game <3

    private Rigidbody m_RB;

    private Vector2 m_moveDir; //is set based on input 
    private bool m_shouldMove; //bool for stopping the coroutine <3

    private void Awake()
    {
        m_RB = GetComponent<Rigidbody>();
    }

    public void SetMoveDirection(Vector2 direction)
    {
        m_moveDir = direction;

        if(m_moveDir == Vector2.zero)
        {
            m_shouldMove = false;
        }
        else
        {
            m_shouldMove = true;
            StartCoroutine(C_Move());
        }
    }

    /// <summary>
    /// Only runs while the player is holding down a button bound to input, stops once they let go
    /// </summary>
    /// <returns></returns>
    private IEnumerator C_Move()
    {
        while(m_shouldMove)
        {
            Vector3 velocity = new Vector3(m_moveDir.x, 0f, m_moveDir.y) * m_moveSpeed; 
            m_RB.linearVelocity = new Vector3(velocity.x, m_RB.linearVelocity.y, velocity.z); //allows player to keep velocity if jumping/falling

            yield return new WaitForFixedUpdate();
        }
    }

    public void Jump()
    {
        if(Physics.Raycast(m_groundCheckTransform.position, Vector3.down, out RaycastHit hit, 0.3f))
        {
            Debug.Log(hit.collider.name);
            m_RB.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }
    }
}
