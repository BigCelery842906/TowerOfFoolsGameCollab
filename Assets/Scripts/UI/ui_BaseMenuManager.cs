using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UI
{
    public abstract class ui_BaseMenuManager : MonoBehaviour
    {
        [Header("UI Settings")] [Tooltip("The UI Document for the menu.")] [SerializeField]
        protected UIDocument m_uiDocument;

        [Tooltip("Whether the menu is shown or hidden when the scene loads.")] [SerializeField]
        private bool m_menuShownByDefault = false;

        private Dictionary<Button, Action> m_buttonCallbacks = new Dictionary<Button, Action>();

        private bool m_isMenuShown = false;

        protected virtual void Awake()
        {
            // If the UI document is not set, warn about it
            if (m_uiDocument == null)
            {
                throw new UnityException($"UI Document is invalid for: {GetType().Name}");
            }

            if (m_menuShownByDefault)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }

            InitialiseMenuManager();
        }

        // Initialiser for child classes to implement UI callbacks.
        protected abstract void InitialiseMenuManager();

        private Button GetButtonFromStringName(string buttonName)
        {
            // Guard if the document is not available (in a scenario where the gameobject is disabled, etc)
            if (m_uiDocument == null) return null;
            if (m_uiDocument.rootVisualElement == null) return null;
            
            Button button = m_uiDocument.rootVisualElement.Q<Button>(buttonName);
            if (button == null)
            {
                Debug.LogWarning($"UI Document button {buttonName} is invalid in: {GetType().Name}");
                return null;
            }

            return button;
        }

    // Button binding helper to bind a callback to a button
        protected void BindButton(string documentButtonName, Action callback)
        {
            Button button = GetButtonFromStringName(documentButtonName);
            if (button == null) return;

            // Cancel out if this button has already been bound
            if (m_buttonCallbacks.ContainsKey(button)) return;

            // Bind and track the callback
            button.clicked += callback;
            
            // Bind the audio events for the button
            button.clicked += PlayClickSound;
            button.RegisterCallback<PointerEnterEvent>(PlayHoverSound);
            
            m_buttonCallbacks.Add(button, callback);
        }

        protected virtual string GetDefaultFocusButtonName()
        {
            return null;
        }

        private void PlayHoverSound(PointerEnterEvent enterEvent)
        {
            AudioManager.instance.PlayAudio("UI_Hover");
        }

        private void PlayClickSound()
        {
            AudioManager.instance.PlayAudio("UI_Confirm");
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
                
                // Unbind the audio events for the button
                buttonCallback.Key.clicked -= PlayClickSound;
                buttonCallback.Key.UnregisterCallback<PointerEnterEvent>(PlayHoverSound);
            }
            
            // Clear the button callbacks set
            m_buttonCallbacks.Clear();
        }

        public void ShowMenu()
        {
            // get the default focus button name from the overridden member
            string defaultFocusButtonName = GetDefaultFocusButtonName();
            
            // gate the default button focus behind gamepad presence (by checking number of connected gamepads to be more than 0)
            if (defaultFocusButtonName != null && Gamepad.all.Count > 0)
            {
                // if it's not null, attempt to focus the button
                Button button = GetButtonFromStringName(defaultFocusButtonName);
                button?.Focus();
            }
            
            m_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
            m_isMenuShown = true;
        }

        public void HideMenu()
        {
            m_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
            m_isMenuShown = false;
        }

        public bool IsMenuShown()
        {
            return m_isMenuShown;
        }
    }
}