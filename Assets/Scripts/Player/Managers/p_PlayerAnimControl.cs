using System.Collections;
using UnityEngine;

public class p_playerAnimControl : MonoBehaviour
{
    private Animator m_anim;
    private Rigidbody m_RB;
    private p_PlayerPickupManager m_playerPickup; //for setting stun

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_RB = GetComponentInParent<Rigidbody>();
        m_playerPickup = GetComponentInParent<p_PlayerPickupManager>();

        m_anim.SetBool("taunting", false);
    }

    public void SetTauntFloat(float taunt)
    {
        StopAllCoroutines();

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
        //transform.parent.rotation = new Quaternion(0, 180, 0, 0);
        m_playerPickup.SetStun(seconds);

        yield return new WaitForSeconds(seconds);

        m_anim.SetBool(boolName, false);
    }
}
