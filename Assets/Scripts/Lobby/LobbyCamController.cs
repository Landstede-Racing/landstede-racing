using UnityEngine;

public class LobbyCamController : MonoBehaviour
{
    public Camera screenCamera;
    public Camera garageCamera;
    public Camera[] trackCameras;
    public int currentTrackCam = -1;

    void Start()
    {
        DisableCameras();
        screenCamera.gameObject.SetActive(true);
    }

    public void SetScreenCamera()
    {
        DisableCameras();
        screenCamera.gameObject.SetActive(true);
    }

    public void SetGarageCamera()
    {
        DisableCameras();
        garageCamera.gameObject.SetActive(true);
    }

    public void SetTrackCamera(int camera)
    {
        if (currentTrackCam >= trackCameras.Length) camera = trackCameras.Length - 1;
        DisableCameras();
        trackCameras[camera].gameObject.SetActive(true);
        currentTrackCam = camera;
    }

    public void NextTrackCamera()
    {
        int newCam = currentTrackCam + 1;
        if (newCam >= trackCameras.Length) newCam = 0;
        SetTrackCamera(newCam);
    }

    public void PreviousTrackCamera()
    {
        int newCam = currentTrackCam - 1;
        if (newCam <= 0) newCam = trackCameras.Length - 1;
        SetTrackCamera(newCam);
    }

    private void DisableTrackCameras()
    {
        foreach (Camera camera in trackCameras)
        {
            camera.gameObject.SetActive(false);
        }
        currentTrackCam = -1;
    }

    private void DisableCameras() {
        screenCamera.gameObject.SetActive(false);
        garageCamera.gameObject.SetActive(false);
        DisableTrackCameras();
    }
}