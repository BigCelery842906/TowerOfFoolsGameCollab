using System.Collections.Generic;
using UnityEngine;

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

        if (!m_backgroundMusic.isPlaying) { m_backgroundMusic.Play(); }
    }

    public void PlayAudioClip (AudioClip sound)
    {
        m_pickupSounds.clip = null;
        m_pickupSounds.clip = sound;

        m_pickupSounds.Play();
    }

    public void PlayAudio(string name)
    {
        AudioSource source = null;

        for(int i = 0; i < m_Sounds.Count; i++)
        {
            if (m_Sounds[i].Name == name)
            {
                source.clip = m_Sounds[i].Clip;
            }
        }

        if (source != null)
        {
            source.Play();
        }
    }

    public void PlayPickupCollected()
    {
        if (m_pickupGained.isPlaying) { return; }
        m_pickupGained.Play();
    }
}
