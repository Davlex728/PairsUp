using UnityEngine;

public enum HerramientaMano { Suelo, Pared, Borrar }

[RequireComponent(typeof(Rigidbody2D))]
public class SpriteManoBob : MonoBehaviour
{
    [Header("Configuración Mano")]
    public float flySpeed = 8f;
    public float limiteY = 2f;

    [Header("Construcción")]
    public HerramientaMano herramientaActual = HerramientaMano.Suelo;
    public GameObject prefabSuelo;
    public GameObject prefabPared;
    public LayerMask capaConstruible;
    public int maxBloques = 8;
    public float cooldown = 0.3f;

    [Header("Referencias")]
    public PlayerInputHandler miMando;
    private Rigidbody2D rb;

    private float tiempoUltimaAccion = 0f;
    private System.Collections.Generic.List<GameObject> bloquesActivos = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    public void ConectarMando(PlayerInputHandler mando) => miMando = mando;

    private void Update()
    {
        if (miMando == null) return;

        // Cambio de herramienta
        if (miMando.isSwitchingTool)
        {
            CambiarHerramientaSiguiente();
            miMando.isSwitchingTool = false;
        }

        // Construcción o destrucción
        if (miMando.isGrabbing && Time.time > tiempoUltimaAccion + cooldown)
        {
            EjecutarAccion();
            tiempoUltimaAccion = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (miMando == null) return;

        rb.linearVelocity = miMando.moveInput * flySpeed;

        if (transform.position.y < limiteY)
        {
            transform.position = new Vector3(transform.position.x, limiteY, transform.position.z);
            if (rb.linearVelocity.y < 0)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    private void EjecutarAccion()
    {
        Debug.Log($"[EjecutarAccion] herramienta: {herramientaActual}");
        switch (herramientaActual)
        {
            case HerramientaMano.Suelo:
                ColocarBloque(prefabSuelo);
                break;
            case HerramientaMano.Pared:
                ColocarBloque(prefabPared);
                break;
            case HerramientaMano.Borrar:
                BorrarBloque();
                break;
        }
    }

    private void ColocarBloque(GameObject prefab)
    {
        Debug.Log($"[ColocarBloque] prefab recibido: {(prefab == null ? "NULL" : prefab.name)} | herramienta: {herramientaActual}");
        if (prefab == null) return;

        if (bloquesActivos.Count >= maxBloques)
        {
            Destroy(bloquesActivos[0]);
            bloquesActivos.RemoveAt(0);
        }

        Vector3 posSnap = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            0f
        );

        GameObject nuevo = Instantiate(prefab, posSnap, Quaternion.identity);
        bloquesActivos.Add(nuevo);
        Debug.Log($"[Mano] Colocado {prefab.name} en {posSnap}");
    }

    private void BorrarBloque()
    {
        Debug.Log("[BorrarBloque] intentando borrar...");
        Collider2D col = Physics2D.OverlapCircle(transform.position, 1.2f, capaConstruible);
        if (col != null && bloquesActivos.Contains(col.gameObject))
        {
            bloquesActivos.Remove(col.gameObject);
            Destroy(col.gameObject);
            Debug.Log("[Mano] Bloque borrado.");
        }
        else
        {
            Debug.Log($"[BorrarBloque] nada encontrado. col={col}");
        }
    }

    public void CambiarHerramienta(HerramientaMano nueva)
    {
        herramientaActual = nueva;
        Debug.Log($"[Mano] Herramienta: {nueva}");
    }

    private void CambiarHerramientaSiguiente()
    {
        herramientaActual = (HerramientaMano)(((int)herramientaActual + 1) % System.Enum.GetValues(typeof(HerramientaMano)).Length);
        Debug.Log($"[Mano] Herramienta cambiada a: {herramientaActual}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
    }
}