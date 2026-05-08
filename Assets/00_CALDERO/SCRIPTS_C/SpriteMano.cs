using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpriteMano : MonoBehaviour
{
    [Header("Configuración Mano")]
    public float flySpeed = 8f;
    // public float limiteY = 2f;

    [Header("Configuración Agarre")]
    public float radioDeAgarre = 1.5f;
    public LayerMask capaIngredientes;

    [Header("Referencias")]
    public PlayerInputHandler miMando;
    private Rigidbody2D rb;

    private Rigidbody2D objetoAgarrado;
    private Animator animator;

    private bool isAgarrandoHugo = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        animator = GetComponent<Animator>();
        animator.SetBool("Caldero", true);
    }

    public void ConectarMando(PlayerInputHandler mando)
    {
        miMando = mando;
    }

    private void Update()
    {
        if (miMando == null) return;

        if (miMando.isGrabbing)
        {
            if (objetoAgarrado == null)
            {
                IntentarAgarrar();
            }
            else
            {
                objetoAgarrado.position = transform.position;
            }
        }
        else
        {
            if (objetoAgarrado != null)
            {
                SoltarObjeto();
            }
        }
    }

    private void FixedUpdate()
    {
        if (miMando == null) return;

        Vector2 movimiento = miMando.moveInput;
        rb.linearVelocity = movimiento * flySpeed;

        /* if (transform.position.y < limiteY)
         {
             transform.position = new Vector3(transform.position.x, limiteY, transform.position.z);
             if (rb.linearVelocity.y < 0)

             {
                 rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
             }
         }*/
        Collider2D coliderDeHugoParaLaMano = Physics2D.OverlapCircle(transform.position, radioDeAgarre, capaIngredientes);
        if (coliderDeHugoParaLaMano != null && !isAgarrandoHugo)
        {
            animator.SetBool("isHovering", true);
        }
        else
        {
            animator.SetBool("isHovering", false);
        }
    }

    private void IntentarAgarrar()
    {
        // para saber  si el botón funciona quitar al final
        Debug.Log("Intentando agarrar... El botón funciona.");

        Collider2D colision = Physics2D.OverlapCircle(transform.position, radioDeAgarre, capaIngredientes);

        if (colision != null)
        {
            // detectar si ha tocado algo quitar al final si eso
            Debug.Log($"¡He tocado algo llamado {colision.gameObject.name}!");

            objetoAgarrado = colision.GetComponent<Rigidbody2D>();

            if (objetoAgarrado != null)
            {
                // si lo agarra en algun momento lo mismo es debug
                Debug.Log("¡Objeto agarrado con éxito!");
                objetoAgarrado.gravityScale = 0f;
                objetoAgarrado.linearVelocity = Vector2.zero;
                animator.SetBool("isGrabbing", true);
                isAgarrandoHugo = true;
            }
            else
            {
                Debug.LogWarning("He tocado la fruta, pero NO tiene Rigidbody2D.");
            }
        }
    }

    private void SoltarObjeto()
    {
        Debug.Log("Soltando objeto...");
        objetoAgarrado.gravityScale = 1f;
        objetoAgarrado.linearVelocity = rb.linearVelocity * 0.5f;
        objetoAgarrado = null;
        animator.SetBool("isGrabbing", false);
        isAgarrandoHugo = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDeAgarre);
    }
}