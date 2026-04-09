using System;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_GameOverMenuManager : ui_BaseMenuManager
{
    [Header("Level Settings")]
    [SerializeField] private bool m_useSceneBuildIndexes;
    
    [Header("Level Settings - Replay Scene")]
    [Tooltip("The gameplay scene build index. This value will only be taken into account if 'Use Scene Build Indexes' is true.")]
    [SerializeField] private int m_replaySceneBuildIndex;
    
    [Tooltip("The gameplay scene name. This value will only be taken into account if 'Use Scene Build Indexes' is false.")]
    [SerializeField] private string  m_replaySceneName;

    [Header("Level Settings - Main Menu Scene")]
    [Tooltip("The main menu scene build index. This value will only be taken into account if 'Use Scene Build Indexes' is true.")]
    [SerializeField] private int m_mainMenuSceneBuildIndex;
    
    [Tooltip("The main menu scene name. This value will only be taken into account if 'Use Scene Build Indexes' is false.")]
    [SerializeField] private string  m_mainMenuSceneName;
    
    protected override void InitialiseMenuManager()
    {
        BindButton("return-btn", HandleButtonClicked_ReturnToMenu);
        BindButton("replay-btn", HandleButtonClicked_Replay);
    }

    private void HandleButtonClicked_ReturnToMenu()
    {
        if (m_useSceneBuildIndexes)
        {
            sc_SceneManager.LoadSceneByIndex(m_mainMenuSceneBuildIndex);
            return;
        }
        
        sc_SceneManager.LoadSceneByName(m_mainMenuSceneName);
    }

    private void HandleButtonClicked_Replay()
    {
        if (m_useSceneBuildIndexes)
        {
            sc_SceneManager.LoadSceneByIndex(m_replaySceneBuildIndex);
            return;
        }
        
        sc_SceneManager.LoadSceneByName(m_replaySceneName);
    }
}
