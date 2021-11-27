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
}
