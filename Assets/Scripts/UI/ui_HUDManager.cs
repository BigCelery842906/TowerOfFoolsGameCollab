using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
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

    private Dictionary<int, VisualElement> m_plrOneLivesRemainingWidgets = new Dictionary<int, VisualElement>{};
    private Dictionary<int, VisualElement> m_plrTwoLivesRemainingWidgets = new Dictionary<int, VisualElement>{};

    private GameObject m_playerOne;
    private p_PlayerPickupManager m_playerOnePickupManager;
    private int m_playerOneLivesRemaining = 3;
    
    private GameObject m_playerTwo;
    private p_PlayerPickupManager m_playerTwoPickupManager;
    private int m_playerTwoLivesRemaining = 3;
    
    [Header("Lives UI")]
    [SerializeField] private Texture2D m_lifeLostTexture;
    
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
        
        // query and store the lives remaining widgets
        m_plrOneLivesRemainingWidgets[0] = m_uiDocument.rootVisualElement.Q<VisualElement>("plrOne-health-1");
        m_plrOneLivesRemainingWidgets[1] = m_uiDocument.rootVisualElement.Q<VisualElement>("plrOne-health-2");
        m_plrOneLivesRemainingWidgets[2] = m_uiDocument.rootVisualElement.Q<VisualElement>("plrOne-health-3");
        
        m_plrTwoLivesRemainingWidgets[0] = m_uiDocument.rootVisualElement.Q<VisualElement>("plrTwo-health-1");
        m_plrTwoLivesRemainingWidgets[1] = m_uiDocument.rootVisualElement.Q<VisualElement>("plrTwo-health-2");
        m_plrTwoLivesRemainingWidgets[2] = m_uiDocument.rootVisualElement.Q<VisualElement>("plrTwo-health-3");
    }

    private void Start()
    {
        // begin a coroutine to wait until the players are ready to be retrieved
        StartCoroutine(PollQueryForPlayers());
        
        // call the unequip functions right away to hide visibility and reset widgets
        HandlePickupUsed_PlayerOne();
        HandlePickupUsed_PlayerTwo();
    }

    IEnumerator PollQueryForPlayers()
    {
        while (e_GlobalData.instance == null)
        {
            // if global data was not found, wait and retry
            yield return new WaitForSeconds(0.05f);
        }
        
        GameObject tempPlayerOne = e_GlobalData.instance.GetPlayer(1);
        GameObject tempPlayerTwo = e_GlobalData.instance.GetPlayer(0);
        while (tempPlayerOne == null || tempPlayerTwo == null)
        {
            // if either player was not found, wait and retry
            yield return new WaitForSeconds(0.05f);
            
            tempPlayerOne = e_GlobalData.instance.GetPlayer(1);
            tempPlayerTwo = e_GlobalData.instance.GetPlayer(0);
        }

        int tempPOneID = p_PlayerData.ReturnPlayerIDFromTag(tempPlayerOne.gameObject.tag);
        if (tempPOneID == 0)
        {
            m_playerOne = tempPlayerOne;
            m_playerTwo = tempPlayerTwo;
        }
        else
        {
            m_playerTwo = tempPlayerOne;
            m_playerOne = tempPlayerTwo;
        }
        
        // if both players are found, store their components
        m_playerOnePickupManager = m_playerOne.GetComponent<p_PlayerPickupManager>();
        m_playerTwoPickupManager = m_playerTwo.GetComponent<p_PlayerPickupManager>();
        
        // bind the player events
        m_playerOnePickupManager.OnPickupUsed += HandlePickupUsed_PlayerOne;
        m_playerOnePickupManager.OnNewPickup += HandlePickupEquipped_PlayerOne;
        
        m_playerTwoPickupManager.OnPickupUsed += HandlePickupUsed_PlayerTwo;
        m_playerTwoPickupManager.OnNewPickup += HandlePickupEquipped_PlayerTwo;
        
        // bind the lives lost event
        e_GameEvents.instance.onPlayerLivesUpdate += HandlePlayerLivesChanged;
    }

    private void OnDestroy()
    {
        if (m_playerOnePickupManager != null)
        {
            m_playerOnePickupManager.OnPickupUsed -= HandlePickupUsed_PlayerOne;
            m_playerOnePickupManager.OnNewPickup -= HandlePickupEquipped_PlayerOne;
        }

        if (m_playerTwoPickupManager != null)
        {
            m_playerTwoPickupManager.OnPickupUsed -= HandlePickupUsed_PlayerTwo;
            m_playerTwoPickupManager.OnNewPickup -= HandlePickupEquipped_PlayerTwo;
        }
        
        e_GameEvents.instance.onPlayerLivesUpdate -= HandlePlayerLivesChanged;
        
        base.OnDestroy();
    }

    private void HandlePlayerLivesChanged(int amt, int playerId)
    {
        Dictionary<int, VisualElement> dictToRetrieveFrom = playerId == 0 ? m_plrOneLivesRemainingWidgets : m_plrTwoLivesRemainingWidgets;

        if (playerId == 0)
        {
            m_playerOneLivesRemaining -= 1;
        }
        else
        {
            m_playerTwoLivesRemaining -= 1;
        }
        
        VisualElement widgetToToggle = dictToRetrieveFrom[playerId == 0 ? m_playerOneLivesRemaining : m_playerTwoLivesRemaining];
        widgetToToggle.style.backgroundImage = new StyleBackground(m_lifeLostTexture);
    }
    
    private void HandlePickupUsed_PlayerOne()
    {
        m_plrOnePowerupWidget.style.display = DisplayStyle.None;
    }
    
    private void HandlePickupUsed_PlayerTwo()
    {
        m_plrTwoPowerupWidget.style.display = DisplayStyle.None;
    }
    
    private void HandlePickupEquipped_PlayerOne(int playerId, SO_PickupInfo.PickupInfo pickupInfo)
    {
        m_plrOnePowerupName.text = pickupInfo.Name;
        m_plrOnePowerupDescription.text = pickupInfo.Description;
        m_plrOnePowerupImage.style.backgroundImage = new StyleBackground(pickupInfo.Image);
        
        m_plrOnePowerupWidget.style.display = DisplayStyle.Flex;
    }
    
    private void HandlePickupEquipped_PlayerTwo(int playerId, SO_PickupInfo.PickupInfo pickupInfo)
    {
        m_plrTwoPowerupName.text = pickupInfo.Name;
        m_plrTwoPowerupDescription.text = pickupInfo.Description;
        m_plrTwoPowerupImage.style.backgroundImage = new StyleBackground(pickupInfo.Image);
        
        m_plrTwoPowerupWidget.style.display = DisplayStyle.Flex;
    }
    
}
