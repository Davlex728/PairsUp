using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Victoria")]
    public float tiempoEspera = 2f;
    public string[] escenasAleatorias;
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
        if (miMando == null || juegoTerminado) { rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); return; }
        rb.linearVelocity = new Vector2(miMando.moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tag "Meta" para el objeto que usemos de meta lo podremos reutilizar para el laberinto imagino
        if (!juegoTerminado && collision.CompareTag("Meta"))
            DeclararVictoria();
    }

    private void DeclararVictoria()
    {
        juegoTerminado = true;
        Debug.Log("Has ganado chacho");
        Invoke(nameof(CargarEscenaAleatoria), tiempoEspera);
    }

    private void CargarEscenaAleatoria()
    {
        if (escenasAleatorias == null || escenasAleatorias.Length == 0) return;
        int i = Random.Range(0, escenasAleatorias.Length);
        SceneManager.LoadScene(escenasAleatorias[i]);
    }
}