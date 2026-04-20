using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_PauseMenuManager : ui_BaseMenuManager
{
    [Header("Pause Menu UI Components")]
    [SerializeField] private UIDocument m_settingsMenuUiDocument;
    
    [Header("Level Settings")]
    [SerializeField] private bool m_useSceneBuildIndex;
    
    [Header("Level Settings - Main Menu Scene")]
    [Tooltip("This value will only be taken into account if 'Use Scene Build Index' is true.")]
    [SerializeField] private int m_mainMenuSceneBuildIndex;
    
    [Tooltip("This value will only be taken into account if 'Use Scene Build Index' is false.")]
    [SerializeField] private string  m_mainMenuSceneName;
    
    protected override void InitialiseMenuManager()
    {
        m_uiDocument.enabled = false;
        m_settingsMenuUiDocument.enabled = false;
        
        BindButton("continue-btn", HandleButtonClicked_Continue);
        BindButton("settings-btn", HandleButtonClicked_Settings);
        BindButton("quit-btn", HandleButtonClicked_Quit);
    }

    private void HandleButtonClicked_Continue()
    {
        // TODO: continue button
        Debug.Log("continue Button Clicked, implementation todo");
    }

    private void HandleButtonClicked_Settings()
    {
        // Toggle the visibility of the main UI document and the settings UI document
        m_uiDocument.enabled = false;
        m_settingsMenuUiDocument.enabled = true;
    }
    
    private void HandleButtonClicked_Quit()
    {
        // Load the scene via build index if that option is selected, if not, load via scene name
        if (m_useSceneBuildIndex)
        {
            sc_SceneManager.LoadSceneByIndex(m_mainMenuSceneBuildIndex);
            return;
        }
        
        sc_SceneManager.LoadSceneByName(m_mainMenuSceneName);
    }
}
