using EasyTransition;
using System.Collections;
using UnityEngine;

public class PlataformeoManager : MonoBehaviour
{
    public static PlataformeoManager Instance { get; private set; }

    [Header("Prefabs Sprites")]
    public GameObject prefabPlatformero;
    public GameObject prefabMano;

    [Header("Prefabs Bloques por Equipo")]
    public GameObject[] prefabSueloPorEquipo;
    public GameObject[] prefabParedPorEquipo;

    [Header("Animators Platformero")]
    public RuntimeAnimatorController[] animatorsPlatformero;

    [Header("Animators Mano")]
    public RuntimeAnimatorController[] animatorsMano;

    [Header("Preview Sprites Mano")]
    public Sprite[] previewSpritesMano;

    [Header("Spawn Points")]
    public Transform[] spawnPointsPlatformero;
    public Transform[] spawnPointsMano;

    [Header("Condicion de Victoria")]
    [SerializeField] private float tiempoEsperaVictoria = 2f;
    [SerializeField] private string[] escenasAleatorias;

    [SerializeField] private TransitionSettings transition;
    [SerializeField] private float startDelay;

    private bool juegoTerminado = false;
    PuntuacionManager puntuacionManager;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
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

        int contadorPlatformeros = 0;
        int contadorManos = 0;

        foreach (PersistentPlayer pp in jugadoresConectados)
        {
            PlayerInputHandler mando = pp.GetComponent<PlayerInputHandler>();
            int slotId = pp.playerIndex; // 1-6 asignado en lobby

            // Slots impares (1,3,5) → Platformero | Slots pares (2,4,6) → Mano
            bool esPlatformero = slotId % 2 != 0;

            if (esPlatformero)
                SpawnearPlatformero(contadorPlatformeros++, mando);
            else
                SpawnearMano(contadorManos++, mando);
        }
        puntuacionManager = FindObjectOfType<PuntuacionManager>();
    }

    public void DeclararVictoria(GameObject ganador)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        int indexxxx = ganador.GetComponent<SpritePlatformero>().miMando.teamIndex;
        puntuacionManager.SumarPuntuacion(indexxxx);
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
            Debug.LogError("[PlataformeoManager] No hay escenas configuradas.");
            return;
        }
        string escena = escenasAleatorias[Random.Range(0, escenasAleatorias.Length)];

        if (string.IsNullOrEmpty(escena))
        {
            Debug.LogError("[PlataformeoManager] Escena vacía.");
            return;
        }
        TransitionManager.Instance().Transition(escena, transition, startDelay);
    }

    private void SpawnearPlatformero(int indice, PlayerInputHandler mando)
    {
        if (indice >= spawnPointsPlatformero.Length)
        {
            Debug.LogError($"[PlataformeoManager] Falta spawn platformero {indice}");
            return;
        }
        GameObject sprite = Instantiate(prefabPlatformero, spawnPointsPlatformero[indice].position, Quaternion.identity);

        if (sprite.TryGetComponent(out SpritePlatformero s)) s.ConectarMando(mando);

        if (sprite.TryGetComponent(out Animator anim) && animatorsPlatformero.Length > indice && animatorsPlatformero[indice] != null)
            anim.runtimeAnimatorController = animatorsPlatformero[indice];
    }

    private void SpawnearMano(int indice, PlayerInputHandler mando)
    {
        if (indice >= spawnPointsMano.Length)
        {
            Debug.LogError($"[PlataformeoManager] Falta spawn mano {indice}");
            return;
        }

        GameObject avatar = Instantiate(prefabMano, spawnPointsMano[indice].position, Quaternion.identity);

        if (avatar.TryGetComponent(out SpriteManoBob s))
        {
            s.ConectarMando(mando);

            // Asignar sprite de preview según el ÍNDICE de la mano
            if (s.spritePreview != null && previewSpritesMano.Length > indice && previewSpritesMano[indice] != null)
                s.spritePreview.sprite = previewSpritesMano[indice];

            // Asignar prefabs de bloques según el ÍNDICE de la mano
            if (prefabSueloPorEquipo.Length > indice && prefabSueloPorEquipo[indice] != null)
                s.prefabSuelo = prefabSueloPorEquipo[indice];

            if (prefabParedPorEquipo.Length > indice && prefabParedPorEquipo[indice] != null)
                s.prefabPared = prefabParedPorEquipo[indice];

            // Inicializar preview DESPUÉS de asignar si no no va
            s.InicializarPreview();
        }

        if (avatar.TryGetComponent(out Animator anim) && animatorsMano.Length > indice && animatorsMano[indice] != null)
            anim.runtimeAnimatorController = animatorsMano[indice];
    }
}