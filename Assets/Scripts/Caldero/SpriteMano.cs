using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpriteMano : MonoBehaviour
{
    [Header("Configuración Mano")]
    public float flySpeed = 8f;
    public float limiteY = 2f;

    [Header("Referencias")]
    public PlayerInputHandler miMando;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        miMando = mando;
        Debug.Log($"[ManoSprite] Mando de {mando.gameObject.name} conectado.");
    }

    private void Update()
    {
        if (miMando == null) return;

        if (miMando.isGrabbing)
        {
            Debug.Log("Agarrar");
        }
    }

    private void FixedUpdate()
    {
        if (miMando == null) return;

        Vector2 movimiento = miMando.moveInput;
        rb.linearVelocity = movimiento * flySpeed;

        if (transform.position.y < limiteY)
        {
            transform.position = new Vector3(transform.position.x, limiteY, transform.position.z);
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
        }
    }
}