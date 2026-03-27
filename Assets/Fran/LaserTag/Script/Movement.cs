using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;
    
    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    private float inputH;
    private float inputV;
    
    public Gamepad MandoAsignado { get; private set; }
    
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    IEnumerator Start()
    {
        yield return null; // esperar un frame para que PlayerInput se inicialice

        InputUser.PerformPairingWithDevice(
            Gamepad.current,
            user: playerInput.user
        );

        playerInput.SwitchCurrentControlScheme(
            "DualGamepad",
            Gamepad.all[0],
            Gamepad.current
        );

        Debug.Log($"Control Scheme: {playerInput.currentControlScheme}");
        foreach (var d in playerInput.devices)
            Debug.Log($"  - {d.displayName}");
    }

    void FixedUpdate()
    {
        
    }
   

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        rb.linearVelocity = new Vector2(moveInput.x * speed, moveInput.y * speed);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        float angle = Mathf.Atan2(-lookInput.x, lookInput.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
            Destroy(gameObject);
    }
    
    public void AsignarMando(Gamepad gamepad)
    {
        MandoAsignado = gamepad;
        if (gamepad != null)
            Debug.Log($"{name} usa {gamepad.displayName} (ID:{gamepad.deviceId})");
        else
            Debug.Log($"{name} sin mando — fallback a teclado");
    }
   
}