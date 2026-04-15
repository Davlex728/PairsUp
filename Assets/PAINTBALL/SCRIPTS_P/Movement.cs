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
        Vector2 input = mandoMovimiento.moveInput;
        rb.linearVelocity = new Vector2(input.x * speed, input.y * speed);
    }
   
}