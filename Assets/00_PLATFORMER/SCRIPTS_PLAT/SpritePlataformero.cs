using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpritePlatformero : MonoBehaviour
{
    [Header("Configuración Movimiento")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Acciones")]
    public float rangoAccion = 1.5f;
    public float fuerzaEmpujon = 12f;
    public float cooldownEmpujon = 1f;

    [Header("Knockback")]
    public float duracionKnockback = 0.3f;

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

    private bool tieneBandera = false;
    private GameObject banderaObj;
    private GameObject banderaOrigen;

    private float tiempoUltimoEmpujon = -99f;
    private float knockbackTimer = 0f;
    private bool switchWasPressed = false;
    private bool grabWasPressed = false;
    private bool juegoTerminado = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void ConectarMando(PlayerInputHandler mando) => miMando = mando;

    public void RecibirKnockback(Vector2 fuerza)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(fuerza, ForceMode2D.Impulse);
        knockbackTimer = duracionKnockback;
    }

    private void Update()
    {
        if (miMando == null || juegoTerminado)
        {
            if (animator != null && juegoTerminado)
            {
                animator.SetFloat("speedAnim", 0f);
            }
            return;
        }

        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // 
        if (miMando.isJumping && isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            miMando.isJumping = false;
            isGrounded = false; // se fuerza un poco antes para el animator
        }

        if (tieneBandera && banderaObj != null)
            banderaObj.transform.position = transform.position + Vector3.up * 1f;

        bool switchDown = miMando.isSwitchingTool && !switchWasPressed;
        switchWasPressed = miMando.isSwitchingTool;
        if (switchDown) IntentarRecogerBandera();

        bool grabDown = miMando.isGrabbing && !grabWasPressed;
        grabWasPressed = miMando.isGrabbing;

        if (grabDown) Empujar();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (miMando == null || juegoTerminado)
        {
            if (juegoTerminado) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        rb.linearVelocity = new Vector2(miMando.moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;


        animator.SetBool("isGroundedAnim", isGrounded);

        // Float velocidad horizontal (abs es para que no importe la direccion (anula +-))
        animator.SetFloat("speedAnim", Mathf.Abs(miMando.moveInput.x));

        // Float  velocidad vertical
        animator.SetFloat("ySpeedAnim", rb.linearVelocity.y);

        // flip de sprite no nse si hace falta por que no he mirado
        if (sr != null)
        {
            if (miMando.moveInput.x > 0.1f)
                sr.flipX = false;
            else if (miMando.moveInput.x < -0.1f)
                sr.flipX = true;
        }
    }

    private void IntentarRecogerBandera()
    {
        if (tieneBandera) return;

        if (banderaOrigen == null)
            banderaOrigen = GameObject.FindWithTag("BanderaOrigen");

        foreach (Collider2D col in GetCercanos())
        {
            if (col.CompareTag("Bandera"))
            {
                tieneBandera = true;
                banderaObj = col.gameObject;
                Collider2D c = banderaObj.GetComponent<Collider2D>();
                if (c != null) c.enabled = false;
                Debug.Log($"[Platformero] {gameObject.name} recogió la bandera.");
                return;
            }
        }
        Debug.Log("[Platformero] No hay bandera en rango.");
    }

    private void Empujar()
    {
        if (Time.time < tiempoUltimoEmpujon + cooldownEmpujon)
        {
            Debug.Log("[Platformero] Empujón en cooldown.");
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("pushAnim");
        }

        Rigidbody2D objetivoRb = null;
        GameObject objetivoGO = null;
        float menorDist = float.MaxValue;

        foreach (Collider2D col in GetCercanos())
        {
            if (col.gameObject == gameObject) continue;
            if (col.CompareTag("Bandera")) continue;

            Rigidbody2D rbOtro = col.GetComponent<Rigidbody2D>();
            if (rbOtro != null)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < menorDist) { menorDist = dist; objetivoRb = rbOtro; objetivoGO = col.gameObject; }
            }
        }

        if (objetivoRb == null) { Debug.Log("[Platformero] Nadie en rango."); return; }

        Vector2 dir = ((Vector2)objetivoRb.transform.position - (Vector2)transform.position).normalized;
        Vector2 fuerza = dir * fuerzaEmpujon;

        SpriteManoBob mano = objetivoGO.GetComponent<SpriteManoBob>();
        if (mano != null)
            mano.RecibirKnockback(fuerza);
        else
        {
            SpritePlatformero otroPlatformero = objetivoGO.GetComponent<SpritePlatformero>();
            if (otroPlatformero != null)
                otroPlatformero.RecibirKnockback(fuerza);
            else
                objetivoRb.AddForce(fuerza, ForceMode2D.Impulse);
        }

        SpritePlatformero victima = objetivoGO.GetComponent<SpritePlatformero>();
        if (victima != null && victima.tieneBandera) victima.SoltarBandera();

        tiempoUltimoEmpujon = Time.time;
        Debug.Log($"[Platformero] {gameObject.name} empujó a {objetivoGO.name}.");
    }

    public void SoltarBandera()
    {
        if (!tieneBandera) return;
        tieneBandera = false;
        if (banderaObj != null)
        {
            Collider2D c = banderaObj.GetComponent<Collider2D>();
            if (c != null) c.enabled = true;
            if (banderaOrigen != null)
                banderaObj.transform.position = banderaOrigen.transform.position;
            banderaObj = null;
        }
        Debug.Log($"[Platformero] {gameObject.name} soltó la bandera.");
    }

    private Collider2D[] GetCercanos()
    {
        ContactFilter2D filtro = new ContactFilter2D();
        filtro.NoFilter();
        filtro.useTriggers = true;
        Collider2D[] buffer = new Collider2D[20];
        int count = Physics2D.OverlapCircle(transform.position, rangoAccion, filtro, buffer);
        Collider2D[] resultado = new Collider2D[count];
        System.Array.Copy(buffer, resultado, count);
        return resultado;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (juegoTerminado) return;
        if (collision.CompareTag("Base") && tieneBandera)
        {
            juegoTerminado = true;
            Debug.Log($"[Platformero] {gameObject.name} llegó a la base. ¡Victoria!");
            PlataformeoManager.Instance?.DeclararVictoria(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAccion);
    }
}