using UnityEngine;

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
    
// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = speed;
        
        speedIncreasePerHit =  speedIncreasePercentage / 100f; 
        angle = Random.Range(0, 361);
        angle = Mathf.Abs(angle);
        transform.eulerAngles = new Vector3(0, 0, angle);
        rb = GetComponent<Rigidbody2D>();

//Como lo has rotado, ahora hay que ir a la derecha
        direction = transform.right;
//Acuerdate de asignarle una velocidad inicial en el editor.
//Esto hace que empiece moviéndose
        rb.linearVelocity = direction * currentSpeed; // Initial velocity of the ball
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       /* Debug.Log("Colisión detectada con: " + collision.gameObject.name);
        var firstContact = collision.contacts[0];
        // Rebota usando la velocidad real actual (no una dirección "vieja").
        var currentVelocity = rb.linearVelocity;
        if (currentVelocity.sqrMagnitude < 0.0001f)
        {
            // Fallback por si justo coincide con reposo.
            currentVelocity = direction.sqrMagnitude > 0.0001f
                ? direction * currentSpeed
                : transform.right * currentSpeed;
        }

        newVelocity = Vector2.Reflect(currentVelocity.normalized, firstContact.normal);
        direction = rb.linearVelocity.normalized;*/
    
        currentSpeed *= (1 + speedIncreasePerHit);
        
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        //newVelocity = newVelocity.normalized * currentSpeed;
        rb.linearVelocity = currentSpeed * direction;

        // Mantén 'direction' coherente si lo usas en otros sitios/debug.
    }

    public void RespawnBomb()
    {
        transform.position = spawnPoint.position;

        currentSpeed = speed;
        
        angle = Random.Range(0, 361);
        Debug.Log(angle);
        angle = Mathf.Abs(angle);
        transform.eulerAngles = new Vector3(0, 0, angle);
        
        direction = transform.right;

        rb.linearVelocity = direction * currentSpeed;
    }
}

/*

using UnityEngine;

public class BallScript : MonoBehaviour
{

Rigidbody2D rb;
// Start is called once before the first execution of Update after the MonoBehaviour is created
void Start()
{
rb = GetComponent<Rigidbody2D>();
rb.linearVelocity = new Vector2(0, 1).normalized * 5f; // Initial velocity of the ball
}

// Update is called once per frame
void Update()
{
}

void FixedUpdate()
{
}

void OnCollisionEnter2D(Collision2D collision)
{
rb.linearVelocity = Vector2.down * 5f;
Debug.Log("Velocity: " + rb.linearVelocity);
}
*/