using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    [SerializeField] private GameObject dayText;
    [SerializeField] private TextMeshProUGUI dayTitle;
    [SerializeField] private TextMeshProUGUI dayDesc; 
    [SerializeField] private Image materialColor;
    [SerializeField] private GameObject dayMenuCanvas;
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private CustomerManager customerManager;

    [SerializeField] private Image shopButton1;
    [SerializeField] private Image shopButton2;
    [SerializeField] private Image shopButton3;

    [SerializeField] private TextMeshProUGUI shopTitle1;
    [SerializeField] private TextMeshProUGUI shopTitle2;
    [SerializeField] private TextMeshProUGUI shopTitle3;

    [SerializeField] private TextMeshProUGUI shopDesc1;
    [SerializeField] private TextMeshProUGUI shopDesc2;
    [SerializeField] private TextMeshProUGUI shopDesc3;

    [SerializeField] private TextMeshProUGUI endDayText;
    [SerializeField] private TextMeshProUGUI moneyTallyText;
    [SerializeField] private TextMeshProUGUI overstockText;

    [SerializeField] private TextMeshProUGUI shopBalanceText;

    [SerializeField] private PhysicsMaterial2D normalMat;
    [SerializeField] private PhysicsMaterial2D heavyMat;
    [SerializeField] private PhysicsMaterial2D bouncyMat;

    [Header("Variables")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int levelLength;
    [SerializeField] private int dayNumber;
    [SerializeField] private float money;
    [SerializeField] private float titleTimer;
    public bool paused;
    private float timeAmount = 0.5f;
    [SerializeField] private string[] dayTitles;
    [SerializeField] private string[] dayDescs;

    [Header("Multipliers")]
    public float fireMult = 1;
    public float firePegMult = 1;
    public float heavyMult = 1;
    public float customerTimerMult = 1;
    public float cannonRepairMult = 1;
    public float ovenRepairMult = 1;


    [Space(5)]
    [SerializeField] private string[] songNames;

    [Header("ShopInfo")]
    [SerializeField] private int[] itemPrices;
    [SerializeField] private string[] itemDescs;
    [SerializeField] private string[] itemTitles;
    [SerializeField] private Color[] itemColors;
    [SerializeField] private int[] shopIds = new int[3];

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
        EndDay(); // TEMP
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
        OnMoneyGain?.Invoke();
        dayNumber++;
        normalMat.bounciness = 0.7f;
        heavyMat.bounciness = 0.5f;
        bouncyMat.bounciness = 0.9f;

        dayTitle.text = dayTitles[dayNumber - 1];
        dayDesc.text = dayDescs[dayNumber - 1];
        dayText.SetActive(true);
        StartCoroutine("ShowDayTimer"); 

        if (dayNumber == 1)
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

    public void AddMoney(float amount)
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

    public void BuyUpgrade(int buttonId)
    {
        shopBalanceText.text = $"Money: ${money.ToString("0.00")}";
    }

    private void EndDay()
    {
        string dayText = "";
        switch (dayNumber)
        {
            case 1:
                dayText = "Monday";
                break;
            case 2:
                dayText = "Tuesday";
                break;
            case 3:
                dayText = "Wednesday";
                break;
            case 4:
                dayText = "Thursday";
                break;
            case 5:
                dayText = "Friday";
                break;
            default:
                break;
        }
        endDayText.text = $"{dayText} Clear";
        moneyTallyText.text = $"Money tally: ${money.ToString("0.00")}";
        overstockText.text = $"Overstock: ${(customerManager.ProductAmounts[0] + customerManager.ProductAmounts[1]).ToString("0.00")}";
        money += customerManager.ProductAmounts[0] + customerManager.ProductAmounts[1];

        Time.timeScale = 0;
        AudioManager.Instance.Pitch(0f, "Soundtracks", true);
        dayMenuCanvas.SetActive(true);
        paused = true;
        GenerateShopTiles();
        OnDayEnd?.Invoke();
    }

    private void GenerateShopTiles()
    {
        shopBalanceText.text = $"Money: ${money.ToString("0.00")}";

        for (int i = 0; i < 3; i++)
        {
            float rarityChance = Random.Range(0, 1f);
            if (rarityChance <= 0.33)
            {
                shopIds[i] = Random.Range(5, 15);
            }
            else if (rarityChance <= 0.66)
            {
                shopIds[i] = Random.Range(1, 5);
            }
            else
            {
                shopIds[i] = 0;
            }
        }

        //changing text + images;
        shopButton1.color = itemColors[shopIds[0]];
        shopButton2.color = itemColors[shopIds[1]];
        shopButton3.color = itemColors[shopIds[2]];

        shopTitle1.text = itemTitles[shopIds[0]];
        shopDesc1.text = itemDescs[shopIds[0]];

        shopTitle2.text = itemTitles[shopIds[1]];
        shopDesc2.text = itemDescs[shopIds[1]];

        shopTitle3.text = itemTitles[shopIds[2]];
        shopDesc3.text = itemDescs[shopIds[2]];
    }

    public void RefreshShop()
    {
        AddMoney(-5f);
        shopBalanceText.text = $"Money: ${money.ToString("0.00")}";
        GenerateShopTiles();
    }

    public void GoNextDay()
    {
        Time.timeScale = 1;
        shopCanvas.SetActive(false);
        paused = false;
        AudioManager.Instance.Pitch(1f, "Soundtracks", true);
        StartDay();
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

    IEnumerator ShowDayTimer()
    {
        while (titleTimer <timeAmount)
        {
            materialColor.color = Color.Lerp(Color.clear, new Color(0f, 0f, 0f, 0.5f), titleTimer/timeAmount);
            dayTitle.color = Color.Lerp(Color.clear, Color.white, titleTimer / timeAmount);
            dayDesc.color = Color.Lerp(Color.clear, Color.white, titleTimer / timeAmount);
            titleTimer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(5);
        titleTimer = 0;
        StartCoroutine("HideDayTimer");
    }

    IEnumerator HideDayTimer()
    {
        while (titleTimer <timeAmount)
        {
            materialColor.color = Color.Lerp(new Color(0f, 0f, 0f, 0.5f), Color.clear, titleTimer / timeAmount);
            dayTitle.color = Color.Lerp(Color.white, Color.clear, titleTimer / timeAmount);
            dayDesc.color = Color.Lerp(Color.white, Color.clear, titleTimer / timeAmount);
            titleTimer += Time.deltaTime;
            yield return null;
        }

        dayText.SetActive(false);
        titleTimer = 0;
    }

    #endregion
}
