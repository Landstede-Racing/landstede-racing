using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Track {
    public string name;
    public string length;
    public string record;
    public string pRecord;
}

public class TrackSelectionController : MonoBehaviour
{
    public LobbyCamController camController;

    public Button nextButton;
    public Button previousButton;
    public TMP_Text trackName;
    public TMP_Text trackLength;
    public TMP_Text trackRecord;
    public TMP_Text trackPRecord;

    public SceneAsset[] trackScenes;
    private readonly Track[] tracks = new Track[1];

    void Start()
    {
        tracks[0] = new Track
        {
            name = "Stadion",
            length = "0 km",
            record = "0:00.000",
            pRecord = "0:00.000"
        };
    }

    public void StartRace() {
        StartCoroutine(CustomSceneManager.LoadScene(trackScenes[camController.currentTrackCam].name));
    }

    public void UpdateButtons() {
        nextButton.interactable = camController.currentTrackCam != tracks.Length - 1;
        previousButton.interactable = camController.currentTrackCam != 0;
    }

    public void UpdateText() {
        trackName.text = tracks[camController.currentTrackCam].name;
        trackLength.text = tracks[camController.currentTrackCam].length;
        trackRecord.text = tracks[camController.currentTrackCam].record;
        trackPRecord.text = tracks[camController.currentTrackCam].pRecord;
    }

    public void NextTrack()
    {
        camController.NextTrackCamera();
        UpdateButtons();
        UpdateText();
    }

    public void PreviousTrack()
    {
        camController.PreviousTrackCamera();
        UpdateButtons();
        UpdateText();
    }
}