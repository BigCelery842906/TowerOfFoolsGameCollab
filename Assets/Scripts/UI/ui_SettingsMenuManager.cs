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
    
    private DropdownField resolutionsDropdown;
    private List<Resolution> resolutionsOptions;
    
    protected override string GetDefaultFocusButtonName() { return "btn-return"; }
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
        
        PopulateSettingsMenu();
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

    private void PopulateSettingsMenu()
    {
        PopulateResolutionDropdown();
    }

    private void PopulateResolutionDropdown()
    {
        resolutionsDropdown = m_uiDocument.rootVisualElement.Q<DropdownField>("resolutions-dropdown");
        resolutionsOptions = new List<Resolution>();

        foreach (var res in Screen.resolutions)
        {
            // only add resolutions that are 16:9
            if (Mathf.Approximately((float)res.width / (float)res.height, 16f / 9f))
            {
                resolutionsOptions.Add(res);
            }
        }

        resolutionsDropdown.choices.Clear();
        bool foundDefaultOption = false;
        
        for (int i = 0; i < resolutionsOptions.Count; i++)
        {
            Resolution resolution = resolutionsOptions[i];
            resolutionsDropdown.choices.Add($"{resolution.width} x {resolution.height} @ {resolution.refreshRateRatio}Hz");

            // attempt to default to 1920 x 1080 if it exists
            if (resolution.width == 1920 && resolution.height == 1080)
            {
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
                resolutionsDropdown.index = i;
                foundDefaultOption = true;
            }
        }

        // if 1920x1080 was not found default to the first available option
        if (!foundDefaultOption)
        {
            if (resolutionsOptions.Count > 0)
            {
                Screen.SetResolution(resolutionsOptions[0].width, resolutionsOptions[0].height, Screen.fullScreen);
            }
        }

        resolutionsDropdown.RegisterValueChangedCallback(evt =>
        {
            Resolution selectedResolution = resolutionsOptions[resolutionsDropdown.index];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        });
    }
}
