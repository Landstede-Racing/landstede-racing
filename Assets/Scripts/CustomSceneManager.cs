using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager
{
    public static IEnumerator LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
        var loadLevel = SceneManager.LoadSceneAsync(sceneName);

        while (!loadLevel.isDone) yield return null;

        SceneManager.UnloadSceneAsync("LoadingScene");
    }
}