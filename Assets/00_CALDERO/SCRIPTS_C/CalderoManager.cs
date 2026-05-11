using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalderoManager : MonoBehaviour
{

    public static CalderoManager Instance { get; private set; }

    [Header("Prefabs Sprites")]
    public GameObject prefabCesta;
    public GameObject prefabMano;

    [Header("Animators por Slot (índice 0 = slot 1, índice 5 = slot 6)")]
    public RuntimeAnimatorController[] animators;

    [Header("Spawn Points")]
    public Transform[] spawnPointsCestas;
    public Transform[] spawnPointsManos;

    [Header("UI")]
    public UIReceta[] panelesUIParejas;
    public TextMeshProUGUI textoTimer;

    [Header("Duración partida")]
    public float duracionPartida = 60f;

    [Header("Ingrediente Objetivo")]
    public float intervaloNuevoObjetivo = 8f;
    [HideInInspector] public float tiempoSiguienteObjetivo;

    [Header("Condición de Victoria")]
    [SerializeField] private float tiempoEsperaVictoria = 2f;
    [SerializeField] private string[] escenasAleatorias;

    private bool juegoTerminado = false;
    private float tiempoRestante;
    [SerializeField] private Image relojTimer;
    private List<SpriteCesta> cestas = new(); //lista para meter las cesta y al acabar el minijuego mirar puntuacion

    PuntuacionManager puntuacionManager;
    private int equipoGanador;

    public TipoIngrediente IngredienteObjetivo { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        tiempoRestante = duracionPartida;
        ElegirNuevoObjetivo();
        tiempoSiguienteObjetivo = intervaloNuevoObjetivo;

        // Configurar la imagen del reloj una sola vez para que mantenga tamaño y propiedades constantes
        if (relojTimer != null)
        {
            relojTimer.type = Image.Type.Filled;
            relojTimer.fillMethod = Image.FillMethod.Radial360;
            relojTimer.fillClockwise = false;
            relojTimer.preserveAspect = true;
        }

        PersistentPlayer[] jugadores = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);
        if (jugadores.Length == 0) { Debug.LogWarning("[CalderoManager] No hay jugadores."); return; }

        foreach (PersistentPlayer pp in jugadores)
        {
            PlayerInputHandler mando = pp.GetComponent<PlayerInputHandler>();
            int slotId = pp.playerIndex;
            int equipo = pp.teamIndex;
            if (slotId % 2 != 0) SpawnearCesta(slotId, equipo, mando);
            else SpawnearMano(slotId, equipo, mando);
        }

        puntuacionManager = FindObjectOfType<PuntuacionManager>();
    }

    private void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;
        if (textoTimer != null) textoTimer.text = Mathf.CeilToInt(Mathf.Max(tiempoRestante, 0f)).ToString();

        if (relojTimer != null)
        {
            // Solo actualizar el fillAmount en runtime; la configuración está en Start()
            float fillAmount = duracionPartida > 0f ? Mathf.Clamp01(tiempoRestante / duracionPartida) : 0f;
            relojTimer.fillAmount = fillAmount;
            if (tiempoRestante <= 5f) relojTimer.color = Color.red * new Color(1f, 1f, 1f, 0.5f);  // Cambia a rojo en los últimos 5 segundos
            else if (tiempoRestante <= duracionPartida/3) relojTimer.color = Color.yellow *new Color(1f, 1f, 1f, 0.5f); // Cambia a amarillo cuando queda un tercio del tiempo
            else relojTimer.color = Color.green * new Color(1f, 1f, 1f, 0.5f); // Verde el resto del tiempo
        }

        tiempoSiguienteObjetivo -= Time.deltaTime;
        if (tiempoSiguienteObjetivo <= 0f)
        {
            ElegirNuevoObjetivo();
            tiempoSiguienteObjetivo = intervaloNuevoObjetivo;
        }

        if (tiempoRestante <= 0f) TerminarPorTimer();
    }

    public void ElegirNuevoObjetivo()
    {
        var valores = System.Enum.GetValues(typeof(TipoIngrediente));
        IngredienteObjetivo = (TipoIngrediente)valores.GetValue(Random.Range(0, valores.Length));
        Debug.Log($"[CalderoManager] Nuevo objetivo: {IngredienteObjetivo}");

        foreach (UIReceta ui in panelesUIParejas)
            if (ui != null) ui.MostrarIngredienteObjetivo(IngredienteObjetivo);
    }

    private void TerminarPorTimer()
    {
        juegoTerminado = true;
        SpriteCesta ganadora = null;
        int maxPuntos = int.MinValue;
        foreach (SpriteCesta c in cestas) //mete las cesras en la lista de la puntuaCION
            if (c.puntos > maxPuntos) { maxPuntos = c.puntos; ganadora = c; }

        Debug.Log($"[CalderoManager] Tiempo acabado. Ganador: {(ganadora != null ? ganadora.gameObject.name : "empate")} con {maxPuntos} puntos.");
        StartCoroutine(EsperarYCargarEscena());
    }

    public void DeclararVictoria(GameObject ganador)
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        StartCoroutine(EsperarYCargarEscena());
    }

    void SacarCampeon()
    {
        SpriteCesta[] cestas = FindObjectsOfType<SpriteCesta>();
        int[] puntosYindex = new int[2];
        puntosYindex[0] = 0;
        puntosYindex[1] = 0;
        for (int i = 0; i < cestas.Length; i++)
            {
            SpriteCesta c = cestas[i];
            if (c.puntos > puntosYindex[0]) // umbral de victoria, ajustar según necesidad
            {
                puntosYindex[0] = c.puntos;
                puntosYindex[1] = c.miMando.teamIndex;
                return;
            }
        }
        equipoGanador = puntosYindex[1];

    }
    private IEnumerator EsperarYCargarEscena()
    {
        yield return new WaitForSeconds(tiempoEsperaVictoria);
        if(puntuacionManager != null)
        {
            SacarCampeon();
            puntuacionManager.SumarPuntuacion(equipoGanador);
        }
        Debug.Log(equipoGanador);
        Debug.Log(puntuacionManager.ObtenerPuntuacion(equipoGanador));
        CargarEscenaAleatoria();
    }

    private void CargarEscenaAleatoria()
    {
        if (escenasAleatorias == null || escenasAleatorias.Length == 0) { Debug.LogError("[CalderoManager] No hay escenas."); return; }
        string escena = escenasAleatorias[Random.Range(0, escenasAleatorias.Length)];
        if (string.IsNullOrEmpty(escena)) { Debug.LogError("[CalderoManager] Escena vacía."); return; }
        SceneManager.LoadScene(escena);
    }

    private void SpawnearCesta(int slotId, int equipo, PlayerInputHandler mando)
    {
        if (equipo >= spawnPointsCestas.Length) { Debug.LogError($"[CalderoManager] Falta spawn cesta {equipo}"); return; }
        GameObject sprite = Instantiate(prefabCesta, spawnPointsCestas[equipo].position, Quaternion.identity);
        if (sprite.TryGetComponent(out SpriteCesta sc))
        {
            sc.ConectarMando(mando);
            cestas.Add(sc);
            if (panelesUIParejas.Length > equipo && panelesUIParejas[equipo] != null)
            {
                sc.miUI = panelesUIParejas[equipo];
                sc.IniciarUI();
            }
        }
        AsignarAnimator(sprite, slotId);
    }

    private void SpawnearMano(int slotId, int equipo, PlayerInputHandler mando)
    {
        if (equipo >= spawnPointsManos.Length) { Debug.LogError($"[CalderoManager] Falta spawn mano {equipo}"); return; }
        GameObject avatar = Instantiate(prefabMano, spawnPointsManos[equipo].position, Quaternion.identity);
        if (avatar.TryGetComponent(out SpriteMano sm)) sm.ConectarMando(mando);
        AsignarAnimator(avatar, slotId);
    }

    private void AsignarAnimator(GameObject obj, int slotId)
    {
        int idx = slotId - 1;
        if (obj.TryGetComponent(out Animator animator) && animators.Length > idx && animators[idx] != null)
            animator.runtimeAnimatorController = animators[idx];
    }
}