using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaserTagManager : MonoBehaviour
{
    public static LaserTagManager Instance; // Singleton para llamarlo desde cualquier sitio

    [Header("Prefab Padre (invisible, lleva movimiento)")]
    public GameObject prefabPadre;

    [Header("Prefab Hijo (el círculo visible)")]
    public GameObject prefabCirculo;
    public Vector2 offsetHijo;

    [Header("Spawn Points")]
    public Transform[] spawnPointsPlayers;

    private List<GameObject> padresVivos = new List<GameObject>();
    private GameObject padreInstanciado;
    private bool partidaTerminada = false;
    public String[] escenasAleatorias;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PersistentPlayer[] jugadoresConectados = FindObjectsByType<PersistentPlayer>(FindObjectsSortMode.None);

        if (jugadoresConectados.Length == 0)
        {
            Debug.LogWarning("[LaserTagManager] No hay jugadores. ¿Has pasado por el lobby?");
            return;
        }

        int contadorJugadores = 0;

        foreach (PersistentPlayer mandoFantasma in jugadoresConectados)
        {
            PlayerInputHandler lectorBotones = mandoFantasma.GetComponent<PlayerInputHandler>();

            if (contadorJugadores % 2 == 0)
                SpawnearPadre(contadorJugadores, lectorBotones);
            else
                SpawnearHijo(contadorJugadores, lectorBotones);

            contadorJugadores++;
        }
    }
    private void Update()
    {
        ComprobarGanador();
    }
    // Llama a este método desde cualquier script cuando muera un jugador
    public void ComprobarGanador()
    {
        if (partidaTerminada) return;

        padresVivos.RemoveAll(p => p == null);

        if (padresVivos.Count == 1)
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
            Debug.Log($"[LaserTagManager] ¡Ha ganado la pareja: {padresVivos[0].name}!");
        }
        else if (padresVivos.Count == 0)
        {
            partidaTerminada = true;
            Debug.Log("[LaserTagManager] ¡Empate! No quedan parejas.");
        }
    }

    private void SpawnearPadre(int idJugador, PlayerInputHandler mando)
    {
        int j = 0;
        int indexSpawn = idJugador / 2;
        Transform puntoSpawn = spawnPointsPlayers[indexSpawn];

        padreInstanciado = Instantiate(prefabPadre, puntoSpawn.position, Quaternion.identity);
        padresVivos.Add(padreInstanciado);

        if (padreInstanciado.TryGetComponent<Movement>(out var movement))
        {
            movement.ConectarMando(mando);
            List<GameObject> corazones = UIManager.instance.ReclamarCorazones();
            movement.AsignarCorazones(corazones);
        }
                  
    }

    private void SpawnearHijo(int idJugador, PlayerInputHandler mando)
    {
        if (padreInstanciado == null)
        {
            Debug.LogWarning("[LaserTagManager] No hay padre instanciado para este hijo.");
            return;
        }

        GameObject hijo = Instantiate(prefabCirculo, padreInstanciado.transform.position + (Vector3)offsetHijo, Quaternion.identity);
        hijo.transform.SetParent(padreInstanciado.transform);

        if (hijo.TryGetComponent<Aiming>(out var aiming))
            aiming.ConectarMando(mando);

        if (hijo.TryGetComponent<Shoot>(out var shoot))
            shoot.ConectarMando(mando);

        padreInstanciado = null;
    }
}