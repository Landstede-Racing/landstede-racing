using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishLine : MonoBehaviour
{
    public TMP_Text lapTimeText;
    private float lapTime;
    private bool raceStarted;
    private float toggleCooldown = 2.0f; // Cooldown duration in seconds
    private float lastToggleTime;

    private void Start()
    {
        raceStarted = false;
        lastToggleTime = -toggleCooldown; // Initialize to allow immediate start
    }

    private void Update()
    {
        if (raceStarted)
        {
            lapTime += Time.deltaTime;
            if (lapTime >= 60)
            {
                int minutes = (int)(lapTime / 60);
                float seconds = lapTime % 60;
                lapTimeText.text = string.Format("Lap Time: {0}:{1:F2} minutes", minutes, seconds);
            }
            else
            {
                lapTimeText.text = "Lap Time: " + lapTime.ToString("F2") + " seconds";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastToggleTime >= toggleCooldown)
        {
            raceStarted = !raceStarted;
            lastToggleTime = Time.time;
            // Reset lap time when race starts
            if (raceStarted)
            {
                lapTime = 0;
            }
        }
    }
}