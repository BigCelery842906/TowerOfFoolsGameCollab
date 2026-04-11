using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource m_backgroundMusic;
    [SerializeField] private AudioSource m_pickupGained;
    [SerializeField] private AudioSource m_pickupSounds;

    private void Awake()
    {
        instance = this;

        if (!m_backgroundMusic.isPlaying) { m_backgroundMusic.Play(); }
    }

    public void PlayAudioClip ( AudioClip sound)
    {
        m_pickupSounds.clip = null;
        m_pickupSounds.clip = sound;

        m_pickupSounds.Play();
    }

    public void PlayPickupCollected()
    {
        if (m_pickupGained.isPlaying) { return; }
        m_pickupGained.Play();
    }
}
