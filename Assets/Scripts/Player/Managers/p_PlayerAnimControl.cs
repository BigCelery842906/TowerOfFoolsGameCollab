using System.Collections;
using UnityEngine;

public class p_playerAnimControl : MonoBehaviour
{
    private Animator m_anim;
    private Rigidbody m_RB;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_RB = GetComponentInParent<Rigidbody>();

        m_anim.SetBool("taunting", false);
    }

    public void SetTauntFloat(float taunt)
    {
        m_anim.SetBool("taunting",true);
        m_anim.SetFloat("Taunt", taunt);

        StartCoroutine(C_WaitForAnim(1, "taunting"));
    }

    public void SetAnimMove(bool isMoving)
    {
        m_anim.SetBool("isMoving", isMoving);
    }

    public void SetAnimJump(float jumpVel)
    {
        m_anim.SetFloat("JumpFloat",jumpVel);
    }

    /// <summary>
    /// [Subject to change] - atm 0 = knife throw, 0.5 = push
    /// </summary>
    /// <param name="pickupValue"></param>
    /// <returns></returns>
    public void SetPickupAnim(float pickupValue)
    {
        if(pickupValue != 0 || pickupValue != 0.5)
        {
            m_anim.SetBool("pickup", false);
        }

        m_anim.SetBool("pickup", true);
        m_anim.SetFloat("pickupFloat", pickupValue);

        StartCoroutine(C_WaitForPickupAnim());
    }

    private IEnumerator C_WaitForPickupAnim()
    {
        yield return new WaitForSeconds(1);

        m_anim.SetBool("pickup", false);
    }

    private IEnumerator C_WaitForAnim(float seconds, string boolName)
    {
        yield return new WaitForSeconds(1);

        m_anim.SetBool("pickup", false);
    }
}
