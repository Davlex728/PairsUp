using System.Collections;
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
    private Animator animator;
    private SpriteRenderer sr;

    [Header("Suelo Salto")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Efectos Ingrediente")]
    public float blinkDuration = 1f;
    public float blinkRate = 0.1f;
    public float flashVerdeDuration = 0.4f;
    private Coroutine corrutinaEfecto;

    [Header("Rebote")]
    public float bounceForce = 5f;
    public float bounceDuration = 0.5f;
    public float escalaRebote = 1.3f;//visua
    public float duracionEscalaRebote = 0.2f;//visual
    private bool isBouncing = false;

    [Header("RECETA")]
    public TipoIngrediente[] recetaObjetivo;
    private int pasoActual = 0;
    public int recetasCompletadas = 0;

    [Header("Condición Victoria")]
    public int recetasParaGanar = 1;

    private bool juegoTerminado = false;

    [HideInInspector]
    public UIReceta miUI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        //animator.SetBool("isGrabbing", true);
    }

    public void ConectarMando(PlayerInputHandler mando) => miMando = mando;

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
        if (miMando == null || juegoTerminado || isBouncing)
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
                
                pasoActual++;
                Debug.Log($"¡Bien! Has cogido {ingrediente.miTipo}. Faltan {recetaObjetivo.Length - pasoActual} ingredientes.");

                // Flash verde al acertar
                if (corrutinaEfecto != null) StopCoroutine(corrutinaEfecto);
                sr.color = Color.white;
                corrutinaEfecto = StartCoroutine(FlashVerde());

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
               
                pasoActual = 0;

                // Parpadeo al fallar
                if (corrutinaEfecto != null) StopCoroutine(corrutinaEfecto);
                sr.color = Color.white;
                corrutinaEfecto = StartCoroutine(Parpadear());
            }

            if (miUI != null)
                miUI.ActualizarTexto(recetaObjetivo, pasoActual, recetasCompletadas);

            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBouncing) return;
        if (!collision.gameObject.TryGetComponent(out SpriteCesta otraCesta)) return;

        Vector2 direccionRebote = (transform.position - collision.transform.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccionRebote * bounceForce, ForceMode2D.Impulse);

        StartCoroutine(TemporizadorRebote());
        StartCoroutine(EfectoEscalaRebote());
    }

    // las corrutinas para los efectos

    private IEnumerator Parpadear()
    {
        float timer = 0f;
        while (timer < blinkDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkRate);
            timer += blinkRate;
        }
        sr.enabled = true;
        sr.color = Color.white;
    }

    private IEnumerator FlashVerde()
    {
        sr.color = new Color(0.3f, 1f, 0.3f);
        yield return new WaitForSeconds(flashVerdeDuration);
        sr.color = Color.white;
    }

    private IEnumerator TemporizadorRebote()
    {
        isBouncing = true;
        yield return new WaitForSeconds(bounceDuration);
        isBouncing = false;
    }

    private IEnumerator EfectoEscalaRebote()
    {
        Vector3 escalaOriginal = transform.localScale;
        transform.localScale = escalaOriginal * escalaRebote;

        float timer = 0f;
        while (timer < duracionEscalaRebote)
        {
            transform.localScale = Vector3.Lerp(escalaOriginal * escalaRebote, escalaOriginal, timer / duracionEscalaRebote);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = escalaOriginal;
    }
     
    

}