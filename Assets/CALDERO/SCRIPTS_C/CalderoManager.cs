using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalderoManager : MonoBehaviour
{
    public static CalderoManager Instance { get; private set; }

    [Header("Prefabs Sprites")]
    public GameObject prefabCesta;
    public GameObject prefabMano;

    [Header("Animators Jugadores")]
    public RuntimeAnimatorController[] animators;

    [Header("Spawn Points")]
    public Transform[] spawnPointsCestas;
    public Transform[] spawnPointsManos;

    [Header("UI (Textos de la pantalla)")]
    public UIReceta[] panelesUIParejas;

    [Header("Condición de Victoria")]
    [SerializeField] private float tiempoEsperaVictoria = 2f;
    [SerializeField] private string[] escenasAleatorias;

    private bool juegoTerminado = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);

        if (jugadoresConectados.Length == 0)
        {
            Debug.LogWarning("[CalderoManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        int contadorJugadores = 0;

        foreach (PersistentPlayer mandoFantasma in jugadoresConectados)
        {
            PlayerInputHandler lectorBotones = mandoFantasma.GetComponent<PlayerInputHandler>();

            if (contadorJugadores % 2 == 0)
                SpawnearCesta(contadorJugadores, lectorBotones);
            else
                SpawnearMano(contadorJugadores, lectorBotones);

            contadorJugadores++;
        }
    }

    // Llamado por SpriteCesta cuando completa las recetas necesarias
    public void DeclararVictoria(GameObject ganador)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Debug.Log($"[CalderoManager] ¡VICTORIA! Ganador: {ganador.name}");

        StartCoroutine(EsperarYCargarEscena());
    }

    private IEnumerator EsperarYCargarEscena()
    {
        yield return new WaitForSeconds(tiempoEsperaVictoria);
        CargarEscenaAleatoria();
    }

    private void CargarEscenaAleatoria()
    {
        if (escenasAleatorias == null || escenasAleatorias.Length == 0)
        {
            Debug.LogError("[CalderoManager] ERROR: No hay escenas configuradas en 'Escenas Aleatorias'.");
            return;
        }

        int indiceAleatorio = Random.Range(0, escenasAleatorias.Length);
        string escenaElegida = escenasAleatorias[indiceAleatorio];

        if (string.IsNullOrEmpty(escenaElegida))
        {
            Debug.LogError($"[CalderoManager] ERROR: El hueco {indiceAleatorio} del array de escenas está vacío.");
            return;
        }

        Debug.Log($"[CalderoManager] Cargando escena: {escenaElegida}");
        SceneManager.LoadScene(escenaElegida);
    }

    private void SpawnearCesta(int idJugador, PlayerInputHandler mando)
    {
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsCestas[indexSpawn];

        GameObject sprite = Instantiate(prefabCesta, puntoSpawn.position, Quaternion.identity);

        if (sprite.TryGetComponent(out SpriteCesta scriptCesta))
        {
            scriptCesta.ConectarMando(mando);

            if (panelesUIParejas.Length > indexSpawn && panelesUIParejas[indexSpawn] != null)
            {
                scriptCesta.miUI = panelesUIParejas[indexSpawn];
                scriptCesta.IniciarUI();
            }
        }

        if (sprite.TryGetComponent(out Animator animator))
        {
            if (animators.Length > idJugador && animators[idJugador] != null)
                animator.runtimeAnimatorController = animators[idJugador];
        }
    }

    private void SpawnearMano(int idJugador, PlayerInputHandler mando)
    {
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsManos[indexSpawn];

        GameObject avatar = Instantiate(prefabMano, puntoSpawn.position, Quaternion.identity);

        if (avatar.TryGetComponent(out SpriteMano scriptMano))
            scriptMano.ConectarMando(mando);

        if (avatar.TryGetComponent(out Animator animator))
        {
            if (animators.Length > idJugador && animators[idJugador] != null)
                animator.runtimeAnimatorController = animators[idJugador];
        }
    }
}