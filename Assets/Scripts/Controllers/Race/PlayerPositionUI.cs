using System;
using System.Collections;
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
    private bool shouldUpdate = true;
    [SerializeField] private float freezeTime = 3;

    public void UpdateUI(PlayerInfo player)
    {
        if (!position || !shortName || !time || !tire) InitializeTextObjects();
        if (shouldUpdate)
        {
            position.text = player.position.ToString();
            shortName.text = player.shortName.ToString();
            tire.text = player.tire.ToString();

            if (player.position == 1 && player.lap > playerObject.lap)
            {
                // Add lapTime from previous timing + time from new timing to get the total lap time
                TimeSpan newTime = TimeSpan.FromMilliseconds(playerObject.lapTime + player.time);
                time.text = newTime.ToString(@"mm\:ss\.fff");
                StartCoroutine(LeaderNewLap());
            } else
            {
                TimeSpan newTime = TimeSpan.FromMilliseconds(player.lapTime);
                time.text = newTime.ToString(@"mm\:ss\.fff");
            }
        }

        playerObject = player;
    }
    
    public IEnumerator LeaderNewLap()
    {
        shouldUpdate = false;

        yield return new WaitForSecondsRealtime(freezeTime);
        
        shouldUpdate = true;
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