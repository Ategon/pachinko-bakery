using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private Vector2 mousePosition;

    public Vector2 MousePosition
    {
        get { return mousePosition; }
        private set {; }
    }


    public void OnAim(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }
}
