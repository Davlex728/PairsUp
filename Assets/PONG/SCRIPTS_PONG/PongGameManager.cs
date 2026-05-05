using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class PongGameManager : MonoBehaviour
{
    public static PongGameManager Instance; // Singleton para llamarlo desde cualquier sitio

    [Header("Prefab Padre (invisible, lleva movimiento)")]
    public GameObject prefabPadre;

    [Header("Animators para cada jugador (en orden de slot)")]
    public RuntimeAnimatorController[] animators;

    [Header("Prefab Hijo (el círculo visible)")]
    public GameObject prefabCirculo;

    public Vector2 offsetHijo;

    [Header("Spawn Points")] public Transform[] spawnPointsPlayers;

    [Header("Ángulos de rotación por spawn point")]
    public float[] angulosMinimos; // Array con el ángulo mínimo para cada spawn

    public float[] angulosMaximos; // Array con el ángulo máximo para cada spawn
    
    public List<GameObject> jugadoresVivos = new List<GameObject>();
    private GameObject jugadorInstanciado;
    private bool partidaTerminada = false;
    public String[] escenasAleatorias;

    private GameObject jugadorUno;
    private GameObject jugadorDos;

    List<List<GameObject>> parejas = new List<List<GameObject>>();


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
        Array.Sort(jugadoresConectados, (a, b) => a.playerIndex.CompareTo(b.playerIndex));
        foreach (PersistentPlayer mandoFantasma in jugadoresConectados)
        {
            Debug.Log("Procesando jugador: " + mandoFantasma.name);
            PlayerInputHandler lectorBotones = mandoFantasma.GetComponent<PlayerInputHandler>();

            int slotId = mandoFantasma.playerIndex;
            int equipo = mandoFantasma.teamIndex;

            bool esJugadorUno = (slotId % 2 != 0);

            if (esJugadorUno)
            {
                SpawnearJugadorUno(slotId, equipo, lectorBotones);
                Debug.Log(
                    $"Spawnereando jugador 1 {contadorJugadores} con mando {lectorBotones.name} en spawn point {slotId-1}");
                jugadorUno.transform.SetParent(spawnPointsPlayers[slotId-1].transform);
            }
            else
            {
                SpawnearJugadorDos(slotId, equipo, lectorBotones);
                Debug.Log(
                    $"Spawnereando jugador 1 {contadorJugadores} con mando {lectorBotones.name} en spawn point {slotId-1}");
                jugadorDos.transform.SetParent(spawnPointsPlayers[slotId-1].transform);
                //jugadorDos.transform.SetParent(jugadorUno.transform);
            }

           
            contadorJugadores++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ComprobarGanador();
    }

    void LateUpdate()
    {
        foreach (var pareja in parejas)
        {
            if (pareja[0].TryGetComponent<PongMovement>(out var jugA) &&
                pareja[1].TryGetComponent<PongMovement>(out var jugB))
            {
                float inputA = jugA.mandoMovimiento.moveInput.x;
                float inputB = jugB.mandoMovimiento.moveInput.x;

                if (inputA != 0 && inputB != 0 && Mathf.Sign(inputA) == Mathf.Sign(inputB))
                {
                    jugA.inputEfectivo = inputA;
                    jugB.inputEfectivo = inputB;
                }
                else
                {
                    jugA.inputEfectivo = 0f;
                    jugB.inputEfectivo = 0f;
                }
            }
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

    private void SpawnearJugadorUno(int slotId, int equipo, PlayerInputHandler mando)
    {
        Debug.Log("Spawneando jugador padre");
        int indexSpawn = (slotId - 1) / 2;
        Transform puntoSpawn = spawnPointsPlayers[indexSpawn];
<<<<<<< Updated upstream
        jugadorUno = Instantiate(prefabPadre, puntoSpawn.position, spawnPointsPlayers[i].transform.rotation);
=======
        jugadorUno = Instantiate(prefabPadre, spawnPointsPlayers[slotId]/*, spawnPointsPlayers[slotId].transform.rotation*/);
        Debug.Log($"slotId : {slotId}");
        jugadorUno.transform.position = spawnPointsPlayers[slotId-1 ].transform.position /*+ spawnPointsPlayers[slotId].transform.rotation * Vector3.up /** 10*/;
        Debug.Log(jugadorUno.transform.position);

>>>>>>> Stashed changes
        jugadoresVivos.Add(jugadorUno);

        if (jugadorUno.TryGetComponent<PongMovement>(out var scriptMovimiento))
        {
            scriptMovimiento.ConectarMando(mando);
            // Configurar los ángulos según el spawn point
            if (indexSpawn < angulosMinimos.Length && indexSpawn < angulosMaximos.Length)
            {
                scriptMovimiento.ConfigurarAngulos(angulosMinimos[indexSpawn], angulosMaximos[indexSpawn]);
            }
        }
    }

    private void SpawnearJugadorDos(int slotId, int equipo, PlayerInputHandler mando)
    {
        List<GameObject> nuevaPareja = new List<GameObject>();
        int indexSpawn = (slotId - 1) / 2;
        Transform puntoSpawn = spawnPointsPlayers[indexSpawn];

<<<<<<< Updated upstream
        jugadorDos = Instantiate(prefabCirculo, puntoSpawn.position + (Vector3)offsetHijo,
            spawnPointsPlayers[i].transform.rotation);
=======
        jugadorDos = Instantiate(prefabCirculo, puntoSpawn.position + (Vector3)offsetHijo, spawnPointsPlayers[slotId - 1].transform.rotation);

>>>>>>> Stashed changes

        nuevaPareja.Add(jugadorUno);
        nuevaPareja.Add(jugadorDos);

        parejas.Add(nuevaPareja);


        if (jugadorDos.TryGetComponent<PongMovement>(out var scriptMovimiento))
        {
            scriptMovimiento.ConectarMando(mando);
            // Configurar los ángulos según el spawn point
            if (indexSpawn < angulosMinimos.Length && indexSpawn < angulosMaximos.Length)
            {
                scriptMovimiento.ConfigurarAngulos(angulosMinimos[indexSpawn], angulosMaximos[indexSpawn]);
            }
        }
        AsignarAnimator(jugadorDos,slotId, equipo);
    }

    private void AsignarAnimator(GameObject obj, int slotId, int equipo)
    {
        Animator[] soloHijos = System.Array.FindAll(
            obj.GetComponentsInChildren<Animator>(), 
            a => a.gameObject != obj
        );

        // equipo 0 → índices 0,1 | equipo 1 → índices 2,3 | equipo 2 → índices 4,5
        int baseIndex = equipo * soloHijos.Length;

        for (int i = 0; i < soloHijos.Length; i++)
        {
            int animIndex = baseIndex + i;

            if (animIndex < animators.Length && animators[animIndex] != null)
                soloHijos[i].runtimeAnimatorController = animators[animIndex];
            else
                Debug.LogWarning($"[PongGameManager] No hay animator para slot {slotId} hijo {i} (índice {animIndex})");
        }
    
    }
}