using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private float levelTimer;
    [SerializeField] private int levelLength;
    [SerializeField] private int dayNumber;
    public static event System.Action OnDayStart;
    public static event System.Action OnDayEnd;

    public float LevelTimer
    {
        get { return levelTimer; }
        private set {; }
    }

    public float DayNumber
    {
        get { return dayNumber; }
        private set {; }
    }

    public void StartDay()
    {
        levelTimer = 0;

        OnDayStart?.Invoke();

        StartCoroutine("IncreaseLevelTimer");
    }

    private void EndDay()
    {
        dayNumber++;
        OnDayEnd?.Invoke();
    }

    IEnumerator IncreaseLevelTimer()
    {
        while(levelTimer < levelLength)
        {
            levelTimer += Time.deltaTime;
            yield return null;
        }

        EndDay();
    }
}
