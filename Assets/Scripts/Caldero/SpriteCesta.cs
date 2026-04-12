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

    [Header("--- LA RECETA ---")]
    public TipoIngrediente[] recetaObjetivo; // El orden exacto de la "receta"
    private int pasoActual = 0;              // Por qué paso de la receta vamos
    public int recetasCompletadas = 0;       // mas de una combinacion cuenta

    // --- AÑADIDO PARA LA UI ---
    [HideInInspector]
    public UIReceta miUI; // El Manager nos pasará esta referencia automáticamente

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        miMando = mando;
    }

    // --- AÑADIDO PARA LA UI: Fuerza el texto a aparecer al empezar ---
    public void IniciarUI()
    {
        if (miUI != null)
        {
            miUI.ActualizarTexto(recetaObjetivo, pasoActual, recetasCompletadas);
        }
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
            miMando.isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        if (miMando == null) return;

        float movimientoX = miMando.moveInput.x;
        rb.linearVelocity = new Vector2(movimientoX * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Ingrediente>(out var ingrediente))
        {
            //  ingrediente  que toca atrapar ahora mismo
            TipoIngrediente ingredienteQueNecesito = recetaObjetivo[pasoActual];

            if (ingrediente.miTipo == ingredienteQueNecesito)
            {
                // si acierta
                pasoActual++; //siguiente paso
                Debug.Log($"¡Bien! Has cogido {ingrediente.miTipo}. Faltan {recetaObjetivo.Length - pasoActual} ingredientes.");

                // ha terminado toda la receta?
                if (pasoActual >= recetaObjetivo.Length)
                {
                    recetasCompletadas++;
                    Debug.Log($"¡POCIÓN COMPLETADA! Llevas {recetasCompletadas} hechas.");
                    pasoActual = 0; // Reiniciamos para hacer otra receta
                }
            }
            else
            {
                // el juagdor ha fallado, ha cogido un ingrediente que no era el que necesitaba
                Debug.Log($"¡Error! Has cogido {ingrediente.miTipo} pero necesitabas {ingredienteQueNecesito}. ¡Receta arruinada!");
                pasoActual = 0; // vuelta al paso 0 se podria hacer reroll en el futuro para que no sea siempre la misma receta
            }

            // Temporal para alpha
            if (miUI != null)
            {
                miUI.ActualizarTexto(recetaObjetivo, pasoActual, recetasCompletadas);
            }

            // Destruimos el ingrediente al chocar
            Destroy(collision.gameObject);
        }
    }
}