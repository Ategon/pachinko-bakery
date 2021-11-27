using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegScript : MonoBehaviour
{
    [SerializeField] private float breadType;
    [SerializeField] private float pegTimer;
    [SerializeField] private bool hit;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        spriteRenderer.sprite = sprites[1];
        hit = false;
    }

    public void SetSprite(int id)
    {
        spriteRenderer.sprite = sprites[id];
    }

    void FixedUpdate()
    {
        if (hit)
        {
            pegTimer -= Time.deltaTime;
            if(pegTimer <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            hit = true;
            pegTimer = 5f;
            spriteRenderer.sprite = sprites[0];
        }
    }
}
