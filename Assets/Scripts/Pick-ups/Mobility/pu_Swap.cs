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


        Debug.Log("player = " + m_triggeredPlayer.gameObject.name + "  other : " + m_otherPlayer.gameObject.name);

        //Grab the positions of each player
        //Vector3 tempPlayerTrans = m_triggeredPlayer.transform.position;
        //Vector3 tempOtherPlayerTrans = m_otherPlayer.transform.position;        

        //tempPlayerTrans = m_triggeredPlayer.transform.position;
        //tempOtherPlayerTrans = m_otherPlayer.transform.position;
        tempPlayerTrans = transform.position;

        int playerID = p_PlayerData.ReturnPlayerIDFromTag(m_triggeredPlayer.tag);
        Debug.Log(playerID);

        tempOtherPlayerTrans = (playerID == 0) ? e_GlobalData.instance.GetPlayerPosition(1) : e_GlobalData.instance.GetPlayerPosition(0);
        tempPlayerTrans = (playerID == 0) ? e_GlobalData.instance.GetPlayerPosition(0) : e_GlobalData.instance.GetPlayerPosition(1);

        Debug.Log(tempPlayerTrans + "player");
        Debug.Log(tempOtherPlayerTrans + "other player");

        //Swap their positions
        //m_otherPlayer.gameObject.transform.position = tempPlayerTrans;
        //m_triggeredPlayer.gameObject.transform.position = tempOtherPlayerTrans;

        m_triggeredPlayer.transform.position = tempPlayerTrans;
        m_otherPlayer.transform.position = tempOtherPlayerTrans;


        Debug.LogAssertion("player = " + m_triggeredPlayer.name + "  other : " + m_otherPlayer.name);

        PickupUsed();
    }
}
