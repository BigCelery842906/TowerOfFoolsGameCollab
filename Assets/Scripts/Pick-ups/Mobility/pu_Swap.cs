using Mono.Cecil.Cil;
using UnityEditor.Rendering;
using UnityEngine;

public class pu_Swap : BasePickup
{
    protected override void PickupEffect()
    {
        m_triggeredPlayer.SetIsInteractablePickup(true,this);

        PickedUp();
    }

    protected override void InteractedPickupEffect()
    {
        //Grab the positions of each player
        Vector3 tempPlayerTrans = m_triggeredPlayer.transform.position;
        Vector3 tempOtherPlayerTrans;

        //We dont know whic player triggered it so check then grabs the position of the other player
        if (m_triggeredPlayer.CompareTag("Player1"))
        {
            //PlayerOne Triggered it
            tempOtherPlayerTrans = m_playerTwo.transform.position;

            m_playerTwo.transform.position = tempPlayerTrans;
            m_triggeredPlayer.transform.position = tempOtherPlayerTrans;
        }
        else 
        {
            //PlayerTwo Triggered it
            tempOtherPlayerTrans = m_playerOne.transform.position;

            m_playerOne.transform.position = tempPlayerTrans;
            m_triggeredPlayer.transform.position = tempOtherPlayerTrans;
        }

        Destroy(gameObject);
    }
}
