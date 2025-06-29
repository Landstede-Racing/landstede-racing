using Unity.Netcode;
using UnityEngine;


//TODo fixen dat deze later start || 2 weken late geen idee waarom het werkt gewoon
public class MiniMapController : NetworkBehaviour
{
    public GameObject MiniMapSmallCamera;
    public GameObject MiniMapBigCamera;
    public MiniMap miniMap;
    public RectTransform carDotRectTransform;

    private void Start()
    {
        SetMiniMapSmallCameraActive(true); //TODO change this in to GUI settings
        SetMiniMapBigCameraActive(false);
        miniMap = GameObject.FindGameObjectsWithTag("MiniMap")[0].gameObject.GetComponent<MiniMap>();
    }

    public override void OnNetworkSpawn()
    {
        miniMap = GameObject.FindGameObjectsWithTag("MiniMap")[0].gameObject.GetComponent<MiniMap>();
    }

    public void SetMiniMapSmallCameraActive(bool isActive)
    {
        if (MiniMapSmallCamera != null)
        {
            MiniMapSmallCamera.SetActive(isActive);
            if (isActive)
            {
                carDotRectTransform.sizeDelta = new Vector2(6.9335f, 6.9335f);
                miniMap.SetSize(10f, 10f);
            }
        }
    }

    public void SetMiniMapBigCameraActive(bool isActive)
    {
        if (MiniMapBigCamera != null)
        {
            MiniMapBigCamera.SetActive(isActive);

            if (isActive)
            {
                carDotRectTransform.sizeDelta = new Vector2(60f, 60f);
                miniMap.SetSize(80f, 80f);
            }
        }
    }
}