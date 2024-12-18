using UnityEngine;

public class MirrorController : MonoBehaviour
{
    Transform trans;
    Vector3 offset;
    public CameraController cameraController;

    void Start()
    {
        trans = cameraController.GetCurrentCam().GetComponent<Transform>();
        offset = trans.rotation.eulerAngles - transform.rotation.eulerAngles;
    }

    void Update()
    {
        Quaternion rot = Quaternion.Euler(cameraController.GetCurrentCam().GetComponent<Transform>().rotation.eulerAngles - offset * -1f);
        gameObject.transform.rotation = rot;
    }
}