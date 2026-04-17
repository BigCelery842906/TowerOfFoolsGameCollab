using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public abstract class ui_BaseMenuManager : MonoBehaviour
    {
        [Header("UI Settings")]
        [Tooltip("The UI Document for the menu.")]
        [SerializeField] protected UIDocument m_uiDocument;
        
        private Dictionary<Button, Action> m_buttonCallbacks = new Dictionary<Button, Action>();

        protected virtual void Awake()
        {
            // If the UI document is not set, warn about it
            if (m_uiDocument == null)
            {
                throw new UnityException($"UI Document is invalid for: {GetType().Name}");
            }

            InitialiseMenuManager();
        }

        // Initialiser for child classes to implement UI callbacks.
        protected abstract void InitialiseMenuManager();
        
        // Button binding helper to bind a callback to a button
        protected void BindButton(string documentButtonName, Action callback)
        {
            Button button = m_uiDocument.rootVisualElement.Q<Button>(documentButtonName);
            if (button == null)
            {
                throw new UnityException($"UI Document button {documentButtonName} is invalid in: {GetType().Name}");
            }

            // Bind and track the callback
            button.clicked += callback;
            m_buttonCallbacks.Add(button, callback);
        }

        protected virtual void OnDestroy()
        {
            // Clean up the button callbacks
            foreach (var buttonCallback in m_buttonCallbacks)
            {
                if (buttonCallback.Key == null)
                {
                    continue;
                } 
                
                buttonCallback.Key.clicked -= buttonCallback.Value;
            }
            
            // Clear the button callbacks set
            m_buttonCallbacks.Clear();
        }

        public void ShowMenu()
        {
            m_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void HideMenu()
        {
            m_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}