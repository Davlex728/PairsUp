using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BombMovement : MonoBehaviour
{
    private int angle;
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    public Vector2 direction;

    public Vector2 newVelocity;

    [SerializeField]private float currentSpeed;

    [SerializeField] private float speedIncreasePercentage;
    
    private float speedIncreasePerHit;
    

    [SerializeField] private float maxSpeed;

    public Transform spawnPoint;

    public float contador;
    
    void Start()
    {
        currentSpeed = speed;
        
        speedIncreasePerHit =  speedIncreasePercentage / 100f; 
        angle = Random.Range(0, 361);
        angle = Mathf.Abs(angle);
        transform.eulerAngles = new Vector3(0, 0, angle);
        rb = GetComponent<Rigidbody2D>();

        direction = transform.right;
        
            rb.linearVelocity = direction * currentSpeed; 
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    
        currentSpeed *= (1 + speedIncreasePerHit);
        
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        
            rb.linearVelocity = currentSpeed * direction;
        

    }

    private void FixedUpdate()
    {
        contador += Time.fixedDeltaTime;
        
    }

    public void RespawnBomb()
    {
        transform.position = spawnPoint.position;
        
        Debug.Log(currentSpeed);
        rb.linearVelocity = new Vector2(0,0);
        if (contador >= 2f)
        {
            StartCoroutine(Timer(2));
        }
        
    }
    
    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        angle = Random.Range(0, 361);
        angle = Mathf.Abs(angle);
        transform.eulerAngles = new Vector3(0, 0, angle);
        currentSpeed = speed;
        direction = transform.right;
        rb.linearVelocity = direction * currentSpeed;
    }
}

