using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera[] cameras;
    public Camera[] reverseCameras;
    public int currentCam;
    private bool reverse;

    public void SetCamera(int camera)
    {
        if (currentCam >= cameras.Length) camera = cameras.Length - 1;
        cameras[currentCam].gameObject.SetActive(false);
        cameras[camera].gameObject.SetActive(true);
        currentCam = camera;
    }

    public void NextCamera()
    {
        var newCam = currentCam + 1;
        if (newCam >= cameras.Length) newCam = 0;
        SetCamera(newCam);
    }

    public void PreviousCamera()
    {
        var newCam = currentCam - 1;
        if (newCam <= 0) newCam = cameras.Length - 1;
        SetCamera(newCam);
    }

    public void SetReverseCam(bool active)
    {
        reverse = active;
        reverseCameras[currentCam].gameObject.SetActive(active);
        cameras[currentCam].gameObject.SetActive(!active);
    }

    public Camera GetCurrentCam()
    {
        return reverse ? reverseCameras[currentCam] : cameras[currentCam];
    }
}