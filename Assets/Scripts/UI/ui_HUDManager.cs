using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_HUDManager : ui_BaseMenuManager
{
    
    private VisualElement m_plrOnePowerupWidget;
    private Label m_plrOnePowerupName;
    private Label m_plrOnePowerupDescription;
    private VisualElement m_plrOnePowerupImage;
    
    private VisualElement m_plrTwoPowerupWidget;
    private Label m_plrTwoPowerupName;
    private Label m_plrTwoPowerupDescription;
    private VisualElement m_plrTwoPowerupImage;

    private GameObject m_playerOne;
    private p_PlayerPickupManager m_playerOnePickupManager;
    
    private GameObject m_playerTwo;
    private p_PlayerPickupManager m_playerTwoPickupManager;
    
    protected override void InitialiseMenuManager()
    {
        
        if (m_uiDocument == null) return;
        if (m_uiDocument.rootVisualElement == null) return;
        
        // query and store the UI references
        m_plrOnePowerupWidget = m_uiDocument.rootVisualElement.Q<VisualElement>("plrOne-powerup-widget");
        m_plrOnePowerupName = m_uiDocument.rootVisualElement.Q<Label>("plrOne-powerup-name");
        m_plrOnePowerupDescription = m_uiDocument.rootVisualElement.Q<Label>("plrOne-powerup-description");
        m_plrOnePowerupImage = m_uiDocument.rootVisualElement.Q<VisualElement>("plrOne-powerup-card");
        
        m_plrTwoPowerupWidget = m_uiDocument.rootVisualElement.Q<VisualElement>("plrTwo-powerup-widget");
        m_plrTwoPowerupName = m_uiDocument.rootVisualElement.Q<Label>("plrTwo-powerup-name");
        m_plrTwoPowerupDescription = m_uiDocument.rootVisualElement.Q<Label>("plrTwo-powerup-description");
        m_plrTwoPowerupImage = m_uiDocument.rootVisualElement.Q<VisualElement>("plrTwo-powerup-card");

        // begin a coroutine to wait until the players are ready to be retrieved
        StartCoroutine(PollQueryForPlayers());
    }

    IEnumerator PollQueryForPlayers()
    {
        m_playerOne = e_GlobalData.instance.GetPlayer(0);
        m_playerTwo = e_GlobalData.instance.GetPlayer(1);
        if (m_playerOne == null || m_playerTwo == null)
        {
            // if either player was not found, wait and retry
            yield return new WaitForSeconds(0.05f);
        }
        
        // if both players are found, store their components
        m_playerOnePickupManager = m_playerOne.GetComponent<p_PlayerPickupManager>();
        m_playerTwoPickupManager = m_playerTwo.GetComponent<p_PlayerPickupManager>();
        
        // bind the player events
        m_playerOnePickupManager.OnPickupUsed
    }

    private void HandlePickupUsed_PlayerOne()
    {
        m_plrOnePowerupWidget.style.display = DisplayStyle.None;
    }
    
    private void HandlePickupUsed_PlayerTwo()
    {
        m_plrTwoPowerupWidget.style.display = DisplayStyle.None;
    }
    
    private void HandlePickupEquipped_PlayerOne()
    {
        m_plrOnePowerupName.text = "pu name";
        m_plrOnePowerupDescription.text = "pu desc";
        m_plrOnePowerupImage.style.backgroundImage = null;
        
        m_plrOnePowerupWidget.style.display = DisplayStyle.Flex;
    }
    
    private void HandlePickupEquipped_PlayerTwo()
    {
        m_plrTwoPowerupName.text = "pu name";
        m_plrTwoPowerupDescription.text = "pu desc";
        m_plrTwoPowerupImage.style.backgroundImage = null;
        
        m_plrTwoPowerupWidget.style.display = DisplayStyle.Flex;
    }
    
}
