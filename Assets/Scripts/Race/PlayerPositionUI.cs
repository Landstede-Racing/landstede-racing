using System;
using LandstedeRacing.Types;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerPositionUI : MonoBehaviour
{
    public TMP_Text position;
    public TMP_Text shortName;
    public TMP_Text gapText;
    public TMP_Text tire;

    public PlayerInfo playerObject;

    public void UpdateUI(PlayerInfo player)
    {
        playerObject = player;
        if (!position || !shortName || !gapText || !tire) InitializeTextObjects();
        position.text = "Test";
        position.text = player.position.ToString();
        shortName.text = player.shortName.ToString();

        gapText.text = player.position == 1
            ? "Leader"
            : $"+{Math.Round(player.gapToFront / 1000, 3)}s";

        tire.text = player.tire.ToString();
    }

    private void InitializeTextObjects()
    {
        for (var index = 0; index < transform.childCount; index++)
            switch (transform.GetChild(index).tag)
            {
                case "PlayerBarPosition":
                    position = transform.GetChild(index).GetComponent<TMP_Text>();
                    break;
                case "PlayerBarName":
                    shortName = transform.GetChild(index).GetComponent<TMP_Text>();
                    break;
                case "PlayerBarTime":
                    gapText = transform.GetChild(index).GetComponent<TMP_Text>();
                    break;
                case "PlayerBarTire":
                    tire = transform.GetChild(index).GetComponent<TMP_Text>();
                    break;
                default:
                    return;
            }
    }
}