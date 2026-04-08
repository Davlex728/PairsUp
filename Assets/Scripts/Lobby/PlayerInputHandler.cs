using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Estado de los Botones (Solo lectura)")]
    // Los Avatares leerán estas variables para saber qué hacer
    public Vector2 moveInput;
    public bool isJumping;
    public bool isGrabbing;



    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Conecta esto al evento "Jump" en el Inspector
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // context.performed significa que el botón está pulsado
        // context.canceled significa que lo has soltado
        if (context.performed) isJumping = true;
        else if (context.canceled) isJumping = false;
    }

    // Conecta esto al evento "Grab" en el Inspector (para la Mano)
    public void OnGrabInput(InputAction.CallbackContext context)
    {
        if (context.performed) isGrabbing = true;
        else if (context.canceled) isGrabbing = false;
    }
}