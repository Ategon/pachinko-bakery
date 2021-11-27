using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private GameObject[] ballPrefabs;
    [SerializeField] private Sprite[] ballSprites;
    [SerializeField] private SpriteRenderer previewSprite;
    [SerializeField] private Transform shotArea;

    [Header("Variables")]
    [SerializeField] private Vector2 mousePosition;
    [SerializeField] private float cannonAngle;
    [SerializeField] private bool waitRight;
    [SerializeField] private Queue<int> balls = new Queue<int>();
    //0 - normal ball, 1 - large ball, 2 - bouncy ball, 3 - cloning ball, 4 - low grav

    void Start()
    {
        balls.Enqueue(0);
        balls.Enqueue(1);
        balls.Enqueue(2);
        balls.Enqueue(3);
        balls.Enqueue(4);
        updatePreview();
    }

    public void AddBall(int id)
    {
        balls.Enqueue(id);
        updatePreview();
    }

    public void AddForceAtAngle(float force, float angle, Rigidbody2D rb)
    {
        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180) * force;
        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180) * force;

        rb.AddForce(new Vector3(ycomponent, 0, xcomponent));
    }

    void Update()
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

    void updatePreview()
    {
        if (balls.Count > 0)
        {
            previewSprite.sprite = ballSprites[balls.Peek()];
        } else
        {
            previewSprite.sprite = null;
        }
    }

    void FixedUpdate()
    {

        //peek at next ball for showing

        if(inputHandler.RightClickInput == true)
        {
            if(waitRight == false)
            {
                waitRight = true;
                if(balls.Count > 0)
                {
                    //create ball
                    GameObject newBall = Instantiate(ballPrefabs[balls.Dequeue()], shotArea.position, Quaternion.identity);
                    AddForceAtAngle(25, cannonAngle, newBall.GetComponent<Rigidbody2D>());
                    updatePreview();
                }
            }
        } else
        {
            waitRight = false;
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
