using UnityEngine;
using UnityEngine.UI;

public class CuentaAtrasImagenes : MonoBehaviour
{
    [SerializeField] private float tiempoAntesConteo = 2f;

    public Image imagen3;
    public Image imagen2;
    public Image imagen1;

    private float timer;
    private int estado = 3;
    private bool empezoConteo = false;

    void Start()
    {
        // Pausar juego
        Time.timeScale = 0f;

        // Ocultar imágenes al inicio
        imagen3.gameObject.SetActive(false);
        imagen2.gameObject.SetActive(false);
        imagen1.gameObject.SetActive(false);

        // Tiempo de espera antes del conteo
        timer = tiempoAntesConteo;
    }

    void Update()
    {
        // Tiempo real aunque el juego esté pausado
        timer -= Time.unscaledDeltaTime;

        if (timer <= 0f)
        {
            // Empezar conteo después de la transición
            if (!empezoConteo)
            {
                empezoConteo = true;

                imagen3.gameObject.SetActive(true);

                timer = 1f;
                return;
            }

            estado--;

            if (estado == 2)
            {
                imagen3.gameObject.SetActive(false);
                imagen2.gameObject.SetActive(true);
            }
            else if (estado == 1)
            {
                imagen2.gameObject.SetActive(false);
                imagen1.gameObject.SetActive(true);
            }
            else if (estado == 0)
            {
                imagen1.gameObject.SetActive(false);

                // Reanudar juego
                Time.timeScale = 1f;

                enabled = false;
            }

            timer = 1f;
        }
    }
}