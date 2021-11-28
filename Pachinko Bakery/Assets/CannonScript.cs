using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CannonScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private GameObject[] ballPrefabs;
    [SerializeField] private Sprite[] ballSprites;
    [SerializeField] private SpriteRenderer previewSprite;
    [SerializeField] private Transform shotArea;
    [SerializeField] private TextMeshProUGUI waitText;
    [SerializeField] private GameplayManager gameplayManager;

    [SerializeField] private PhysicsMaterial2D normalMat;
    [SerializeField] private PhysicsMaterial2D heavyMat;
    [SerializeField] private PhysicsMaterial2D bouncyMat;

    [Header("Variables")]
    [SerializeField] private Vector2 mousePosition;
    [SerializeField] private float cannonAngle;
    [SerializeField] private bool waitRight;
    [SerializeField] private bool ballOut;
    [SerializeField] private int bagSize;
    [SerializeField] private int bagUsed;
    [SerializeField] private int repairTime = 10;
    [SerializeField] private float repairTimer;
    [SerializeField] private bool underRepair;
    [SerializeField] private Queue<int> balls = new Queue<int>();
    //0 - normal ball, 1 - large ball, 2 - bouncy ball, 3 - money ball, 4 - low grav

    void Start()
    {
        balls.Enqueue(3);
        balls.Enqueue(3);
        bagSize = 2;
        updatePreview();
    }

    public void AddBall(int id)
    {
        balls.Enqueue(id);
        ballOut = false;
        if(!underRepair) updatePreview();
    }

    public void AddForceAtAngle(float force, float angle, Rigidbody2D rb)
    {
        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180) * force;
        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180) * force;

        rb.AddForce(new Vector3(ycomponent, 0, xcomponent));
    }

    void Update()
    {
        if (!underRepair && !gameplayManager.paused)
        {
            mousePosition = inputHandler.MousePosition;
            mousePosition = Camera.main.ScreenToViewportPoint(mousePosition);
            Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

            float angle = AngleBetweenTwoPoints(positionOnScreen, mousePosition);
            cannonAngle = angle - 90;
            if (cannonAngle < -80 && cannonAngle >= -180) cannonAngle = -80;
            else if (cannonAngle < -180 || cannonAngle > 80) cannonAngle = 80;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, cannonAngle));
        }
    }

    void updatePreview()
    {
        if (balls.Count > 0)
        {
            previewSprite.sprite = ballSprites[balls.Peek()];
        }
        else
        {
            previewSprite.sprite = null;
        }
    }

    void removePreview()
    {
        if (bagUsed == bagSize)
        {
            previewSprite.sprite = null;
            bagUsed = 0;
            repairTimer = repairTime * gameplayManager.cannonRepairMult;
            underRepair = true;
        }
        else
        {
            previewSprite.sprite = null;
        }
    }

    void FixedUpdate()
    {
        if (underRepair)
        {
            repairTimer -= Time.deltaTime;
            waitText.text = $"{Mathf.Round(repairTimer * 10f) / 10f}";
            if(repairTimer < 0)
            {
                underRepair = false;
                waitText.text = "";
                updatePreview();
            }
        }


        if (!underRepair && !gameplayManager.paused)
        {
            if (inputHandler.RightClickInput == true)
            {
                if (waitRight == false)
                {
                    waitRight = true;
                    if (balls.Count > 0 && !ballOut)
                    {
                        normalMat.bounciness = 0.7f;
                        heavyMat.bounciness = 0.5f;
                        bouncyMat.bounciness = 0.9f;

                        //create ball
                        GameObject newBall = Instantiate(ballPrefabs[balls.Dequeue()], shotArea.position, Quaternion.identity);
                        AddForceAtAngle(25, cannonAngle, newBall.GetComponent<Rigidbody2D>());
                        bagUsed++;
                        ballOut = true;
                        removePreview();
                    }
                }
            }
            else
            {
                waitRight = false;
            }
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
