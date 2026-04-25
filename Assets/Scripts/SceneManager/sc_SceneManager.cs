using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sc_SceneManager : MonoBehaviour
{

    [Tooltip("The scene name of the loading screen scene.")]
    [SerializeField] private string m_loadingSceneName = "LoadingScene";

    [Tooltip("The duration, in seconds, that the scene manager will keep the loading screen active for after the scene is loaded.")]
    [SerializeField] private float m_waitAfterLoadingInSeconds = 1.0f;
    
    public static sc_SceneManager instance;
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ensure the scene manager is not destroyed between scenes
        }
        else
        {
            Destroy(this);
        }
    }
    
    /// <summary>
    /// A function to load a scene via a scene name
    /// </summary>
    /// <param name="sceneName">The scene name to load</param>
    public static void LoadScene(string sceneName)
    {
        instance.StartCoroutine(instance.LoadSceneCoroutine(sceneName));
    }
    
    /// <summary>
    /// A function to load a scene via a build index
    /// </summary>
    /// <param name="sceneBuildIndex">The build index of the scene to load</param>
    public static void LoadScene(int sceneBuildIndex)
    {
        // convert build index to a scene name
        // via https://discussions.unity.com/t/how-to-get-scene-name-at-certain-buildindex/175723/6
        string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        
        instance.StartCoroutine(instance.LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        Debug.Log("LoadSceneCoroutine: " + sceneName);

        Scene currentScene = SceneManager.GetActiveScene();
        
        // load the loading scene additively and set it as the active scene
        yield return SceneManager.LoadSceneAsync(m_loadingSceneName, LoadSceneMode.Additive);
        
        Scene loadingScene = SceneManager.GetSceneByName(m_loadingSceneName);
        SceneManager.SetActiveScene(loadingScene);
        
        ui_LoadingScreenManager loadingScreenManager = FindFirstObjectByType<ui_LoadingScreenManager>();
        
        // load the target scene additively
        AsyncOperation loadTargetSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!loadTargetSceneOperation.isDone)
        {
            // TODO hook progress visuals
            yield return null;
        }
        
        // activate the target scene
        Scene targetScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(targetScene);
        
        // unload the previous scene if it is still valid
        if (currentScene.IsValid())
        {
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }
        
        // explicitly hide the loading screen
        if (loadingScreenManager != null) 
            loadingScreenManager.Hide();

        
        // wait for the user-specified duration
        yield return new WaitForSeconds(m_waitAfterLoadingInSeconds);
        
        // unload the loading scene
        yield return SceneManager.UnloadSceneAsync(loadingScene);
    }
    
}
