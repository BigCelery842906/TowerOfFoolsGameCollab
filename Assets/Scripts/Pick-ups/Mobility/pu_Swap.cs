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
        Debug.Log("player = " + m_triggeredPlayer.gameObject.name + "  other : " + m_otherPlayer.gameObject.name);
        
        //Grab the positions of each player
        int playerID = p_PlayerData.ReturnPlayerIDFromTag(m_triggeredPlayer.tag);
        tempOtherPlayerTrans = (playerID == 0) ? e_GlobalData.instance.GetPlayerPosition(1) : e_GlobalData.instance.GetPlayerPosition(0);
        tempPlayerTrans = (playerID == 0) ? e_GlobalData.instance.GetPlayerPosition(0) : e_GlobalData.instance.GetPlayerPosition(1);

        Debug.Log(tempPlayerTrans + "player");
        Debug.Log(tempOtherPlayerTrans + "other player");

        m_triggeredPlayer.transform.position = tempPlayerTrans;
        m_otherPlayer.transform.position = tempOtherPlayerTrans;

        PickupUsed();
    }
}
