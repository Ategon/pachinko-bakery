using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private SpriteRenderer backgroundNight;

    [Header("Variables")]
    [SerializeField] private float minuteLength;

    void FixedUpdate()
    {
        int hour = 8;
        int mins = 0;
        bool pm = false;

        mins = (int) Mathf.Floor(gameplayManager.LevelTimer / minuteLength) % 6;
        hour += (int) (Mathf.Floor(gameplayManager.LevelTimer / minuteLength) - mins) / 6;

        mins *= 10;
        string minString = mins.ToString("00"); //needed for 00 edge case

        if (hour > 12)
        {
            hour -= 12;
            pm = true;
        }

        timerText.text = $"{hour}:{minString} {(pm ? "pm" : "am")}";
        if (gameplayManager.LevelTimer > gameplayManager.LevelLength / 2) backgroundNight.color = new Color(1f, 1f, 1f, (gameplayManager.LevelTimer - gameplayManager.LevelLength / 2) * 2 / gameplayManager.LevelLength);
        else backgroundNight.color = new Color(1f, 1f, 1f, 0f);

    }
}
