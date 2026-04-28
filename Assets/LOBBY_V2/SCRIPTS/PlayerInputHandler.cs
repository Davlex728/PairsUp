using UnityEngine;
using UnityEngine.InputSystem;
//recordatorio que hay que asignar los eventos de los players en el prefab SI CAMBIAMOS EL NOMBRE DE UNA ESCENA LAS COSAS SE ROMPEN, "desnúdate mujer" -Frankie ruiz
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Estado de los Botones (Solo lectura)")]
    // Los Avatares leerán estas variables para saber qué hacer
    public Vector2 moveInput;
    public bool isJumping;
    public bool isGrabbing;
    public bool isShooting;
    public bool isSouthZone;
    public bool isNorthZone;
    public bool isWestZone;
    public bool isEastZone;
    public Vector2 lookInput;

    //platformer
    public bool isSwitchingTool;
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

    public void OnShootInput(InputAction.CallbackContext context)
    {
        Debug.Log($"[OnShootInput] llamado! performed={context.performed} | canceled={context.canceled}");
        if (context.performed) isShooting = true;
        else if (context.canceled) isShooting = false;
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    /* public void OnSeparate(InputAction.CallbackContext context)
     {

     }*/

    public void OnSwitchToolInput(InputAction.CallbackContext context)
    {
        if (context.performed) isSwitchingTool = true;
        else if (context.canceled) isSwitchingTool = false;
    }
    
    public void OnSouthZoneInput(InputAction.CallbackContext context)
    {
       if(context.performed) isSouthZone = true;
       else if (context.canceled) isSouthZone = false;
    }
    public void OnNorthZoneInput(InputAction.CallbackContext context)
    {
        if(context.performed) isNorthZone = true;
        else if (context.canceled) isNorthZone = false;
    }
    public void OnWestZoneInput(InputAction.CallbackContext context)
    {
        if(context.performed) isWestZone = true;
        else if (context.canceled) isWestZone = false;
    }
    public void OnEastZoneInput(InputAction.CallbackContext context)
    {
        if(context.performed) isEastZone = true;
        else if (context.canceled) isEastZone = false;
    }
}