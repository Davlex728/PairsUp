using UnityEngine;

public class MazeMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    
    Rigidbody2D rb;
    
    private PlayerInputHandler mandoMovimiento;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
