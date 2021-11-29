using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private int nextProduct;
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private int[] productAmounts;
    [SerializeField] private TextMeshProUGUI[] textObjects;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image spriteRenderer;
    [SerializeField] private float customerTimer = 20f;
    [SerializeField] private TextMeshProUGUI customerTimerText;

    public int[] ProductAmounts
    {
        get { return productAmounts; }
        private set {; }
    }

    public void addProduct(int id, int amount = 1)
    {
        productAmounts[id - 1] += amount;
        textObjects[id - 1].text = "" + productAmounts[id - 1];
    }

    void Start()
    {
        nextProduct = Random.Range(1, 3);
        GameplayManager.OnDayStart += UpdateDay;
    }

    void FixedUpdate()
    {
        customerTimer -= Time.deltaTime;
        if(customerTimer <= 0)
        {
            //ran out of time
            gameplayManager.AddMoney(-3);
            nextProduct = Random.Range(1, 3);
            spriteRenderer.sprite = sprites[nextProduct - 1];
            customerTimer = 10f / gameplayManager.customerTimerMult;
        }
        customerTimerText.text = $"{Mathf.Round(customerTimer * 10f) / 10f} sec";
    }

    private void UpdateDay()
    {
        customerTimer = 20f;
        nextProduct = Random.Range(1, 3);
        productAmounts[0] = 0;
        productAmounts[1] = 0;
        textObjects[0].text = "" + productAmounts[0];
        textObjects[1].text = "" + productAmounts[1];
    }

    public void useProduct(int id)
    {
        if(id == nextProduct)
        {
            //correct
            if(productAmounts[id-1] > 0)
            {
                productAmounts[id - 1] -= 1;
                gameplayManager.AddMoney(gameplayManager.moneyMult);
                nextProduct = Random.Range(1, 3);
                textObjects[id - 1].text = "" + productAmounts[id - 1];
                spriteRenderer.sprite = sprites[nextProduct - 1];
                customerTimer = 10f / gameplayManager.customerTimerMult;
            } else
            {
                //not enough
            }
        } else
        {
            //incorrect
            if (productAmounts[id - 1] > 0)
            {
                productAmounts[id - 1] -= 1;
                nextProduct = Random.Range(1, 3);
                textObjects[id - 1].text = "" + productAmounts[id - 1];
                spriteRenderer.sprite = sprites[nextProduct - 1];
                customerTimer = 10f / gameplayManager.customerTimerMult;
            }
            else
            {
                //not enough
            }
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
