using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int levelLength;
    [SerializeField] private int dayNumber;
    [SerializeField] private int money;

    [Space(5)]
    [SerializeField] private string[] songNames;

    public static event System.Action OnDayStart;
    public static event System.Action OnDayEnd;

    #region Properties

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

    public float Money
    {
        get { return money; }
        private set {; }
    }

    #endregion

    #region Unity Functions

    void Start()
    {
        StartDay();
    }

    #endregion

    #region Public Functions

    public void StartDay()
    {
        levelTimer = 0;
        dayNumber++;

        if(dayNumber == 1)
        {
            AudioManager.Instance.Play(songNames[dayNumber], true, true);
        } else
        {
            AudioManager.Instance.Play(songNames[dayNumber], true);
        }

        OnDayStart?.Invoke();

        StartCoroutine("IncreaseLevelTimer");
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void ToggleOptions()
    {

    }

    public void ToggleCredits()
    {

    }

    public void Quit()
    {

    }

    #endregion

    #region Private Functions

    private void EndDay()
    {
        OnDayEnd?.Invoke();
    }

    #endregion

    #region Coroutines

    IEnumerator IncreaseLevelTimer()
    {
        while(levelTimer < levelLength)
        {
            levelTimer += Time.deltaTime;
            yield return null;
        }

        EndDay();
    }

    #endregion
}
