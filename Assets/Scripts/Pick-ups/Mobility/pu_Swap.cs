using Mono.Cecil.Cil;
using UnityEditor.Rendering;
using UnityEngine;

public class pu_Swap : BasePickup
{
    public Vector3 tempPlayerTrans;
    public Vector3 tempOtherPlayerTrans;


    protected override void PickupEffect()
    {
        m_triggeredPlayer.SetIsInteractablePickup(true,this);

        PickedUp();
    }

    protected override void InteractedPickupEffect()
    {
        //m_triggeredPlayer.transform.position = Vector3.zero;


        Debug.Log("player = " + m_triggeredPlayer.gameObject.name + "  other : " + m_otherPlayer.name);

        //Grab the positions of each player
        //Vector3 tempPlayerTrans = m_triggeredPlayer.transform.position;
        //Vector3 tempOtherPlayerTrans = m_otherPlayer.transform.position;        

        tempPlayerTrans = m_triggeredPlayer.transform.position;
        tempOtherPlayerTrans = m_otherPlayer.transform.position;

        Debug.Log(tempPlayerTrans + "player");
        Debug.Log(tempOtherPlayerTrans + "other player");

        //Swap their positions
        m_otherPlayer.transform.position = tempPlayerTrans;
        m_triggeredPlayer.transform.position = tempOtherPlayerTrans;

        PickupUsed();
    }
}
