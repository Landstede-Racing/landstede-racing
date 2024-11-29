using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera[] cameras;
    public Camera reverseCam;
    public int currentCam;

    private void Start() {
        
    }

    private void Update() {
        
    }

    public void SetCamera(int camera) {
        if(currentCam >= cameras.Length) camera = cameras.Length - 1;
        cameras[currentCam].gameObject.SetActive(false);
        cameras[camera].gameObject.SetActive(true);
        currentCam = camera;
    }

    public void NextCamera() {
        int newCam = currentCam + 1;
        if(newCam >= cameras.Length) newCam = 0;
        SetCamera(newCam); 
    }

    public void PreviousCamera() {
        int newCam = currentCam - 1;
        if(newCam <= 0) newCam = cameras.Length - 1;
        SetCamera(newCam);
    }

    public void SetReverseCam(bool active) {
        reverseCam.gameObject.SetActive(active);
        cameras[currentCam].gameObject.SetActive(!active);
    }
}