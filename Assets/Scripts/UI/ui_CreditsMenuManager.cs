using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_CreditsMenuManager : ui_BaseMenuManager
{
    [Header("Main Menu UI Components")]
    [SerializeField] private ui_MainMenuManager m_mainMenuManager;
    
    private bool m_quitButtonClicked = false;
    
    protected override void InitialiseMenuManager()
    {
        BindButton("quit-btn", HandleButtonClicked_Quit);
    }
    
    private void HandleButtonClicked_Quit()
    {
        if (m_quitButtonClicked) return;
        m_quitButtonClicked = true;
        
        HideMenu();
        m_mainMenuManager.ShowMenu();

        m_quitButtonClicked = false;
    }
}
