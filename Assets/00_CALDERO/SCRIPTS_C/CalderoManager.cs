using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalderoManager : MonoBehaviour
{
    public static CalderoManager Instance { get; private set; }

    [Header("Prefabs Sprites")]
    public GameObject prefabCesta;
    public GameObject prefabMano;

    [Header("Animators por Slot (índice 0 = slot 1, índice 5 = slot 6)")]
    public RuntimeAnimatorController[] animators; // array de 6, uno por slot

    [Header("Spawn Points")]
    public Transform[] spawnPointsCestas; // 3 puntos, uno por equipo
    public Transform[] spawnPointsManos;  // 3 puntos, uno por equipo

    [Header("UI (Textos de la pantalla)")]
    public UIReceta[] panelesUIParejas; // 3 paneles, uno por equipo

    [Header("Condición de Victoria")]
    [SerializeField] private float tiempoEsperaVictoria = 2f;
    [SerializeField] private string[] escenasAleatorias;

    private bool juegoTerminado = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        PersistentPlayer[] jugadores = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);

        if (jugadores.Length == 0)
        {
            Debug.LogWarning("[CalderoManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        foreach (PersistentPlayer pp in jugadores)
        {
            PlayerInputHandler mando = pp.GetComponent<PlayerInputHandler>();
            int slotId = pp.playerIndex;  // 1-6, asignado en la lobby
            int equipo = pp.teamIndex;    // 0-2, (slotId-1)/2

            // Dentro de cada equipo: slot impar → Cesta, slot par → Mano
            // Slot 1,3,5 (impares) → Cesta  |  Slot 2,4,6 (pares) → Mano
            bool esCesta = (slotId % 2 != 0);

            if (esCesta)
                SpawnearCesta(slotId, equipo, mando);
            else
                SpawnearMano(slotId, equipo, mando);
        }
    }

    

    private void SpawnearCesta(int slotId, int equipo, PlayerInputHandler mando)
    {
        if (equipo >= spawnPointsCestas.Length)
        {
            Debug.LogError($"[CalderoManager] No hay spawnPoint de cesta para equipo {equipo}");
            return;
        }

        GameObject sprite = Instantiate(prefabCesta, spawnPointsCestas[equipo].position, Quaternion.identity);

        if (sprite.TryGetComponent(out SpriteCesta scriptCesta))
        {
            scriptCesta.ConectarMando(mando);

            if (panelesUIParejas.Length > equipo && panelesUIParejas[equipo] != null)
            {
                scriptCesta.miUI = panelesUIParejas[equipo];
                scriptCesta.IniciarUI();
            }
        }

        AsignarAnimator(sprite, slotId);
        Debug.Log($"[CalderoManager] Cesta spawneada → Slot {slotId} | Equipo {equipo}");
    }

    private void SpawnearMano(int slotId, int equipo, PlayerInputHandler mando)
    {
        if (equipo >= spawnPointsManos.Length)
        {
            Debug.LogError($"[CalderoManager] No hay spawnPoint de mano para equipo {equipo}");
            return;
        }

        GameObject avatar = Instantiate(prefabMano, spawnPointsManos[equipo].position, Quaternion.identity);

        if (avatar.TryGetComponent(out SpriteMano scriptMano))
            scriptMano.ConectarMando(mando);

        AsignarAnimator(avatar, slotId);
        Debug.Log($"[CalderoManager] Mano spawneada → Slot {slotId} | Equipo {equipo}");
    }

    /// <summary>Asigna el animator según slotId (1-6 → índice 0-5 del array).</summary>
    private void AsignarAnimator(GameObject obj, int slotId)
    {
        int animIndex = slotId - 1; // slot 1 → índice 0
        if (obj.TryGetComponent(out Animator animator))
        {
            if (animators.Length > animIndex && animators[animIndex] != null)
                animator.runtimeAnimatorController = animators[animIndex];
            else
                Debug.LogWarning($"[CalderoManager] No hay animator para slot {slotId} (índice {animIndex})");
        }
    }

    // ── Victoria ────────────────────────────────────────────────────────────

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
            Debug.LogError("[CalderoManager] ERROR: No hay escenas configuradas.");
            return;
        }

        int idx = Random.Range(0, escenasAleatorias.Length);
        string escena = escenasAleatorias[idx];

        if (string.IsNullOrEmpty(escena))
        {
            Debug.LogError($"[CalderoManager] ERROR: Hueco {idx} del array vacío.");
            return;
        }

        Debug.Log($"[CalderoManager] Cargando escena: {escena}");
        SceneManager.LoadScene(escena);
    }
}