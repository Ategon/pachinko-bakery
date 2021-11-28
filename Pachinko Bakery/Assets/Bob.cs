using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    Vector2 floatY;
    float originalY;

    [SerializeField] float floatStrength;

    void Start()
    {
        this.originalY = this.transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x,
            originalY + ((float)System.Math.Sin(Time.time * 1.5) * floatStrength),
            transform.position.z);
    }
}
