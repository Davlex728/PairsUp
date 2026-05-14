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

    [Header("Efectos")]
    public float blinkDuration = 1f;
    public float blinkRate = 0.1f;
    public float flashDuration = 0.4f;
    private Coroutine corrutinaEfecto;

    [Header("Rebote")]
    public float bounceForce = 5f;
    public float bounceDuration = 0.5f;
    public float escalaRebote = 1.3f;
    public float duracionEscalaRebote = 0.2f;
    private bool isBouncing = false;

    [HideInInspector] public UIReceta miUI;
    [HideInInspector] public int puntos = 0;
    [HideInInspector] public TipoIngrediente IngredienteObjetivo { get; private set; }

    private bool juegoTerminado = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void ConectarMando(PlayerInputHandler mando) => miMando = mando;

    public void AsignarObjetivo(TipoIngrediente tipo)
    {
        IngredienteObjetivo = tipo;
        if (miUI != null) miUI.MostrarIngredienteObjetivo(tipo);
    }

    public void IniciarUI()
    {
        if (miUI != null) miUI.ActualizarPuntos(puntos);
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
        rb.linearVelocity = new Vector2(miMando.moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (juegoTerminado) return;

        if (collision.TryGetComponent(out Ingrediente ingrediente))
        {
            bool esObjetivo = ingrediente.miTipo == IngredienteObjetivo;

            if (esObjetivo)
            {
                puntos++;


                Debug.Log($"[Cesta] ¡Correcto! {ingrediente.miTipo}. Puntos: {puntos}");
                if (corrutinaEfecto != null) StopCoroutine(corrutinaEfecto);
                sr.enabled = true;
                corrutinaEfecto = StartCoroutine(Flash(new Color(0.3f, 1f, 0.3f)));

                CalderoManager.Instance.ElegirNuevoObjetivoParaCesta(this);
                CalderoManager.Instance.tiempoSiguienteObjetivo = CalderoManager.Instance.intervaloNuevoObjetivo;
            }
            else
            {
                puntos = Mathf.Max(0, puntos - 1);



                Debug.Log($"[Cesta] ¡Incorrecto! {ingrediente.miTipo}. Puntos: {puntos}");
                if (corrutinaEfecto != null) StopCoroutine(corrutinaEfecto);
                sr.enabled = true;
                corrutinaEfecto = StartCoroutine(Parpadear());
            }

            if (miUI != null) miUI.ActualizarPuntos(puntos);
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBouncing) return;
        if (!collision.gameObject.TryGetComponent(out SpriteCesta _)) return;

        Vector2 dir = (transform.position - collision.transform.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * bounceForce, ForceMode2D.Impulse);

        StartCoroutine(TemporizadorRebote());
        StartCoroutine(EfectoEscalaRebote());
    }

    private IEnumerator Flash(Color color)
    {
        Color colorAnterior = sr.color;
        sr.color = color;
        yield return new WaitForSeconds(flashDuration);
        sr.color = colorAnterior;
    }

    private IEnumerator Parpadear()
    {
        Color colorAnterior = sr.color;
        float timer = 0f;
        while (timer < blinkDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkRate);
            timer += blinkRate;
        }
        sr.enabled = true;
        sr.color = colorAnterior;
    }

    private IEnumerator TemporizadorRebote()
    {
        isBouncing = true;
        yield return new WaitForSeconds(bounceDuration);
        isBouncing = false;
    }

    private IEnumerator EfectoEscalaRebote()
    {
        Vector3 original = transform.localScale;
        transform.localScale = original * escalaRebote;
        float timer = 0f;
        while (timer < duracionEscalaRebote)
        {
            transform.localScale = Vector3.Lerp(original * escalaRebote, original, timer / duracionEscalaRebote);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = original;
    }
    private void Start()
    {
        miMando.LastBullet();
        Debug.Log(miMando.teamIndex);
    }
}