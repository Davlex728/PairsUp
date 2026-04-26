using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpritePlatformero : MonoBehaviour
{
    [Header("Configuración Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Referencias")]
    public PlayerInputHandler miMando;
    private Rigidbody2D rb;

    [Header("Suelo Salto")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    private bool juegoTerminado = false;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void ConectarMando(PlayerInputHandler mando) => miMando = mando;

    private void Update()
    {
        if (miMando == null || juegoTerminado) return;

        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (miMando.isJumping && isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            miMando.isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        if (miMando == null || juegoTerminado)
        {
            if (juegoTerminado) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        rb.linearVelocity = new Vector2(miMando.moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (juegoTerminado) return;

        if (collision.CompareTag("Meta"))
        {
            juegoTerminado = true;
            PlataformeoManager.Instance.DeclararVictoria(gameObject);
        }
    }
}