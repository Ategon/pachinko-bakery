using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputHandler inputHandler;

    [Header("Variables")]
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private Vector2 mousePosition;
    [SerializeField] private float cannonAngle;

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

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
