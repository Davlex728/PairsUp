using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class MazeGameManager : MonoBehaviour
{
    public static MazeGameManager instance;

    [Header("Prefabs")] 
    public GameObject prefabPlayerUno;

    public GameObject prefabPlayerDos;

    [Header("Spawners")]
    private int i = 0;

    public Transform[] spawnPointsPlayers;

    public List<List<GameObject>> spawnPointsList;

    private bool partidaTerminada = false;
    
    [Header("Random Scenes")]
    public String[] escenasAleatorias;
    
    [Header("Animators para cada jugador (en orden de slot)")]
    public RuntimeAnimatorController[] animators;

    [Header("Players")] 
    public List<GameObject> jugadoresVivos = new List<GameObject>();
    private GameObject jugadorUno;
    private GameObject jugadorDos;

    List<List<GameObject>> parejas = new List<List<GameObject>>();
    public Transform puntoSpawn;
    int pare;


    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);

        Debug.Log($"[MazeGameManager] Jugadores conectados: {jugadoresConectados.Length}");

        if (jugadoresConectados.Length == 0)
        {
            Debug.LogWarning("[MazeGameManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        int contadorJugadores = 0;
        Array.Sort(jugadoresConectados, (a, b) => a.playerIndex.CompareTo(b.playerIndex));
        foreach (PersistentPlayer jugador in jugadoresConectados)
        {
            Debug.Log($"Jugador: {jugador.name} playerIndex: {jugador.playerIndex} teamIndex: {jugador.teamIndex}");

            Debug.Log(contadorJugadores);
            Debug.Log($"Procesando Jugador : {jugador.name}");
            PlayerInputHandler lectorBotones = jugador.GetComponent<PlayerInputHandler>();

            int slotId = jugador.playerIndex;
            int equipo = jugador.teamIndex;

            bool esJugadorUno = (slotId % 2 != 0);
            if (esJugadorUno)
            {
                SpawnearJugadorUno(slotId, equipo, lectorBotones);
                Debug.Log(
                    $"Spawnereando jugador 1 {contadorJugadores} con mando {lectorBotones.name} en spawn point {i}");
            }
            else
            {
                SpawnearJugadorDos(slotId, equipo, lectorBotones);
                Debug.Log(
                    $"Spawnereando jugador 2 {contadorJugadores} con mando {lectorBotones.name} en spawn point {i}");
            }

            i = i + 1;
            contadorJugadores++;
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < parejas.Count; i++)
        {
            var pareja = parejas[i];
            
            //Debug.Log($"Pareja[0]: {pareja[0]} Pareja[1]: {pareja[1]}");

            if (pareja[0].TryGetComponent<MazeMovement>(out var jugA) &&
                pareja[1].TryGetComponent<MazeMovement>(out var jugB))
            {
                InputBoton botonA = ObtenerBotonPulsado(jugA.mandoMovimiento);
                InputBoton botonB = ObtenerBotonPulsado(jugB.mandoMovimiento);
                Debug.Log($"BotonA: {botonA} BotonB: {botonB}");


                // ¿Han pulsado el mismo botón los dos?
                if (botonA != InputBoton.Ninguno && botonA == botonB)
                {
                    // Manda el botón al script de movimiento
                    jugA.RecibirInput(botonA);
                    jugB.RecibirInput(botonB);
                    
                    
                    Debug.Log("Hola que pasa, pulsando los botones");
                    // Manda al script de símbolos para comprobar si es correcto
                    ButtonLogic.instance.ComprobarInput(botonA, jugA.gameObject.tag,pareja);
                }
                else
                {
                    jugA.RecibirInput(InputBoton.Ninguno);
                    jugB.RecibirInput(InputBoton.Ninguno);
                }
            }
        }
    }

    private InputBoton ObtenerBotonPulsado(PlayerInputHandler mando)
    {
        if (mando == null) return InputBoton.Ninguno;

        if (mando.isSouthZone) return InputBoton.South;
        if (mando.isNorthZone) return InputBoton.North;
        if (mando.isWestZone) return InputBoton.West;
        if (mando.isEastZone) return InputBoton.East;
        if (mando == null)
        {
            Debug.LogError("El mando es null");
            return InputBoton.Ninguno;
        }

        return InputBoton.Ninguno;
    }

    public void ComprobarGanador()
    {
        if (partidaTerminada) return;

        jugadoresVivos.RemoveAll(p => p == null);

        if (AzulMeta.instance.jugadoresAzules == 2 || RojoMeta.instance.jugadoresRojos == 2 ||
            AmarilloMeta.instance.jugadoresAmarillos == 2)
        {
            partidaTerminada = true;
            // Comprobación de seguridad
            if (escenasAleatorias == null || escenasAleatorias.Length == 0)
            {
                Debug.LogError("[SpriteCesta] ERROR: No hay escenas configuradas en el array 'Escenas Aleatorias'.");
                return;
            }

            // Elegir y cargar la escena aleatoria
            int indiceAleatorio = UnityEngine.Random.Range(0, escenasAleatorias.Length);
            string escenaElegida = escenasAleatorias[indiceAleatorio];

            if (string.IsNullOrEmpty(escenaElegida))
            {
                Debug.LogError($"[SpriteCesta] ERROR: El hueco {indiceAleatorio} del array de escenas está vacío.");
                return;
            }

            Debug.Log($"[SpriteCesta] Saltando a la nueva partida: {escenaElegida}");
            SceneManager.LoadScene(escenaElegida);
            Debug.Log($"[LaserTagManager] ¡Ha ganado la pareja: {jugadoresVivos[0].name}!");
        }
        else if (jugadoresVivos.Count == 0)
        {
            partidaTerminada = true;
            Debug.Log("[LaserTagManager] ¡Empate! No quedan parejas.");
        }
    }

    private void SpawnearJugadorUno(int slotId, int equipo, PlayerInputHandler mando)
    {
        Debug.Log("Spawneando jugador padre");

        jugadorUno = Instantiate(prefabPlayerUno, spawnPointsPlayers[slotId - 1].position, Quaternion.identity);

        jugadoresVivos.Add(jugadorUno);
        if (slotId == 1)
        {
            jugadorUno.gameObject.tag = "Azul";
        }

        if (slotId == 3)
        {
            jugadorUno.gameObject.tag = "Rojo";
        }

        if (slotId == 5)
        {
            jugadorUno.gameObject.tag = "Amarillo";
        }

        if (jugadorUno.TryGetComponent<MazeMovement>(out var scriptMovimiento))
        {
            scriptMovimiento.ConectarMando(mando);

            scriptMovimiento.spawnPoint = spawnPointsPlayers[slotId - 1];
        }
        AsignarAnimator(jugadorUno, slotId);
    }

    private void SpawnearJugadorDos(int slotId, int equipo, PlayerInputHandler mando)
    {
        Debug.Log($"jugadorUno al entrar en SpawnearJugadorDos:{jugadorUno}");
        List<GameObject> nuevaPareja = new List<GameObject>();
        //int indexSpawn = idJugador / 2;
        Debug.Log(slotId);
        jugadorDos = Instantiate(prefabPlayerDos, spawnPointsPlayers[slotId - 1].position, Quaternion.identity);

        jugadoresVivos.Add(jugadorDos);
        nuevaPareja.Add(jugadorUno);
        nuevaPareja.Add(jugadorDos);

        parejas.Add(nuevaPareja);

        if (slotId == 2)
        {
            jugadorDos.gameObject.tag = "Azul";
            
        }

        if (slotId == 4)
        {
            jugadorDos.gameObject.tag = "Rojo";
        }

        if (slotId == 6)
        {
            jugadorDos.gameObject.tag = "Amarillo";
        }
        //parejas.Add(nuevaPareja);
        //Debug.Log(i);

        if (jugadorDos.TryGetComponent<MazeMovement>(out var scriptMovimiento))
        {
            scriptMovimiento.ConectarMando(mando);
            scriptMovimiento.spawnPoint = spawnPointsPlayers[slotId - 1];
        }
        AsignarAnimator(jugadorDos, slotId);
    }
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
}