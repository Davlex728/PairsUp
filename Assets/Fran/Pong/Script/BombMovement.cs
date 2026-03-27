using UnityEngine;

public class BombMovement : MonoBehaviour
{
    private int angle;
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    public Vector2 direction;

    public Vector2 newVelocity;

    private float currentSpeed;

    [SerializeField] private float speedIncreasePercentage;
    
    private float speedIncreasePerHit;
    

    [SerializeField] private float maxSpeed;


// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = speed;
        
        speedIncreasePerHit =  speedIncreasePercentage / 100f; 
        angle = Random.Range(0, 361);
        angle = Mathf.Abs(angle);
        transform.eulerAngles = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody2D>();

//Como lo has rotado, ahora hay que ir a la derecha
        direction = transform.right;
//Acuerdate de asignarle una velocidad inicial en el editor.
//Esto hace que empiece moviéndose
        rb.linearVelocity = direction * currentSpeed; // Initial velocity of the ball
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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

        currentSpeed *= (1 + speedIncreasePerHit);
        
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        newVelocity = newVelocity.normalized * currentSpeed;
        rb.linearVelocity = newVelocity;

        // Mantén 'direction' coherente si lo usas en otros sitios/debug.
        direction = rb.linearVelocity.normalized;
    }

    public void CalculateNewDirection()
    {
        //bomb.localPosition= (2 * transform.localPosition);
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