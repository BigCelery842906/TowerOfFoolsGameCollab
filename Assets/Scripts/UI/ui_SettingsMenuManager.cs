using System;
using System.Collections.Generic;
using System.Net;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_SettingsMenuManager : ui_BaseMenuManager
{
    [Header("Settings Menu")]
    [Tooltip("The 'other' menu refers to the menu manager that is returned to via the settings menu's return button.")]
    [SerializeField] private ui_BaseMenuManager m_otherMenuManager;
    
    private List<VisualElement> m_tabPanels = new List<VisualElement>();
    
    private DropdownField m_resolutionsDropdown;
    private DropdownField m_refreshRateDropdown;
    private DropdownField m_windowModeDropdown;
    private Toggle m_vsyncToggle;
    private Slider m_volumeSlider;
    
    private List<Resolution> m_resolutionsOptions;
    private Dictionary<string, List<uint>> m_refreshRatesPerResolution;
    private Dictionary<string, FullScreenMode> m_fullScreenModeOptions =  new Dictionary<string, FullScreenMode>
    {
        { "Exclusive Fullscreen", FullScreenMode.ExclusiveFullScreen },
        { "Borderless Windowed", FullScreenMode.FullScreenWindow },
        { "Windowed", FullScreenMode.Windowed }
    };
    
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
        m_resolutionsDropdown = m_uiDocument.rootVisualElement.Q<DropdownField>("resolutions-dropdown");
        m_refreshRateDropdown = m_uiDocument.rootVisualElement.Q<DropdownField>("refreshrate-dropdown");
        m_windowModeDropdown = m_uiDocument.rootVisualElement.Q<DropdownField>("windowmode-dropdown");
        m_vsyncToggle = m_uiDocument.rootVisualElement.Q<Toggle>("vsync-toggle");
        m_volumeSlider = m_uiDocument.rootVisualElement.Q<Slider>("volume-slider");

        // resolution & refresh rate
        BuildResolutionData();
        PopulateResolutionDropdown();
        PopulateRefreshRateDropdown(m_resolutionsDropdown.value);
        
        m_resolutionsDropdown.RegisterValueChangedCallback(OnResolutionChanged);
        m_refreshRateDropdown.RegisterValueChangedCallback(OnRefreshRateChanged);

        // vsync
        // "If vSyncCount == 1, rendering is synchronized to the vertical refresh rate of the display."
        bool isVsyncEnabled = QualitySettings.vSyncCount > 0;
        m_vsyncToggle.RegisterValueChangedCallback(OnVsyncChanged);
        m_vsyncToggle.value = isVsyncEnabled;
        e_GlobalData.instance.m_settingsInfo.vsyncEnabled = isVsyncEnabled;

        // window mode
        m_windowModeDropdown.RegisterValueChangedCallback(OnWindowModeChanged);
        PopulateWindowModeDropdown();

        // volume
        m_volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
        m_volumeSlider.value = AudioListener.volume * 100f;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        // unbind all of the callbacks
        m_resolutionsDropdown?.UnregisterValueChangedCallback(OnResolutionChanged);
        m_refreshRateDropdown?.UnregisterValueChangedCallback(OnRefreshRateChanged);
        m_vsyncToggle?.UnregisterValueChangedCallback(OnVsyncChanged);
        m_windowModeDropdown?.UnregisterValueChangedCallback(OnWindowModeChanged);
        m_volumeSlider?.UnregisterValueChangedCallback(OnVolumeChanged);
    }

    private void OnResolutionChanged(ChangeEvent<string> evt)
    {
        PopulateRefreshRateDropdown(evt.newValue);
        ApplyResolution();
    }

    private void OnRefreshRateChanged(ChangeEvent<string> evt)
    {
        ApplyResolution();
    }

    private void OnVsyncChanged(ChangeEvent<bool> evt)
    {
        QualitySettings.vSyncCount = evt.newValue ? 1 : 0;
        e_GlobalData.instance.m_settingsInfo.vsyncEnabled = evt.newValue;

        // when vsync is enabled the frame rate is determined by the display refresh rate, so the
        // refresh rate setting becomes unused, so disable it while vsync is enabled
        m_refreshRateDropdown.SetEnabled(!evt.newValue);
    }

    private void OnWindowModeChanged(ChangeEvent<string> evt)
    {
        FullScreenMode newMode = m_fullScreenModeOptions[evt.newValue];
        e_GlobalData.instance.m_settingsInfo.windowModeIndex = m_windowModeDropdown.index;
        Screen.SetResolution(Screen.width, Screen.height, newMode);
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        AudioListener.volume = evt.newValue / 100f;
    }

    private void PopulateWindowModeDropdown()
    {
        m_windowModeDropdown.choices.Clear();
        m_windowModeDropdown.choices.AddRange(m_fullScreenModeOptions.Keys);
        
        // find the key that matches the current screen mode
        string defaultKey = null;
        foreach (var mode in m_fullScreenModeOptions)
        {
            if (mode.Value == Screen.fullScreenMode)
            {
                defaultKey =  mode.Key;
                break;
            }
        }

        if (defaultKey != null)
        {
            m_windowModeDropdown.value = defaultKey;
        }
        else
        {
            m_windowModeDropdown.value = m_windowModeDropdown.choices[0];
        }

        e_GlobalData.instance.m_settingsInfo.windowModeIndex = m_windowModeDropdown.index;
    }


    private void BuildResolutionData()
    {
        m_resolutionsOptions = new List<Resolution>();
        m_refreshRatesPerResolution = new Dictionary<string, List<uint>>();

        foreach (Resolution res in Screen.resolutions)
        {
            // if it's not a 16:9 ratio, ignore it
            if (!Mathf.Approximately((float)res.width / res.height, 16f / 9f)) continue;

            string resLabel = $"{res.width} x {res.height}";

            // if the refresh rates dictionary does not have the resolution label, add it
            if (!m_refreshRatesPerResolution.ContainsKey(resLabel))
            {
                m_resolutionsOptions.Add(res);
                m_refreshRatesPerResolution[resLabel] = new List<uint>();
            }
            
            // store all the refresh rates for the resolution
            uint hertz = (uint)Math.Round((double)res.refreshRateRatio.numerator / res.refreshRateRatio.denominator);
            m_refreshRatesPerResolution[resLabel].Add(hertz);
        }
    }

    private void PopulateResolutionDropdown()
    {
        m_resolutionsDropdown.choices.Clear();

        Resolution currentRes = Screen.currentResolution;
        int defaultIndex = 0;

        for (int i = 0; i < m_resolutionsOptions.Count; i++)
        {
            Resolution res =  m_resolutionsOptions[i];
            m_resolutionsDropdown.choices.Add($"{res.width} x {res.height}");
            
            // if this is the current resolution
            if (res.width == currentRes.width && res.height == currentRes.height)
                defaultIndex = i;
        }

        m_resolutionsDropdown.index = defaultIndex;
        e_GlobalData.instance.m_settingsInfo.resolutionIndex = defaultIndex;
    }


    private void PopulateRefreshRateDropdown(string resolutionLabel)
    {
        m_refreshRateDropdown.choices.Clear();

        // if the resolution label does not exist, cancel out
        if (!m_refreshRatesPerResolution.ContainsKey(resolutionLabel)) return;
        
        uint hertz = (uint)Math.Round((double)Screen.currentResolution.refreshRateRatio.numerator / Screen.currentResolution.refreshRateRatio.denominator);
        int defaultIndex = 0;

        for (int i = 0; i < m_refreshRatesPerResolution[resolutionLabel].Count; i++)
        {
            m_refreshRateDropdown.choices.Add($"{m_refreshRatesPerResolution[resolutionLabel][i]}Hz");

            if (hertz == m_refreshRatesPerResolution[resolutionLabel][i])
                defaultIndex = i;
        }
        
        m_refreshRateDropdown.index = defaultIndex;
        e_GlobalData.instance.m_settingsInfo.refreshRateIndex = defaultIndex;
    }

    private void ApplyResolution()
    {
        string resLabel = m_resolutionsDropdown.value;

        // if the resolution label does not exist, cancel out
        if (!m_refreshRatesPerResolution.ContainsKey(resLabel)) return;

        Resolution res = m_resolutionsOptions[m_resolutionsDropdown.index];
        uint selectedRefreshRate = m_refreshRatesPerResolution[resLabel][m_refreshRateDropdown.index];
        
        e_GlobalData.instance.m_settingsInfo.resolutionIndex = m_resolutionsDropdown.index;
        e_GlobalData.instance.m_settingsInfo.refreshRateIndex = m_refreshRateDropdown.index;

        RefreshRate newRefreshRate = new RefreshRate
        {
            numerator = selectedRefreshRate, denominator = 1
        };

        FullScreenMode selectedFullScreenMode = m_fullScreenModeOptions[m_windowModeDropdown.value];
        Screen.SetResolution(res.width, res.height, selectedFullScreenMode, newRefreshRate);
    }
}
