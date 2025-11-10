using UnityEngine;
using UnityEngine.Video;

public class IntroController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += EndReached;
    }

    void Update()
    {
        if(Input.anyKey) {
            SkipIntro();
        }
    }

    void EndReached(VideoPlayer vp) {
        SkipIntro();
    }

    public void SkipIntro()
    {
        videoPlayer.Stop();
        StartCoroutine(CustomSceneManager.LoadScene("LobbyScene"));
    }
}