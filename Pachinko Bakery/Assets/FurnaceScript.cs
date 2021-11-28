using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image furnaceImage;
    [SerializeField] private GameObject clickButton;
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private ParticleSystem fireParticles;

    [SerializeField] private float[] fireChances;
    [SerializeField] private bool onSmoke;
    [SerializeField] private float smokeTimer;
    [SerializeField] private float[] smokeMultipliers;
    [SerializeField] private float lastTimeCheck;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private float cooldownTimer;
    [SerializeField] private bool onCooldown;
    [SerializeField] private GameObject startButton;

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

    public void SetFire()
    {
        //disable all buttons
        //enable click button
        //play particle

        //boolean for if starting in or not and then reverts to appropriate spot(whether in order or not)
    }

    public void StartBaking(int type)
    {
        GameObject.Find("GameplayManager").GetComponent<GameplayManager>().AddMoney(-1);
        bakingTimer = 15f;
        bakingType = type;
        furnaceImage.sprite = sprites[1];
        isBaking = true;
    }

    public void ExtinguishFire()
    {
        if(smokeTimer < 0)
        {
            //fire
            onSmoke = false;
            smokeParticles.Stop();
            fireParticles.Stop();
            isBaking = false;
            clickButton.SetActive(false);
            furnaceImage.sprite = sprites[0];
            cooldownTimer = 15f * GameObject.Find("GameplayManager").GetComponent<GameplayManager>().ovenRepairMult;
            onCooldown = true;

        } else
        {
            //smoke
            onSmoke = false;
            smokeParticles.Stop();
            clickButton.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownText.text = $"{Mathf.Round(cooldownTimer * 10f) / 10f}";
            if (cooldownTimer < 0)
            {
                onCooldown = false;
                startButton.SetActive(true);
                cooldownText.text = "";
            }
        } 


        if (onSmoke)
        {
            smokeTimer -= Time.deltaTime * smokeMultipliers[heatState];
            if (smokeTimer < 0)
            {
                fireParticles.Play();
            }
        }


        if (isBaking && !onSmoke && !onCooldown)
        {
            bakingTimer -= Time.deltaTime * heatMultipliers[heatState];

            if (lastTimeCheck != Mathf.Floor(bakingTimer))
            {
                lastTimeCheck = Mathf.Floor(bakingTimer);

                if (Random.Range(0f, 1f) <= fireChances[heatState] * GameObject.Find("GameplayManager").GetComponent<GameplayManager>().fireMult)
                {
                    onSmoke = true;
                    smokeParticles.Play();
                    clickButton.gameObject.SetActive(true);
                    clickButton.SetActive(true);
                    smokeTimer = 8f;
                }

                if (bakingTimer < 0 && !onSmoke)
                {
                    isBaking = false;
                    furnaceImage.sprite = sprites[0];
                    collectButton.SetActive(true);
                }
            }
        }
    }

    public void Collect()
    {
        GameObject.Find("Pegs").GetComponent<PegManager>().respawnPeg(bakingType, 4);
    }
}
