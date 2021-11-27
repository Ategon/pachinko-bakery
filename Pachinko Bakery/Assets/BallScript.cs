using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private CannonScript cannonScript;
    [SerializeField] private int ballID = 0;

    void Start()
    {
        cannonScript = GameObject.Find("Cannon").GetComponent<CannonScript>();
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "EndZone")
        {
            cannonScript.AddBall(ballID);
            Destroy(this.gameObject);
        }
    }
}
