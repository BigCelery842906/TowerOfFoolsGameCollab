using UnityEngine;
using UnityEngine.SceneManagement;

public class tempWinCon : MonoBehaviour
{
    [SerializeField] private string m_sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Player")) { return; }

        SceneManager.LoadScene(m_sceneName);
    }
}
