using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarHorn : MonoBehaviour
{
    public AudioClip hornClip;
    public float fadeOutDuration = 0.1f;

    public float volume = 0.8f; // volume TODO: adjust volume to engine sound in futrue
    private AudioSource hornSource;
    private bool isHornPlaying;

    private void Start()
    {
        hornSource = GetComponent<AudioSource>();
        hornSource.clip = hornClip;
        hornSource.loop = true; // Loopie, doesnt work correctly, for demo xox 
        hornSource.volume = volume;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !isHornPlaying)
        {
            hornSource.volume = volume;
            hornSource.Play();
            isHornPlaying = true;
        }
        else if (Input.GetKeyUp(KeyCode.H) && isHornPlaying)
        {
            StartCoroutine(FadeOutHorn());
            isHornPlaying = false;
        }
    }

    private IEnumerator FadeOutHorn()
    {
        var startVolume = hornSource.volume;
        var timeElapsed = 0f;

        while (timeElapsed < fadeOutDuration)
        {
            timeElapsed += Time.deltaTime;
            hornSource.volume = Mathf.Lerp(startVolume, 0, timeElapsed / fadeOutDuration);
            yield return null;
        }

        hornSource.Stop();
        hornSource.volume = startVolume; // Reset volume for next play
    }
}