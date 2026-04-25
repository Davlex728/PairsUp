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

    [Header("Suelo Salto")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("RECETA")]
    public TipoIngrediente[] recetaObjetivo;
    private int pasoActual = 0;
    public int recetasCompletadas = 0;

    [Header("Condición Victoria")]
    public int recetasParaGanar = 1;

    private bool juegoTerminado = false;
    private Animator animator;

    [HideInInspector]
    public UIReceta miUI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("Caldero", true);
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        miMando = mando;
    }

    public void IniciarUI()
    {
        if (miUI != null)
            miUI.ActualizarTexto(recetaObjetivo, pasoActual, recetasCompletadas);
    }

    private void Update()
    {
        if (miMando == null || juegoTerminado) return;

        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (miMando.isJumping && isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            miMando.isJumping = false;
            animator.SetBool("isJumping", true);
        }
    }

    private void FixedUpdate()
    {
        if (miMando == null || juegoTerminado)
        {
            if (juegoTerminado) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float movimientoX = miMando.moveInput.x;
        rb.linearVelocity = new Vector2(movimientoX * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (juegoTerminado) return;

        if (collision.TryGetComponent(out Ingrediente ingrediente))
        {
            TipoIngrediente ingredienteQueNecesito = recetaObjetivo[pasoActual];

            if (ingrediente.miTipo == ingredienteQueNecesito)
            {
                animator.SetTrigger("correctIngredient");
                pasoActual++;
                Debug.Log($"¡Bien! Has cogido {ingrediente.miTipo}. Faltan {recetaObjetivo.Length - pasoActual} ingredientes.");

                if (pasoActual >= recetaObjetivo.Length)
                {
                    recetasCompletadas++;
                    Debug.Log($"¡POCIÓN COMPLETADA! Llevas {recetasCompletadas} hechas.");
                    pasoActual = 0;

                    if (recetasCompletadas >= recetasParaGanar)
                    {
                        juegoTerminado = true;
                        CalderoManager.Instance.DeclararVictoria(gameObject);
                    }
                }
            }
            else
            {
                Debug.Log($"¡Error! Has cogido {ingrediente.miTipo} pero necesitabas {ingredienteQueNecesito}. ¡Receta arruinada!");
                animator.SetTrigger("wrongIngredient");
                pasoActual = 0;
            }

            if (miUI != null)
                miUI.ActualizarTexto(recetaObjetivo, pasoActual, recetasCompletadas);

            Destroy(collision.gameObject);
        }
    }
}