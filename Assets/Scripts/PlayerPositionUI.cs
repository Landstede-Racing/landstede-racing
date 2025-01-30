using System;
using LandstedeRacing.Types;
using TMPro;
using UnityEngine;

public class PlayerPositionUI : MonoBehaviour
{
    public TMP_Text position;
    public TMP_Text shortName;
    public TMP_Text time;
    public TMP_Text tire;

    public PlayerInfo playerObject;

    public void UpdateUI(PlayerInfo player)
    {
        playerObject = player;
        if (!position || !shortName || !time || !tire) InitializeTextObjects();
        position.text = player.position.ToString();
        shortName.text = player.shortName;
        time.text = Math.Round(player.time, 3).ToString();
        tire.text = player.tire;
    }

    private void InitializeTextObjects()
    {
        for (var index = 0; index < transform.childCount; index++)
            switch (transform.GetChild(index).tag)
            {
                case "PlayerBarPosition":
                    position = transform.GetChild(index).GetComponent<TextMeshPro>();
                    break;
                case "PlayerBarName":
                    shortName = transform.GetChild(index).GetComponent<TextMeshPro>();
                    break;
                case "PlayerBarTime":
                    time = transform.GetChild(index).GetComponent<TextMeshPro>();
                    break;
                case "PlayerBarTire":
                    tire = transform.GetChild(index).GetComponent<TextMeshPro>();
                    break;
                default:
                    return;
            }
    }
}