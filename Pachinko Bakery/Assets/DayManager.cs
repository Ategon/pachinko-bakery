using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private TextMeshProUGUI dayText;

    void Start()
    {
        GameplayManager.OnDayStart += UpdateDay;
    }

    void UpdateDay()
    {
        switch (gameplayManager.DayNumber)
        {
            case 1:
                dayText.text = "Monday";
                break;
            case 2:
                dayText.text = "Tuesday";
                break;
            case 3:
                dayText.text = "Wednesday";
                break;
            case 4:
                dayText.text = "Thursday";
                break;
            case 5:
                dayText.text = "Friday";
                break;
            default:
                dayText.text = "---";
                break;
        }
    }

    private void OnDestroy()
    {
        GameplayManager.OnDayStart -= UpdateDay;
    }
    private void OnDisable()
    {
        GameplayManager.OnDayStart -= UpdateDay;
    }
}