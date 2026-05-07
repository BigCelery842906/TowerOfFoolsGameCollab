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
    private DropdownField refreshRateDropdown;
    private List<uint> refreshRateOptions;
    
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
        
        int currentScreenHeight = Screen.currentResolution.height;
        int currentScreenWidth = Screen.currentResolution.width;
        RefreshRate currentScreenRefreshRate = Screen.currentResolution.refreshRateRatio;
        
        for (int i = 0; i < resolutionsOptions.Count; i++)
        {
            Resolution resolution = resolutionsOptions[i]; 
            resolutionsDropdown.choices.Add($"{resolution.width} x {resolution.height}");
            
            if (!refreshRateOptions.Contains(resolution.refreshRateRatio.numerator))
            {
                refreshRateOptions.Add(resolution.refreshRateRatio.numerator);
            }
            
            // attempt to default to current resolution
            if (resolution.width == currentScreenHeight &&
                resolution.height == currentScreenWidth &&
                currentScreenRefreshRate.numerator == resolution.refreshRateRatio.numerator &&
                currentScreenRefreshRate.denominator == resolution.refreshRateRatio.denominator
                )
            {
                resolutionsDropdown.index = i;
                e_GlobalData.instance.m_settingsInfo.resolutionIndex = i;
                
                // Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }
        
        resolutionsDropdown.RegisterValueChangedCallback(evt =>
        {
            uint selectedRefreshRate = refreshRateOptions[refreshRateDropdown.index];
            Resolution selectedResolution = resolutionsOptions[resolutionsDropdown.index];
            e_GlobalData.instance.m_settingsInfo.resolutionIndex = resolutionsDropdown.index;

            RefreshRate newRefreshRate = new RefreshRate();
            newRefreshRate.numerator = selectedRefreshRate;
            
            // Screen.SetResolution(selectedResolution.width, selectedResolution.height, , newRefreshRate);
        });

        PopulateRefreshRateDropdown();
    }

    private void PopulateRefreshRateDropdown()
    {
        refreshRateDropdown = m_uiDocument.rootVisualElement.Q<DropdownField>("resolutions-dropdown");
        refreshRateDropdown.choices.Clear();
        
        RefreshRate currentScreenRefreshRate = Screen.currentResolution.refreshRateRatio;
        
        for (int i = 0; i < refreshRateOptions.Count; i++)
        {
            uint refreshRate = refreshRateOptions[i]; 
            refreshRateDropdown.choices.Add($"{refreshRate}Hz");
            
            // attempt to default to current resolution
            if (refreshRate == currentScreenRefreshRate.numerator)
            {
                refreshRateDropdown.index = i;
                e_GlobalData.instance.m_settingsInfo.refreshRateIndex = i;
                
                // Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }
        
        resolutionsDropdown.RegisterValueChangedCallback(evt =>
        {
            uint selectedRefreshRate = refreshRateOptions[refreshRateDropdown.index];
            Resolution selectedResolution = resolutionsOptions[resolutionsDropdown.index];
            e_GlobalData.instance.m_settingsInfo.refreshRateIndex = refreshRateDropdown.index;
            
            // Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        });
    }
}
