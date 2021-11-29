using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private Vector2 mousePosition;
    [SerializeField] private bool rightClickInput;

    public Vector2 MousePosition
    {
        get { return mousePosition; }
        private set {; }
    }

    public bool RightClickInput
    {
        get { return rightClickInput; }
        private set {; }
    }


    public void OnAim(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public void OnRightClick(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() == 1f)
        {
            rightClickInput = true;
        }
        else
        {
            rightClickInput = false;
        }
    }
}
