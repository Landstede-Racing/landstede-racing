using System;
using TMPro;
using UnityEngine;

public class PlayerPositionUI : MonoBehaviour
{
    public TMP_Text _position;
    public TMP_Text _shortName;
    public TMP_Text _time;
    public TMP_Text _tire;

    public PlayerStats playerObject;

    public void UpdateUI(PlayerStats player)
    {
        playerObject = player;
        if (!_position || !_shortName || !_time || !_tire) InitializeTextObjects();
        _position.text = player.position.ToString();
        _shortName.text = player.shortName;
        _time.text = Math.Round((player.time), 3).ToString();
        _tire.text = player.tire;
    }

    private void InitializeTextObjects()
    {
        for (var index = 0; index < transform.childCount; index++)
        {
            switch (transform.GetChild(index).tag)
            {
                case "PlayerBarPosition":
                    _position = transform.GetChild(index).GetComponent<TextMeshPro>();
                    break;
                case "PlayerBarName":
                    _shortName = transform.GetChild(index).GetComponent<TextMeshPro>();
                    break;
                case "PlayerBarTime":
                    _time = transform.GetChild(index).GetComponent<TextMeshPro>();
                    break;
                case "PlayerBarTire":
                    _tire = transform.GetChild(index).GetComponent<TextMeshPro>();
                    break;
                default:
                    return;
            }
        }
    }
}
