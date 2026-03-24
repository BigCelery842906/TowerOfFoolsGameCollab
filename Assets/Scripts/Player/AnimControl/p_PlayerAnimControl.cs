using UnityEngine;

public class p_playerAnimControl : MonoBehaviour
{
    private Animator m_anim;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
    }

    public void SetAnimMove(bool isMoving)
    {
        m_anim.SetBool("isMoving", isMoving);
    }
}
