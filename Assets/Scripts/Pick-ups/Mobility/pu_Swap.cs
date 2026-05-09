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
        //Debug.Log("player = " + m_triggeredPlayer.gameObject.name + "  other : " + m_otherPlayer.gameObject.name);
        if(m_triggeredPlayer == null || m_otherPlayer == null) { BackupCheck(); }
        
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

    /// <summary>
    /// Since players can die and get disabled frequently this does its best to avoid null refs
    /// </summary>
    private void BackupCheck()
    {
        Debug.Log("backup");

        //grabbing refs to our players
        foreach (p_PlayerPickupManager player in Resources.FindObjectsOfTypeAll(typeof(p_PlayerPickupManager)))
        {
            //Debug.Log(player.name);
            //if (player.gameObject.CompareTag("Player0") && !player.gameObject.name.Contains("Variant"))
            if (player.gameObject.name == "Player1") //|| player.gameObject.name == "Player1 Variant 1")
            {
                m_playerOne = player;
            }
            //else if(player.gameObject.CompareTag("Player1") && !player.gameObject.name.Contains("Variant"))
            else if (player.gameObject.name == "Player2") // || player.gameObject.name == "Player2 Variant 1")
            {
                m_playerTwo = player;
            }
        }

        if (m_triggeredPlayer.CompareTag("Player0"))
        {
            //PlayerOne Triggered it
            m_otherPlayer = m_playerTwo;
        }
        else
        {
            //PlayerTwo Triggered it
            m_otherPlayer = m_playerOne;
        }
    }
}
