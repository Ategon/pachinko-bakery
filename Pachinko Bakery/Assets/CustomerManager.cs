using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private int nextProduct;
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private int[] productAmounts;
    [SerializeField] private TextMeshProUGUI[] textObjects;

    public void addProduct(int id, int amount = 1)
    {
        productAmounts[id - 1] += amount;
        textObjects[id - 1].text = "" + productAmounts[id - 1];
    }

    void Start()
    {
        nextProduct = Random.Range(1, 3);
    }

    public void useProduct(int id)
    {
        if(id == nextProduct)
        {
            //correct
            if(productAmounts[id-1] > 0)
            {
                productAmounts[id - 1] -= 1;
                gameplayManager.AddMoney(4);
                nextProduct = Random.Range(1, 3);
                textObjects[id - 1].text = "" + productAmounts[id - 1];
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
            }
            else
            {
                //not enough
            }
        }
    }
}
