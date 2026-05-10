using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class m_VideoPlay : MonoBehaviour
{
    //Quick script thrown together by CS for the purpose of loading the video that the uni want us to play
    [SerializeField] private VideoPlayer m_player;

    [SerializeField] private string m_sceneToLoad;
    void Start()
    {
        if (m_player == null)
        {
            GetComponent<VideoPlayer>();
        }
        StartCoroutine(WaitForVideo());
    }

    IEnumerator WaitForVideo()
    {
        yield return new WaitForSecondsRealtime((float)m_player.clip.length);
        SceneManager.LoadScene(m_sceneToLoad);
    }
}