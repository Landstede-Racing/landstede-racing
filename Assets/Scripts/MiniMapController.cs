using UnityEngine;


//TODo fixen dat deze later start || 2 weken late geen idee waarom het werkt gewoon
public class MiniMapController : MonoBehaviour
{
    public GameObject MiniMapSmallCamera;
    public GameObject MiniMapBigCamera;
    public MiniMap miniMap;
    // public MiniMap2 miniMap2l;
    public RectTransform carDotRectTransform;

    void Start()
    {

        SetMiniMapSmallCameraActive(true); //TODO change this in to GUI settings
        SetMiniMapBigCameraActive(false);

        // if (carDotRectTransform != null && carDotRectTransform.sizeDelta == new Vector2(6.9335f, 6.9335f))
        // {

        // }
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

    public void SwitchCameras()
    {
        bool isSmallCameraActive = MiniMapSmallCamera.activeSelf;
        SetMiniMapSmallCameraActive(!isSmallCameraActive);
        SetMiniMapBigCameraActive(isSmallCameraActive);
    }
}
