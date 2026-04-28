using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_PauseMenuManager : ui_BaseMenuManager
{
    [Header("Pause Menu UI Components")]
    [SerializeField] private ui_SettingsMenuManager m_settingsMenuManager;
    
    [Header("Level Settings")]
    [SerializeField] private bool m_useSceneBuildIndex;
    
    [Header("Level Settings - Main Menu Scene")]
    [Tooltip("This value will only be taken into account if 'Use Scene Build Index' is true.")]
    [SerializeField] private int m_mainMenuSceneBuildIndex;
    
    [Tooltip("This value will only be taken into account if 'Use Scene Build Index' is false.")]
    [SerializeField] private string  m_mainMenuSceneName;

    private bool m_quitButtonPressed = false;
    
    protected override void InitialiseMenuManager()
    {
        BindButton("continue-btn", HandleButtonClicked_Continue);
        BindButton("settings-btn", HandleButtonClicked_Settings);
        BindButton("quit-btn", HandleButtonClicked_Quit);
    }

    private void Start()
    {
        // bind in start as game events instance may not be available during InitialiseMenuManager (Awake)
        // listen for paused signals to toggle the visibility of the pause menu UI
        e_GameEvents.instance.onPauseToggle += Handle_PausedToggle;
    }

    protected override void OnDestroy()
    {
        // run the parent class OnDestroy method to unbind the BindButton callbacks
        base.OnDestroy();
        
        // unbind the ui visibility from the paused toggle signal
        e_GameEvents.instance.onPauseToggle -= Handle_PausedToggle;
    }

    private void Handle_PausedToggle(bool newPausedState)
    {
        if (newPausedState)
        {
            ShowMenu();
        }
        else
        {
            HideMenu();
            // also hide the settings menu if it's currently shown
            if (m_settingsMenuManager.IsMenuShown()) m_settingsMenuManager.HideMenu();
        }
    }

    private void HandleButtonClicked_Continue()
    {
        HideMenu();
        e_GlobalData.instance.SetPause(false);
    }

    private void HandleButtonClicked_Settings()
    {
        HideMenu();
        m_settingsMenuManager.ShowMenu();
    }
    
    private void HandleButtonClicked_Quit()
    {
        if (m_quitButtonPressed) return;
        m_quitButtonPressed = true;
        
        HideMenu();
        e_GlobalData.instance.SetPause(false);
        
        // Load the scene via build index if that option is selected, if not, load via scene name
        if (m_useSceneBuildIndex)
        {
            sc_SceneManager.LoadScene(m_mainMenuSceneBuildIndex);
            return;
        }
        
        sc_SceneManager.LoadScene(m_mainMenuSceneName);
    }
}
