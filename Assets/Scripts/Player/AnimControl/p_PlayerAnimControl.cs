using UnityEngine;

public class p_playerAnimControl : MonoBehaviour
{
    private Animator m_anim;
    private Rigidbody m_RB;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_RB = GetComponentInParent<Rigidbody>();
    }

    public void SetAnimMove(bool isMoving)
    {
        m_anim.SetBool("isMoving", isMoving);
    }

    public void SetAnimJump(float jumpVel)
    {
        m_anim.SetFloat("JumpFloat",jumpVel);
    }
}
