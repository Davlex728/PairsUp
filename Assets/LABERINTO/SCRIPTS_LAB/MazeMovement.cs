using UnityEngine;

public class MazeMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    
    Rigidbody2D rb;
    
    public PlayerInputHandler mandoMovimiento;
    private bool canMove = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        mandoMovimiento = mando;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 input = mandoMovimiento.moveInput;
        if (canMove)
        {
            rb.linearVelocity = new Vector2(input.x * speed, input.y * speed);
        }
    }

    void Update()
    {
        
    }
}
