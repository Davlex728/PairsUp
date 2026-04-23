using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class MazeGameManager : MonoBehaviour
{
    [Header("Prefabs")] 
    
    public GameObject prefabPlayerUno;

    public GameObject prefabPlayerDos;

    [Header("Spawners")] 
    
    private int i = 0;
    
    public Transform[]  spawnPointsPlayers;
    
    public List<List<GameObject>> spawnPointsList;
    
    private bool partidaTerminada = false;
    [Header("Random Scenes")]
    
    public String[] escenasAleatorias;
    
    
    [Header("Players")]
    
    public List<GameObject> jugadoresVivos = new List<GameObject>();
    private GameObject jugadorUno;
    private GameObject jugadorDos;
    
    List<List<GameObject>> parejas = new List<List<GameObject>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);

        if (parejas.Count == 0)
        {
            Debug.LogWarning("[MazeGameManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        int contadorJugadores = 0;

        foreach (PersistentPlayer jugador in jugadoresConectados)
        {
            Debug.Log($"Procesando Jugador : {jugador.name}");
            PlayerInputHandler lectorBotones = jugador.GetComponent<PlayerInputHandler>();
            if (contadorJugadores % 2 == 0)
            {
                SpawnearJugadorUno(contadorJugadores,lectorBotones,i);
                Debug.Log($"Spawnereando jugador 1 {contadorJugadores} con mando {lectorBotones.name} en spawn point {i}");
            }
            else
            {
                SpawnearJugadorDos(contadorJugadores,lectorBotones,i);
                Debug.Log($"Spawnereando jugador 2 {contadorJugadores} con mando {lectorBotones.name} en spawn point {i}");
                
            }

            i = i + 1;
            contadorJugadores++;
        }
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
        
        jugadorUno = Instantiate(prefabPlayerUno, puntoSpawn.position, spawnPointsPlayers[i].transform.rotation);
        jugadoresVivos.Add(jugadorUno);

        if (jugadorUno.TryGetComponent<MazeMovement>(out var scriptMovimiento))
        {
            scriptMovimiento.ConectarMando(mando);
            // Configurar los ángulos según el spawn point
           
        }
    } 
    
    private void SpawnearJugadorDos(int idJugador, PlayerInputHandler mando, int i )
    {
        List<GameObject> nuevaPareja = new List<GameObject>();
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsPlayers[indexSpawn];
        
        jugadorDos = Instantiate(prefabPlayerDos, puntoSpawn.position, spawnPointsPlayers[i].transform.rotation );
        
            nuevaPareja.Add(jugadorUno);
            nuevaPareja.Add(jugadorDos);
            
            parejas.Add(nuevaPareja);
            
        
        if (jugadorDos.TryGetComponent<MazeMovement>(out var scriptMovimiento))
        {
            scriptMovimiento.ConectarMando(mando);
            // Configurar los ángulos según el spawn point
            
        }
    }
}