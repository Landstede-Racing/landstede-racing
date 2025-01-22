using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager
{
    public static IEnumerator LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync(sceneName);

        while (!loadLevel.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync("LoadingScene");
    }
}