using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpriteCesta : MonoBehaviour
{
    [Header("Configuración Cesta")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Referencias")]
    public PlayerInputHandler miMando;
    private Rigidbody2D rb;

    [Header("Suelo")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        miMando = mando;
        Debug.Log($"[CestaSprite] Mando de {mando.gameObject.name} conectado.");
    }

    private void Update()
    {
        if (miMando == null) return;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        }

        if (miMando.isJumping && isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            miMando.isJumping = false; // solo 1 salto
        }
    }

    private void FixedUpdate()
    {
        if (miMando == null) return;

        float movimientoX = miMando.moveInput.x;
        rb.linearVelocity = new Vector2(movimientoX * moveSpeed, rb.linearVelocity.y);
    }
}