using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class PongGameManager : MonoBehaviour
{
    public static PongGameManager Instance; // Singleton para llamarlo desde cualquier sitio

    [Header("Prefab Padre (invisible, lleva movimiento)")]
    public GameObject prefabPadre;

    [Header("Prefab Hijo (el círculo visible)")]
    public GameObject prefabCirculo;
    public Vector2 offsetHijo;

    [Header("Spawn Points")]
    public Transform[] spawnPointsPlayers;
    
    private int i = 0;

    public List<GameObject> jugadoresVivos = new List<GameObject>();
    private GameObject jugadorInstanciado;
    private bool partidaTerminada = false;
    public String[] escenasAleatorias;

    private GameObject jugadorUno;
    private GameObject jugadorDos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);
        
        if (jugadoresConectados.Length == 0)
        {
            Debug.LogWarning("[PongGameManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        int contadorJugadores = 0;

        foreach (PersistentPlayer mandoFantasma in jugadoresConectados)
        {
            
            Debug.Log("Procesando jugador: " + mandoFantasma.name);
            PlayerInputHandler lectorBotones = mandoFantasma.GetComponent<PlayerInputHandler>();
            if (contadorJugadores % 2 == 0)
            {
                
                SpawnearJugadorUno(contadorJugadores, lectorBotones, i);
                Debug.Log($"Spawnereando jugador 1 {contadorJugadores} con mando {lectorBotones.name} en spawn point {i}");
                jugadorUno.transform.SetParent(spawnPointsPlayers[i].transform);
                
            }
            else
            {
                SpawnearJugadorDos(contadorJugadores, lectorBotones, i);
                Debug.Log($"Spawnereando jugador 1 {contadorJugadores} con mando {lectorBotones.name} en spawn point {i}");
                jugadorDos.transform.SetParent(spawnPointsPlayers[i].transform);
                //jugadorDos.transform.SetParent(jugadorUno.transform);
            }
            i = i + 1;
            contadorJugadores++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ComprobarGanador();
    }
    public void ComprobarGanador()
    {
        if (partidaTerminada) return;

        jugadoresVivos.RemoveAll(p => p == null);

        if (jugadoresVivos.Count == 1)
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

    private void SpawnearJugadorUno(int idJugador, PlayerInputHandler mando, int i)
    {
        Debug.Log("Spawneando jugador padre");
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsPlayers[indexSpawn];
        
        jugadorUno = Instantiate(prefabPadre, puntoSpawn.position, spawnPointsPlayers[i].transform.rotation);
        jugadoresVivos.Add(jugadorUno);

        if (jugadorUno.TryGetComponent<PongMovement>(out var scriptMovimiento))
        {
            scriptMovimiento.ConectarMando(mando);
        }
    } 
    
    private void SpawnearJugadorDos(int idJugador, PlayerInputHandler mando, int i )
    {
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsPlayers[indexSpawn];
        
      jugadorDos = Instantiate(prefabCirculo, puntoSpawn.position + (Vector3)offsetHijo, spawnPointsPlayers[i].transform.rotation );
        //hijo.transform.SetParent(jugadorInstanciado.transform);

        if (jugadorDos.TryGetComponent<PongMovement>(out var scriptMovimiento))
        {
            scriptMovimiento.ConectarMando(mando);
        }
    }
}
