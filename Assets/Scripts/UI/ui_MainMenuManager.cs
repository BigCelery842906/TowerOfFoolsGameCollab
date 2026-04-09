using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_MainMenuManager : ui_BaseMenuManager
{
    [Header("Level Settings")]
    [SerializeField] private bool m_useSceneBuildIndex;
    
    [Header("Level Settings - Gameplay Scene")]
    [Tooltip("This value will only be taken into account if 'Use Scene Build Index' is true.")]
    [SerializeField] private int m_sceneBuildIndex;
    
    [Tooltip("This value will only be taken into account if 'Use Scene Build Index' is false.")]
    [SerializeField] private string  m_sceneName;

    protected override void InitialiseMenuManager()
    {
        BindButton("play-btn", HandleButtonClicked_Play);
        BindButton("play-scoreboard", HandleButtonClicked_Scoreboard);
        BindButton("play-quit", HandleButtonClicked_Quit);
    }

    private void HandleButtonClicked_Play()
    {
        // Load the scene via build index if that option is selected, if not, load via scene name
        if (m_useSceneBuildIndex)
        {
            sc_SceneManager.LoadSceneByIndex(m_sceneBuildIndex);
            return;
        }
        
        sc_SceneManager.LoadSceneByName(m_sceneName);
    }

    private void HandleButtonClicked_Scoreboard()
    {
        // TODO: Scoreboard button implementation
        Debug.Log("Scoreboard Button Clicked, implementation todo");
    }

    private void HandleButtonClicked_Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
