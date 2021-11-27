using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FurnaceScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heatText;
    [SerializeField] private int heatState = 1;
    [SerializeField] private string[] heatTexts;
    [SerializeField] private Color[] heatColors;
    [SerializeField] private float bakingTimer;
    [SerializeField] private bool isBaking;
    [SerializeField] private int bakingType;
    [SerializeField] private float[] heatMultipliers;
    [SerializeField] private GameObject collectButton;

    void Start()
    {
        heatText.text = heatTexts[heatState];
        heatText.color = heatColors[heatState];
    }

    public void ChangeHeat()
    {
        heatState++;
        if (heatState >= 4) heatState = 0;
        heatText.text = heatTexts[heatState];
        heatText.color = heatColors[heatState];
    }

    public void StartBaking(int type)
    {
        bakingTimer = 15f;
        bakingType = type;
        isBaking = true;
    }

    void FixedUpdate()
    {
        if (isBaking)
        {
            bakingTimer -= Time.deltaTime * heatMultipliers[heatState];
            if (bakingTimer < 0)
            {
                isBaking = false;
                collectButton.SetActive(true);
            }
        }
    }

    public void Collect()
    {
        GameObject.Find("Pegs").GetComponent<PegManager>().respawnPeg(bakingType, 4);
    }
}
