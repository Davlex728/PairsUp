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

    [Header("Knockback")]
    public float duracionKnockback = 0.3f;

    [Header("Referencias")]
    public PlayerInputHandler miMando;
    private Rigidbody2D rb;

    private float tiempoUltimaAccion = 0f;
    private float knockbackTimer = 0f;
    private System.Collections.Generic.List<GameObject> bloquesActivos = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
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

        if (bloquesActivos.Count >= maxBloques)
        {
            Destroy(bloquesActivos[0]);
            bloquesActivos.RemoveAt(0);
        }

        Vector3 posSnap = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
        GameObject nuevo = Instantiate(prefab, posSnap, Quaternion.identity);
        bloquesActivos.Add(nuevo);
    }

    private void BorrarBloque()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, 1.2f, capaConstruible);
        if (col != null && bloquesActivos.Contains(col.gameObject))
        {
            bloquesActivos.Remove(col.gameObject);
            Destroy(col.gameObject);
        }
    }

    public void CambiarHerramienta(HerramientaMano nueva) => herramientaActual = nueva;

    private void CambiarHerramientaSiguiente()
    {
        herramientaActual = (HerramientaMano)(((int)herramientaActual + 1) % System.Enum.GetValues(typeof(HerramientaMano)).Length);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}