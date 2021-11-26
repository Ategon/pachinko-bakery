using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private Slider[] sliders;

    [Header("Variables")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int levelLength;
    [SerializeField] private int dayNumber;
    [SerializeField] private int money;

    [Space(5)]
    [SerializeField] private string[] songNames;

    public static event System.Action OnDayStart;
    public static event System.Action OnDayEnd;
    public static event System.Action OnMoneyGain;

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
        AudioManager.Instance.Play(songNames[0]);
        sliders[0].value = AudioManager.Instance.Volume(-1f);
        sliders[1].value = AudioManager.Instance.Volume(-1f, "Soundtracks");
        sliders[2].value = AudioManager.Instance.Volume(-1f, "UI SFX");
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
        OnMoneyGain?.Invoke();
    }

    public void Play()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        StartDay();
    }

    public void ToggleOptions()
    {
        if (optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
        } else
        {
            optionsMenu.SetActive(true);
            creditsMenu.SetActive(false);
        }
    }

    public void ToggleCredits()
    {
        if (creditsMenu.activeSelf)
        {
            creditsMenu.SetActive(false);
        }
        else
        {
            optionsMenu.SetActive(false);
            creditsMenu.SetActive(true);
        }
    }

    public void MasterChange(float amount)
    {
        AudioManager.Instance.Volume(amount);
    }

    public void SoundtracksChange(float amount)
    {
        AudioManager.Instance.Volume(amount, "Soundtracks");
    }

    public void SFXChange(float amount)
    {
        AudioManager.Instance.Volume(amount, "UI");
        AudioManager.Instance.Volume(amount, "Player");
        AudioManager.Instance.Volume(amount, "Objects");
    }

    public void UIHover()
    {
        AudioManager.Instance.Play("Hover");
    }

    public void UIForward()
    {
        AudioManager.Instance.Play("Forward");
    }

    public void UIBackward()
    {
        AudioManager.Instance.Play("Backward");
    }

    public void UIChange()
    {
        AudioManager.Instance.Play("Load");
    }

    public void AteDiscord()
    {
        Application.OpenURL("https://discord.gg/HNwTAW7");
    }

    public void AteTwitter()
    {
        Application.OpenURL("https://twitter.com/Etegondev");
    }

    public void AteYT()
    {
        Application.OpenURL("https://www.youtube.com/playlist?list=PLS74pJwD4TnydqBISELS6DrywMzhEU6MK");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
