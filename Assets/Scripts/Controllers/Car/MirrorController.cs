using Unity.Netcode;
using UnityEngine;

public class MirrorController : NetworkBehaviour
{
    public CameraController cameraController;
    private Vector3 offset;
    private Transform trans;

    private void Start()
    {
        trans = cameraController.GetCurrentCam().GetComponent<Transform>();
        offset = trans.rotation.eulerAngles - transform.rotation.eulerAngles;
    }

    private void Update()
    {
        if (IsServer) return;
        var rot = Quaternion.Euler(cameraController.GetCurrentCam().GetComponent<Transform>().rotation.eulerAngles -
                                   offset * -1f);
        gameObject.transform.rotation = rot;
    }
}