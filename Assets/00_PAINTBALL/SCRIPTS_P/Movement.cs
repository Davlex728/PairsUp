using System;
using UnityEngine;
using System.Collections.Generic;



public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    public PlayerInputHandler mandoMovimiento;
    
    [SerializeField]private int health = 3; // Vida del jugador
    private List<GameObject> misCorazones;
    public GameObject prefabSangre;

    int tamu = 0;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        mandoMovimiento = mando;
    }
    public void AsignarCorazones(List<GameObject> corazones)
    {
        misCorazones = corazones;
        UIManager.instance.ActualizarCorazones(misCorazones, health);
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
            health = health - 1;
            Debug.Log(health);
            //Si no le queda vida, destruir el jugador
            UIManager.instance.ActualizarCorazones(misCorazones, health);
            
            if (health == 0)
            {
                Destroy(gameObject);
                Instantiate(prefabSangre, transform.position, Quaternion.identity);
            }
            
            
        }
    }
    private void Start()
    {
        mandoMovimiento.LastBullet();
        tamu = mandoMovimiento.teamIndex;
        Debug.Log($"[Movement] Team Index: {tamu}");
    }
}