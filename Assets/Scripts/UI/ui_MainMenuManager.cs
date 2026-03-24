using UnityEngine;
using UnityEngine.UIElements;

public class ui_MainMenuManager : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("The UI Document for the Main Menu.")]
    [SerializeField] private UIDocument m_uiMainMenuDocument;

    [Header("Gameplay Level Settings")]
    [SerializeField] private bool m_useSceneBuildIndex;
    
    [Tooltip("This value will only be taken into account if 'Use Scene Build Index' is true.")]
    [SerializeField] private int m_sceneBuildIndex;
    
    [Tooltip("This value will only be taken into account if 'Use Scene Build Index' is false.")]
    [SerializeField] private string  m_sceneName;

    private Button m_playButton;
    private Button m_scoreboardButton;
    private Button m_quitButton;
    
    void Awake()
    {
        // Check if the UI Document is valid
        if (m_uiMainMenuDocument == null)
        {
            throw new UnityException("ui_MainMenuDocument is null");
        }
        
        // Query the buttons from the UI document, check validity, and set click callbacks
        m_playButton = m_uiMainMenuDocument.rootVisualElement.Q<Button>("play-btn");
        m_scoreboardButton =  m_uiMainMenuDocument.rootVisualElement.Q<Button>("scoreboard-btn");
        m_quitButton = m_uiMainMenuDocument.rootVisualElement.Q<Button>("quit-btn");

        if (m_playButton == null || m_scoreboardButton == null || m_quitButton == null)
        {
            throw new UnityException("ui_MainMenuDocument play or quit button is invalid");
        }

        m_playButton.clicked += HandleButtonClicked_Play;
        m_scoreboardButton.clicked += HandleButtonClicked_Scoreboard;
        m_quitButton.clicked += HandleButtonClicked_Quit;
    }

    private void OnDestroy()
    {
        // Just to be safe, unbind the callbacks when the menu is destroyed
        if (m_playButton != null)
        {
            m_playButton.clicked -= HandleButtonClicked_Play;
        }

        if (m_scoreboardButton != null)
        {
            m_scoreboardButton.clicked -= HandleButtonClicked_Scoreboard;
        }
        
        if (m_quitButton != null)
        {
            m_quitButton.clicked -= HandleButtonClicked_Quit;
        }
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
        
    }

    private void HandleButtonClicked_Quit()
    {
        Application.Quit();
    }
}
