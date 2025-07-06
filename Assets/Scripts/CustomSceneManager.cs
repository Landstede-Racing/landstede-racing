using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager
{
    public static IEnumerator LoadScene(string sceneName, bool startMultiplayer = false)
    {
        SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
        var loadLevel = SceneManager.LoadSceneAsync(sceneName);

        while (!loadLevel.isDone) yield return null;

        SceneManager.UnloadSceneAsync("LoadingScene");

        yield return new WaitForSeconds(0.5f);

        if (startMultiplayer)
        {
            EventService.InvokeStartMultiplayer();
        }
    }
}