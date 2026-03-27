using System;

using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BombMovement : MonoBehaviour
{
    private int angle;
    [SerializeField] private float speed;
    Rigidbody2D rb;
    public Vector2 direction;
    public Vector2 previousDirection;

    public Vector2 newVelocity;

    private Collision2D collision;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        angle = Random.Range(0, 361);
        //Debug.Log(angle);
        angle = Mathf.Abs(angle);
        transform.eulerAngles = new Vector3(0, 0, 90);
        rb = GetComponent<Rigidbody2D>();
        direction = transform.right;
    }

    private void Update()
    {
        /*
        if(Raycast.instance.hit.collider.CompareTag("Player"))
        {
            var firstContact = collision.contacts[0];
            Debug.DrawLine(firstContact.point,firstContact.normal*10, Color.greenYellow, 2);
            newVelocity = Vector2.Reflect(direction.normalized, firstContact.normal);
            Debug.Log("Old: " + direction.normalized + " New: " + newVelocity.normalized);
            Movement(newVelocity.normalized);
        }
        */
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
        Movement(direction);  
    }

    public void Movement(Vector2 direction)
    {
        Debug.Log("Movement Direction: " + direction);
        rb.linearVelocity = direction * speed * Time.fixedDeltaTime;
        
        Debug.Log("Direction: " + direction + " Velocity: " + rb.linearVelocity);

        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            var firstContact = collision.contacts[0];
            Debug.DrawLine(firstContact.point,firstContact.normal*10, Color.greenYellow, 2);
            newVelocity = Vector2.Reflect(direction.normalized, firstContact.normal);
            Debug.Log("Old: " + direction.normalized + " New: " + newVelocity.normalized);
            Destroy(collision.gameObject);
            Movement(newVelocity.normalized);

        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }

    public void CalculateNewDirection()
    {
         //bomb.localPosition= (2 * transform.localPosition);
    }
}
