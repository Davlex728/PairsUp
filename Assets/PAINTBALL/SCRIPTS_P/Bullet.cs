using System;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 10f;
    
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        
    }

    private void Update()
    {
       
    }

    public void Disparo(Vector2 direction)
    {
        this.direction = direction;
        rb.linearVelocity = this.direction * speed;
       
           
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            var firstContact = other.contacts[0];
            Vector2 newVelocity = Vector2.Reflect(direction.normalized, firstContact.normal);
            Disparo(newVelocity.normalized);
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
        
        if (other.gameObject.CompareTag("Wall"))
        {
            Wall wall = other.gameObject.GetComponent<Wall>();
            if (wall != null)
            {
                wall.OnHit();
            }
        }
    }

    
}
