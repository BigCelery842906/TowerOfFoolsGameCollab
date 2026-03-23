using UnityEngine;
using UnityEngine.SceneManagement;

public static class sc_SceneManager
{

    /// <summary>
    /// A function to load a scene via a scene name
    /// </summary>
    /// <param name="sceneName">The scene name to load</param>
    public static void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// A function to load a scene via a build index
    /// </summary>
    /// <param name="sceneBuildIndex">The build index of the scene to load</param>
    public static void LoadSceneByIndex(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }
    
}
