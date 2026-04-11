using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource m_backgroundMusic;
    [SerializeField] private AudioSource m_pickupSound;
    [SerializeField] private AudioSource m_pickupUsed;

    private void Awake()
    {
        instance = this;

        if (!m_backgroundMusic.isPlaying) { m_backgroundMusic.Play(); }
    }

    public void PlayPickupCollected()
    {
        if (m_pickupSound.isPlaying) { return; }
        m_pickupSound.Play();
    }

    public void PlayPickupUsed()
    {
        if (m_pickupUsed.isPlaying) { return; }
        m_pickupUsed.Play();
    } 
}
