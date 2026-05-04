using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_SettingsMenuManager : ui_BaseMenuManager
{
    [Header("Settings Menu")]
    [Tooltip("The 'other' menu refers to the menu manager that is returned to via the settings menu's return button.")]
    [SerializeField] private ui_BaseMenuManager m_otherMenuManager;
    
    private List<VisualElement> m_tabPanels = new List<VisualElement>();
    
    protected override void InitialiseMenuManager()
    {
        if (m_uiDocument == null) return;
        if (m_uiDocument.rootVisualElement == null) return;
        
        // Query and hide all the tabs by default
        m_tabPanels = m_uiDocument.rootVisualElement.Query<VisualElement>(className: "tab-panel").ToList();
        foreach (var tabPanel in m_tabPanels)
        {
            tabPanel.style.display = DisplayStyle.None;
        }
        
        BindButton("btn-video", () => SwitchTab("panel-video"));
        BindButton("btn-audio",  () => SwitchTab("panel-audio"));
        BindButton("btn-return", HandleButtonClicked_Return);

        // open the default tab
        SwitchTab("panel-video");
        
    }
    
    void SwitchTab(string tabName)
    {
        bool tabExists = m_tabPanels.Exists(panel => panel.name == tabName);
        if (!tabExists)
        {
            throw new UnityException($"SwitchTab: no panel found with name {tabName}");
            return;
        }
        
        foreach (var tabPanel in m_tabPanels)
        {
            if (tabPanel.name == tabName)
            {
                tabPanel.style.display = DisplayStyle.Flex;
                continue;
            }
            
            tabPanel.style.display = DisplayStyle.None;
        }
    }
    
    void HandleButtonClicked_Return()
    {
        // Hide the current settings menu and show the other menu
        m_otherMenuManager.ShowMenu();
        HideMenu();
    }
}
