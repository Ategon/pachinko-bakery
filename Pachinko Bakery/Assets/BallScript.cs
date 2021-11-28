using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private CannonScript cannonScript;
    private GameplayManager gameplayManager;
    [SerializeField] private int ballID = 0;

    void Start()
    {
        cannonScript = GameObject.Find("Cannon").GetComponent<CannonScript>();
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        GetComponent<Rigidbody2D>().sharedMaterial.bounciness = GetComponent<Rigidbody2D>().sharedMaterial.bounciness/gameplayManager.heavyMult;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "EndZone")
        {
            cannonScript.AddBall(ballID);
            Destroy(this.gameObject);
        }

        if(collision.gameObject.tag == "Peg" && ballID == 3)
        {
            gameplayManager.AddMoney(0.25f);
        }
    }
}
