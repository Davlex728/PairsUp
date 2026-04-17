using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;



public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private PlayerInputHandler mandoMovimiento;
    
    [SerializeField]private int health = 3; // Vida del jugador

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        mandoMovimiento = mando;
    }

    private void FixedUpdate()
    {
        if (mandoMovimiento == null) return;
        //obtener la entrada del input
        Vector2 input = mandoMovimiento.moveInput;
        //hacer que se mueva el jugador con esa entrada
        rb.linearVelocity = new Vector2(input.x * speed, input.y * speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            //Destruir la bala
            Destroy(other.gameObject);
            //Restar vida al jugador
            health--;
            //Si no le queda vida, destruir el jugador
            if (health == 0)
            {
                Destroy(gameObject);
            }
            
            
        }
    }
}