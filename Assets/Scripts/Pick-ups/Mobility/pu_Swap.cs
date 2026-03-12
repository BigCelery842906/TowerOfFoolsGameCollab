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
        Vector3 tempOtherPlayerTrans = m_otherPlayer.transform.position;

        //Swap their positions
        m_triggeredPlayer.transform.position = tempOtherPlayerTrans;
        m_otherPlayer.transform.position = tempPlayerTrans;

        PickupUsed();
    }
}
