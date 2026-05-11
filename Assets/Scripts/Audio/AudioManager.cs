using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Gey

[System.Serializable]
struct AudioData
{
    public AudioClip Clip;
    public string Name;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource m_backgroundMusic;
    [SerializeField] private AudioSource m_pickupGained;
    [SerializeField] private AudioSource m_pickupSounds;
    [SerializeField] private AudioSource m_soundEffectSource;
    
    [SerializeField] private List<AudioData> m_Sounds;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            //If already playing, don't.
            m_backgroundMusic.Stop();
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "MainMenu")
        {
            if (!m_backgroundMusic.isPlaying)
            {
                m_backgroundMusic.Play();
            }
        }
    }

    public void PlayAudioClip (AudioClip sound)
    {
        m_pickupSounds.clip = null;
        m_pickupSounds.clip = sound;

        m_pickupSounds.Play();
    }

    public void PlayAudio(string audioName)
    {
        for(int i = 0; i < m_Sounds.Count; i++)
        {
            if (m_Sounds[i].Name == audioName)
            {
                m_soundEffectSource.PlayOneShot(m_Sounds[i].Clip);
                return;
            }
        }
        
        Debug.LogWarning($"Audio manager: clip '{audioName}' not found");
    }

    public void PlayPickupCollected()
    {
        if (m_pickupGained.isPlaying) { return; }
        m_pickupGained.Play();
    }
}
