
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

public class ui_LoadingScreenManager : ui_BaseMenuManager
{

    [SerializeField] private List<Texture2D> m_conceptArtAssets;
    
    protected override void InitialiseMenuManager()
    {
        VisualElement conceptArt = m_uiDocument.rootVisualElement.Q<VisualElement>("concept-art");
        if (conceptArt == null) return;

        conceptArt.style.backgroundImage = new StyleBackground(m_conceptArtAssets[UnityEngine.Random.Range(0, m_conceptArtAssets.Count)]);
    }

    // public void Hide()
    // {
    //     m_uiDocument.panelSettings = null;
    //     m_uiDocument.rootVisualElement?.Clear();
    // }
    
    // TODO implement loading progress
}
