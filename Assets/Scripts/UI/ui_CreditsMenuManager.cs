using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_CreditsMenuManager : ui_BaseMenuManager
{
    [Header("Main Menu UI Components")]
    [SerializeField] private ui_MainMenuManager m_mainMenuManager;
    
    protected override string GetDefaultFocusButtonName() { return "quit-btn"; }
    protected override void InitialiseMenuManager()
    {
        BindButton("quit-btn", HandleButtonClicked_Quit);
    }

    private void HandleButtonClicked_Quit()
    {
        HideMenu();
        m_mainMenuManager.ShowMenu();
    }
}
