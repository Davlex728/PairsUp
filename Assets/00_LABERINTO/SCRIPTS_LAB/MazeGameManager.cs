using EasyTransition;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGameManager : MonoBehaviour
{
    public static MazeGameManager instance;

    [Header("Prefabs")]
    public GameObject prefabPlayerUno;
    public GameObject prefabPlayerDos;

    [Header("Spawners")]
    private int i = 0;
    public Transform[] spawnPointsPlayers;

    [Header("Secuencias por pareja")]

    public ButtonLogic[] buttonLogicsPorPareja;

    [Header("Random Scenes")]
    public String[] escenasAleatorias;

    [Header("Animators para cada jugador (en orden de slot)")]
    public RuntimeAnimatorController[] animators;

    [Header("Players")]
    public List<GameObject> jugadoresVivos = new List<GameObject>();
    private GameObject jugadorUno;
    private GameObject jugadorDos;

    List<List<GameObject>> parejas = new List<List<GameObject>>();
    int parejaIndex = 0;

    private bool partidaTerminada = false;
    private bool minijuegoResuelto = false;
    [SerializeField] private TransitionSettings transition;
    [SerializeField] private float startDelay;

    PuntuacionManager puntuacionManager;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);
        Debug.Log($"[MazeGameManager] Jugadores conectados: {jugadoresConectados.Length}");

        if (jugadoresConectados.Length == 0)
        {
            Debug.LogWarning("[MazeGameManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        Array.Sort(jugadoresConectados, (a, b) => a.playerIndex.CompareTo(b.playerIndex));

        int contadorJugadores = 0;
        foreach (PersistentPlayer jugador in jugadoresConectados)
        {
            PlayerInputHandler lectorBotones = jugador.GetComponent<PlayerInputHandler>();
            int slotId = jugador.playerIndex;
            int equipo = jugador.teamIndex;

            bool esJugadorUno = (slotId % 2 != 0);
            if (esJugadorUno)
                SpawnearJugadorUno(slotId, equipo, lectorBotones);
            else
                SpawnearJugadorDos(slotId, equipo, lectorBotones);

            i++;
            contadorJugadores++;
        }

        for (int p = 0; p < buttonLogicsPorPareja.Length; p++)
        {
            if (buttonLogicsPorPareja[p] == null) continue;

            int idx = p;
            buttonLogicsPorPareja[p].pareja = parejas.Count > idx ? parejas[idx] : null;
            buttonLogicsPorPareja[p].OnSecuenciaCompletada += OnParejaCompletaSecuencia;
        }

        puntuacionManager = FindObjectOfType<PuntuacionManager>();
    }

    private void Update()
    {
        ComprobarGanador();
    }

    void LateUpdate()
    {
        for (int p = 0; p < parejas.Count; p++)
        {
            var pareja = parejas[p];

            if (pareja[0].TryGetComponent<MazeMovement>(out var jugA) &&
                pareja[1].TryGetComponent<MazeMovement>(out var jugB))
            {
                InputBoton botonA = ObtenerBotonPulsado(jugA.mandoMovimiento);
                InputBoton botonB = ObtenerBotonPulsado(jugB.mandoMovimiento);


                if (botonA != InputBoton.Ninguno && botonA == botonB)
                {

                    if (p < buttonLogicsPorPareja.Length && buttonLogicsPorPareja[p] != null)
                    {
                        buttonLogicsPorPareja[p].ComprobarInput(botonA);
                    }

                }
            }
        }
    }


    private void OnParejaCompletaSecuencia(ButtonLogic ganadora)
    {
        if (minijuegoResuelto) return;
        minijuegoResuelto = true;


        foreach (ButtonLogic bl in buttonLogicsPorPareja)
        {
            if (bl != null && bl != ganadora)
                bl.Bloquear();
        }
    }

    private InputBoton ObtenerBotonPulsado(PlayerInputHandler mando)
    {
        if (mando == null) return InputBoton.Ninguno;
        if (mando.isSouthZone) return InputBoton.South;
        if (mando.isNorthZone) return InputBoton.North;
        if (mando.isWestZone) return InputBoton.West;
        if (mando.isEastZone) return InputBoton.East;
        return InputBoton.Ninguno;
    }

    public void ComprobarGanador()
    {
        if (partidaTerminada) return;

        jugadoresVivos.RemoveAll(p => p == null);

        if (AzulMeta.instance.jugadoresAzules == 2 || RojoMeta.instance.jugadoresRojos == 2 || AmarilloMeta.instance.jugadoresAmarillos == 2)
        {
            if (AzulMeta.instance.jugadoresAzules == 2)
            { 
                puntuacionManager.SumarPuntuacion(0);
            Debug.Log($"[MazeGameManager] Jugadores azules en meta: {AzulMeta.instance.jugadoresAzules}");
        }
        else if (RojoMeta.instance.jugadoresRojos == 2)
            puntuacionManager.SumarPuntuacion(1);
        else if (AmarilloMeta.instance.jugadoresAmarillos == 2)
            puntuacionManager.SumarPuntuacion(2);

            partidaTerminada = true;

            if (escenasAleatorias == null || escenasAleatorias.Length == 0)
            {
                Debug.LogError("[MazeGameManager] No hay escenas configuradas.");
                return;
            }

            int indiceAleatorio = UnityEngine.Random.Range(0, escenasAleatorias.Length);
            string escenaElegida = escenasAleatorias[indiceAleatorio];

            if (string.IsNullOrEmpty(escenaElegida))
            {
                Debug.LogError($"[MazeGameManager] El hueco {indiceAleatorio} del array de escenas está vacío.");
                return;
            }

            TransitionManager.Instance().Transition(escenaElegida, transition, startDelay);
        }
        else if (jugadoresVivos.Count == 0)
        {
            partidaTerminada = true;
            Debug.Log("[MazeGameManager] ¡Empate!");
        }
    }

    private void SpawnearJugadorUno(int slotId, int equipo, PlayerInputHandler mando)
    {
        jugadorUno = Instantiate(prefabPlayerUno, spawnPointsPlayers[slotId - 1].position, Quaternion.identity);
        jugadoresVivos.Add(jugadorUno);

        if (slotId == 1) jugadorUno.tag = "Azul";
        if (slotId == 3) jugadorUno.tag = "Rojo";
        if (slotId == 5) jugadorUno.tag = "Amarillo";

        if (jugadorUno.TryGetComponent<MazeMovement>(out var mov))
        {
            mov.ConectarMando(mando);
            mov.spawnPoint = spawnPointsPlayers[slotId - 1];
        }
        AsignarAnimator(jugadorUno, slotId);
    }

    private void SpawnearJugadorDos(int slotId, int equipo, PlayerInputHandler mando)
    {
        jugadorDos = Instantiate(prefabPlayerDos, spawnPointsPlayers[slotId - 1].position, Quaternion.identity);
        jugadoresVivos.Add(jugadorDos);

        List<GameObject> nuevaPareja = new List<GameObject> { jugadorUno, jugadorDos };
        parejas.Add(nuevaPareja);
        parejaIndex++;

        if (slotId == 2) jugadorDos.tag = "Azul";
        if (slotId == 4) jugadorDos.tag = "Rojo";
        if (slotId == 6) jugadorDos.tag = "Amarillo";

        if (jugadorDos.TryGetComponent<MazeMovement>(out var mov))
        {
            mov.ConectarMando(mando);
            mov.spawnPoint = spawnPointsPlayers[slotId - 1];
        }
        AsignarAnimator(jugadorDos, slotId);
    }

    private void AsignarAnimator(GameObject obj, int slotId)
    {
        int animIndex = slotId - 1;
        if (obj.TryGetComponent(out Animator animator))
        {
            if (animators.Length > animIndex && animators[animIndex] != null)
                animator.runtimeAnimatorController = animators[animIndex];
            else
                Debug.LogWarning($"[MazeGameManager] No hay animator para slot {slotId}");
        }
    }
}