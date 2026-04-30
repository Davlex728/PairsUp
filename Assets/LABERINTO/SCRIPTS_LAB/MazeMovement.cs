using System;
using UnityEngine;

public class MazeMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    
    public Rigidbody2D rb;
    
    public PlayerInputHandler mandoMovimiento;
    
    public static MazeMovement instance;
    
    public Transform spawnPoint;
    
    public InputBoton inputEfectivo = InputBoton.Ninguno;
    
    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
    }
    
    public void RecibirInput(InputBoton boton)
    {
        inputEfectivo = boton;
        // Aquí tu lógica de movimiento según el botón
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        mandoMovimiento = mando;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (mandoMovimiento == null) return;
        Vector2 input = mandoMovimiento.moveInput;
        
        rb.linearVelocity = new Vector2(input.x * speed, input.y * speed);
        
        Debug.Log($"Moviendo la mierda esta");
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 8)
        {
            transform.position = spawnPoint.position;
        }
    }
}
