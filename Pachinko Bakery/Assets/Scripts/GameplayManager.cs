using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject pachinkoBoard;
    [SerializeField] private GameObject topThings;
    [SerializeField] private GameObject forge;
    [SerializeField] private Slider[] sliders;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject pauseButton;

    [Header("Variables")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int levelLength;
    [SerializeField] private int dayNumber;
    [SerializeField] private int money;
    public bool paused;

    [Header("Multipliers")]
    public float fireMult = 1;
    public float firePegMult = 1;
    public float heavyMult = 1;
    public float customerTimerMult = 1;
    public float cannonRepairMult = 1;
    public float ovenRepairMult = 1;


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

    public float LevelLength
    {
        get { return levelLength; }
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

        switch (dayNumber)
        {
            case 1:
                break;
            case 2:
                cannonRepairMult += 0.5f;
                ovenRepairMult += 0.5f;
                break;
            case 3:
                cannonRepairMult -= 0.5f;
                ovenRepairMult -= 0.5f;
                heavyMult += 0.5f;
                break;
            case 4:
                heavyMult -= 0.5f;
                firePegMult += 1;
                fireMult += 1;
                break;
            case 5:
                firePegMult -= 1;
                fireMult -= 1;
                customerTimerMult += 1;
                break;
            default:
                break;
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
        pachinkoBoard.SetActive(true);
        topThings.SetActive(true);
        forge.SetActive(true);
        title.SetActive(false);
        StartDay();
    }

    public void ToggleOptions()
    {
        if (optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
            title.SetActive(true);
        } else
        {
            optionsMenu.SetActive(true);
            creditsMenu.SetActive(false);
            title.SetActive(false);
        }
    }

    public void ToggleCredits()
    {
        if (creditsMenu.activeSelf)
        {
            creditsMenu.SetActive(false);
            title.SetActive(true);
        }
        else
        {
            optionsMenu.SetActive(false);
            creditsMenu.SetActive(true);
            title.SetActive(false);
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

    public void Pause()
    {
        Time.timeScale = 0;
        AudioManager.Instance.Pitch(0f, "Soundtracks", true);
        pauseCanvas.SetActive(true);
        pauseButton.SetActive(false);
        paused = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
        pauseButton.SetActive(true);
        paused = false;
        AudioManager.Instance.Pitch(1f, "Soundtracks", true);
    }

    public void QuitLevel()
    {
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
        pauseButton.SetActive(true);
        paused = false;
        AudioManager.Instance.Pitch(1f, "Soundtracks");
        SceneManager.LoadScene("MainScene");
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
        StartDay(); //TEMP
    }

    #endregion
}
