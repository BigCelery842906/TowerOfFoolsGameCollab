using System.Collections;
using UnityEngine;

public class p_PlayerMovement : MonoBehaviour
{
    [Header("Movement + Jumping Variables")]
    [Tooltip("How fast the player moves, 5 as a base is good")]
    [SerializeField] private float m_moveSpeed;

    [Tooltip("How high the player jumps, 9 feels good?")]
    [SerializeField] private float m_jumpForce;

    [Header("Gravity Variables")]
    [Tooltip("This value is how much gravity the player has has when they start jumping, should feel lighter than the higher gravity")]
    [SerializeField] private float m_lowerGravValue;

    [Tooltip("This value is how much gravity the player has a the apex of their jump, it should be lower than the other gravity values")]
    [SerializeField] private float m_apexGravValue;

    [Tooltip("This value is how much gravity the player feels when falling or when they stop their jump")]
    [SerializeField] private float m_highGravValue;

    [Tooltip("You shouldn't need to touch this, this is where the grounded check for the jump is coming from")]
    [SerializeField] private Transform m_groundCheckTransform; //gives us more control over where the ground check is coming from, should be useful once theres a mesh in the game <3

    [Tooltip("Oh my you put the ground layer here, it should say ground :D")]
    [SerializeField] private LayerMask m_groundLayer;

    private p_PlayerPickupManager m_PlayerPickupManager;
    private Rigidbody m_RB;
    private CapsuleCollider m_CapsuleCollider;

    private float m_dynamicFriction; //dont set this here do it in the physics material
    private float m_staticFriction; //dont set this here do it in the physics material

    private Vector2 m_moveDir; //is set based on input 
    private bool m_shouldMove; //bool for stopping the movement coroutine <3

    private int m_maxJumps; //set in pc at start to one, then controlled by pickups through pc
    private int m_usedJumps; //inc for each jump player maxs until reaches max
    private bool m_isGrounded; //bool for stopping the grounded check (is also set to true in the grounded check)
    private Vector3 m_lowGrav;    
    private Vector3 m_apexGrav;    
    private Vector3 m_highGrav;    

    private void Awake()
    {
        m_PlayerPickupManager = GetComponentInParent<p_PlayerPickupManager>();
        if(m_PlayerPickupManager != null) { m_PlayerPickupManager.OnMaxJumpChange += SetMaxJumps; }

        m_RB = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponentInChildren<CapsuleCollider>();

        //getting the intended values
        m_dynamicFriction = m_CapsuleCollider.material.dynamicFriction;
        m_staticFriction = m_CapsuleCollider.material.staticFriction;

        //I didnt want to expose vectors to the designers ill be real xx You can change this
        m_lowGrav = new Vector3(0f, m_lowerGravValue, 0f);
        m_apexGrav = new Vector3(0f,m_apexGravValue, 0f);
        m_highGrav = new Vector3(0f,m_highGravValue, 0f);
    }

    public void SetMoveDirection(Vector2 direction)
    {
        m_moveDir = new Vector2(direction.x, 0);

        if(m_moveDir == Vector2.zero)
        {
            m_shouldMove = false;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(m_moveDir);

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

    #region Jump Stuff
    //This does normal jump stuff based on input but also handels variable jump height where the player falls faster once they let go of space 
    //The player also hangs at their apex for a little bit longer than normal too

    //All done for game feel since this is a platformer
    public void Jump()
    {
        if(Physics.Raycast(m_groundCheckTransform.position, Vector3.down, out RaycastHit hit, 0.3f) || m_usedJumps < m_maxJumps)
        {
            m_RB.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            m_isGrounded = false;
            Physics.gravity = m_lowGrav;

            m_usedJumps++;

            StartCoroutine(C_GroundedCheck());
        }
    }

    public void JumpCancelled()
    {
        Physics.gravity = m_highGrav;
    }

    //runs after the player jumps 
    private IEnumerator C_GroundedCheck()
    {
        //Having this set to 0 means the player cant run into a wall while jumping and get stuck
        m_CapsuleCollider.material.dynamicFriction = 0;
        m_CapsuleCollider.material.staticFriction = 0;

        while (!m_isGrounded)
        {
            yield return new WaitForSeconds(0.1f); //delays it so its not insta set to true whoops            

            if (Physics.Raycast(m_groundCheckTransform.position, Vector3.down, out RaycastHit hit, 0.3f, m_groundLayer))
            {
                m_isGrounded = true;

                m_CapsuleCollider.material.dynamicFriction = m_dynamicFriction;
                m_CapsuleCollider.material.staticFriction = m_staticFriction;

                m_usedJumps = 0;

                yield return new WaitForFixedUpdate();
                //the coroutine is exited now since the bool is now true
            }            

            //the peak of the jump so the player can hang in mid air for a second (a forgiveness mechanic)
            if (m_RB.linearVelocity.y < 1f && m_RB.linearVelocity.y > 0f)
            {
                Physics.gravity = m_apexGrav;
            }

            if (m_RB.linearVelocity.y < 0f)
            {
                Physics.gravity = m_highGrav; //fixes an edge case where the player could hold jump then fall off ledges with lower gravity (IDK y they would do this but they can't now at least)                
            }
        }
    }
    private void SetMaxJumps(int max) { m_maxJumps = max;}


    public bool GetPlayerGrouned() { return m_isGrounded; } 
    #endregion
}
