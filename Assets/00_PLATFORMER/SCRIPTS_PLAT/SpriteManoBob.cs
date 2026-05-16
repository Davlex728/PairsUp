using System.Collections.Generic;
using UnityEngine;

public enum HerramientaMano { Suelo, Pared, Borrar }

[RequireComponent(typeof(Rigidbody2D))]
public class SpriteManoBob : MonoBehaviour
{
    [Header("Configuración Mano")]
    public float flySpeed = 8f;

    [Header("Construcción")]
    public HerramientaMano herramientaActual = HerramientaMano.Suelo;
    public GameObject prefabSuelo;
    public GameObject prefabPared;
    public LayerMask capaConstruible;
    public int maxBloques = 8;
    public float cooldown = 0.3f;

    [Header("Preview")]
    public SpriteRenderer spritePreview;

    [Header("Knockback")]
    public float duracionKnockback = 0.3f;

    [Header("Referencias")]
    public PlayerInputHandler miMando;
    private Rigidbody2D rb;
    private Animator animator;

    private float tiempoUltimaAccion = 0f;
    private float knockbackTimer = 0f;
    private List<GameObject> bloquesActivos = new List<GameObject>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        animator = GetComponent<Animator>();

        // Crear SpriteRenderer para preview si no existe
        if (spritePreview == null)
        {
            GameObject previewObj = new GameObject("PreviewBloque");
            previewObj.transform.SetParent(transform);
            previewObj.transform.localPosition = Vector3.zero;
            previewObj.transform.localScale = Vector3.one;

            spritePreview = previewObj.AddComponent<SpriteRenderer>();
            spritePreview.sortingOrder = -1; // Que aparezca detrás de la mano
        }
    }

    private void Start()
    {
        // No llamar aquí, esperar a que el Manager lo haga
    }

    public void InicializarPreview()
    {
        ActualizarPreview();
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
        if (miMando == null) return;

        if (miMando.isSwitchingTool)
        {
            CambiarHerramientaSiguiente();
            miMando.isSwitchingTool = false;
        }

        // Actualizar posición del preview en tiempo real
        if (spritePreview != null && spritePreview.enabled)
        {
            Vector3 posSnap = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
            spritePreview.transform.position = posSnap;
        }

        if (miMando.isGrabbing && Time.time > tiempoUltimaAccion + cooldown)
        {
            EjecutarAccion();
            tiempoUltimaAccion = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (miMando == null) return;

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        rb.linearVelocity = miMando.moveInput * flySpeed;
    }

    private void EjecutarAccion()
    {
        switch (herramientaActual)
        {
            case HerramientaMano.Suelo: ColocarBloque(prefabSuelo); break;
            case HerramientaMano.Pared: ColocarBloque(prefabPared); break;
            case HerramientaMano.Borrar: BorrarBloque(); break;
        }
    }

    private void ColocarBloque(GameObject prefab)
    {
        if (prefab == null) return;

        // Limpiamos los bloques que hayan podido ser destruidos por otros jugadores(evitar capeo falso)
        bloquesActivos.RemoveAll(b => b == null);

        if (bloquesActivos.Count >= maxBloques)
        {
            if (bloquesActivos[0] != null)
                Destroy(bloquesActivos[0]);

            bloquesActivos.RemoveAt(0);
        }

        Vector3 posSnap = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
        GameObject nuevo = Instantiate(prefab, posSnap, Quaternion.identity);

        // Rotar el bloque según el modo de construcción
        if (herramientaActual == HerramientaMano.Pared)
            nuevo.transform.rotation = Quaternion.Euler(0, 0, 90f); // Horizontal (90°)
        else if (herramientaActual == HerramientaMano.Suelo)
            nuevo.transform.rotation = Quaternion.identity; // Vertical (0°)

        bloquesActivos.Add(nuevo);
    }

    private void BorrarBloque()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, 1.2f, capaConstruible);

        // Ya NO comprobamos a quién le pertenece el bloque 
        if (col != null)
        {
            // Lo quitamos de nuestra lista por si acaso resulta que si  era de la lista 
            if (bloquesActivos.Contains(col.gameObject))
            {
                bloquesActivos.Remove(col.gameObject);
            }

            // Destruimos el bloque sin piedad
            Destroy(col.gameObject);
        }
    }

    public void CambiarHerramienta(HerramientaMano nueva) => herramientaActual = nueva;

    private void CambiarHerramientaSiguiente()
    {
        herramientaActual = (HerramientaMano)(((int)herramientaActual + 1) % System.Enum.GetValues(typeof(HerramientaMano)).Length);
        ActualizarPreview();
    }

    private void ActualizarPreview()
    {
        if (spritePreview == null)
        {
            Debug.LogWarning("[SpriteManoBob] spritePreview no está asignado");
            return;
        }

        if (animator == null)
        {
            Debug.LogWarning("[SpriteManoBob] animator no está encontrado");
            return;
        }

        if (herramientaActual == HerramientaMano.Borrar)
        {
            spritePreview.enabled = false;
            animator.SetBool("buildAnim", false);
            animator.SetBool("deleteAnim", true);
        }
        else if (herramientaActual == HerramientaMano.Suelo)
        {
            if (spritePreview.sprite == null && prefabSuelo != null && prefabSuelo.TryGetComponent(out SpriteRenderer srSuelo))
                spritePreview.sprite = srSuelo.sprite;

            spritePreview.enabled = true;
            spritePreview.transform.rotation = Quaternion.identity; // Vertical (0°)
            animator.SetBool("buildAnim", true);
            animator.SetBool("deleteAnim", false);
        }
        else if (herramientaActual == HerramientaMano.Pared)
        {
            if (spritePreview.sprite == null && prefabPared != null && prefabPared.TryGetComponent(out SpriteRenderer srPared))
                spritePreview.sprite = srPared.sprite;

            spritePreview.enabled = true;
            spritePreview.transform.rotation = Quaternion.Euler(0, 0, 90f); // Horizontal (90°)
            animator.SetBool("buildAnim", true);
            animator.SetBool("deleteAnim", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}