using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private TextMeshProUGUI moneyText;

    void Start()
    {
        GameplayManager.OnMoneyGain += UpdateMoney;
    }

    void UpdateMoney()
    {
        moneyText.text = $"{gameplayManager.Money}";
    }

    private void OnDestroy()
    {
        GameplayManager.OnMoneyGain -= UpdateMoney;
    }
    private void OnDisable()
    {
        GameplayManager.OnMoneyGain -= UpdateMoney;
    }
}
