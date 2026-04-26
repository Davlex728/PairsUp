using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlataformeoManager : MonoBehaviour
{
    public static PlataformeoManager Instance { get; private set; }

    [Header("Prefabs Sprites")]
    public GameObject prefabPlatformero;
    public GameObject prefabMano;

    [Header("Animators Platformero")]
    public RuntimeAnimatorController[] animatorsPlatformero;

    [Header("Animators Mano")]
    public RuntimeAnimatorController[] animatorsMano;

    [Header("Spawn Points")]
    public Transform[] spawnPointsPlatformero;
    public Transform[] spawnPointsMano;

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
            Debug.LogWarning("[PlataformeoManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        int contadorJugadores = 0;

        foreach (PersistentPlayer mandoFantasma in jugadoresConectados)
        {
            PlayerInputHandler lectorBotones = mandoFantasma.GetComponent<PlayerInputHandler>();

            if (contadorJugadores % 2 == 0)
                SpawnearPlatformero(contadorJugadores, lectorBotones);
            else
                SpawnearMano(contadorJugadores, lectorBotones);

            contadorJugadores++;
        }
    }

    // Llamado por SpritePlatformero cuando toca la meta(singleton)
    public void DeclararVictoria(GameObject ganador)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        Debug.Log($"[PlataformeoManager] ¡VICTORIA! Ganador: {ganador.name}");

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
            Debug.LogError("[PlataformeoManager] ERROR: No hay escenas configuradas en 'Escenas Aleatorias'.");
            return;
        }

        int indiceAleatorio = Random.Range(0, escenasAleatorias.Length);
        string escenaElegida = escenasAleatorias[indiceAleatorio];

        if (string.IsNullOrEmpty(escenaElegida))
        {
            Debug.LogError($"[PlataformeoManager] ERROR: El hueco {indiceAleatorio} del array de escenas está vacío.");
            return;
        }

        Debug.Log($"[PlataformeoManager] Cargando escena: {escenaElegida}");
        SceneManager.LoadScene(escenaElegida);
    }

    private void SpawnearPlatformero(int idJugador, PlayerInputHandler mando)
    {
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsPlatformero[indexSpawn];

        GameObject sprite = Instantiate(prefabPlatformero, puntoSpawn.position, Quaternion.identity);

        if (sprite.TryGetComponent(out SpritePlatformero scriptPlatformero))
            scriptPlatformero.ConectarMando(mando);

        if (sprite.TryGetComponent(out Animator animator))
        {
            if (animatorsPlatformero.Length > indexSpawn && animatorsPlatformero[indexSpawn] != null)
                animator.runtimeAnimatorController = animatorsPlatformero[indexSpawn];
        }
    }

    private void SpawnearMano(int idJugador, PlayerInputHandler mando)
    {
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsMano[indexSpawn];

        GameObject avatar = Instantiate(prefabMano, puntoSpawn.position, Quaternion.identity);

        if (avatar.TryGetComponent(out SpriteManoBob scriptMano))
            scriptMano.ConectarMando(mando);

        if (avatar.TryGetComponent(out Animator animator))
        {
            if (animatorsMano.Length > indexSpawn && animatorsMano[indexSpawn] != null)
                animator.runtimeAnimatorController = animatorsMano[indexSpawn];
        }
    }
}