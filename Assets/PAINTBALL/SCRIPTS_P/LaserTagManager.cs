using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaserTagManager : MonoBehaviour
{
    public static LaserTagManager Instance; // Singleton para llamarlo desde cualquier sitio
    
    [Header("Animators para cada jugador (en orden de slot)")]
    public RuntimeAnimatorController[] animators;
    
    
    [Header("Prefab Padre (invisible, lleva movimiento)")]
    public GameObject prefabPadre;

    [Header("Prefab Hijo (el círculo visible)")]
    public GameObject prefabCirculo;
    public Vector2 offsetHijo;

    [Header("Spawn Points")]
    public Transform[] spawnPointsPlayers;

    private List<GameObject> padresVivos = new List<GameObject>();
    private GameObject padreInstanciado;
    private GameObject hijo;
    private bool partidaTerminada = false;
    public String[] escenasAleatorias;
    [Header("Players")]
    public List<GameObject> jugadoresVivos = new List<GameObject>();

    private GameObject jugadorUno;
    private GameObject jugadorDos;
    
    List<List<GameObject>> parejas = new List<List<GameObject>>();
    public Transform puntoSpawn;
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

        Array.Sort(jugadoresConectados, (a, b) => a.playerIndex.CompareTo(b.playerIndex));

        foreach (PersistentPlayer mandoFantasma in jugadoresConectados)
        {
            Debug.Log(contadorJugadores);
            PlayerInputHandler lectorBotones = mandoFantasma.GetComponent<PlayerInputHandler>();
            
            int slotId = mandoFantasma.playerIndex;  // 1-6, asignado en la lobby
            int equipo = mandoFantasma.teamIndex;    
            
            bool esJugadorUno = (slotId % 2 != 0);
            
            if (esJugadorUno)
                SpawnearPadre(slotId, equipo, lectorBotones);
            else
            {
                Debug.Log("Hola hijo");
                SpawnearHijo(slotId, equipo, lectorBotones);
            }
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

    private void SpawnearPadre(int slotId, int equipo,PlayerInputHandler mando)
    {
        
        padreInstanciado = Instantiate(prefabPadre, spawnPointsPlayers[slotId].position, Quaternion.identity);
        padresVivos.Add(padreInstanciado);

        if (padreInstanciado.TryGetComponent<Movement>(out var movement))
        {
            movement.ConectarMando(mando);
            if (UIManager.instance == null)
            {
                Debug.LogError("[LaserTagManager] UIManager.instance es null. ¿Está en la escena?");
                return;
            }
            List<GameObject> corazones = UIManager.instance.ReclamarCorazones();
            movement.AsignarCorazones(corazones);
        }
        AsignarAnimator(padreInstanciado, slotId);
                  
    }

    private void SpawnearHijo(int slotId, int equipo ,PlayerInputHandler mando)
    {
        if (padreInstanciado == null)
        {
            Debug.LogWarning("[LaserTagManager] No hay padre instanciado para este hijo.");
            return;
        }

        Debug.Log("Hola");
        List<GameObject> nuevaPareja = new List<GameObject>();
         hijo = Instantiate(prefabCirculo, padreInstanciado.transform.position + (Vector3)offsetHijo, Quaternion.identity);
        hijo.transform.SetParent(padreInstanciado.transform);
        
        jugadoresVivos.Add(hijo);
        nuevaPareja.Add(padreInstanciado);
        nuevaPareja.Add(hijo);
        
        if (hijo.TryGetComponent<Aiming>(out var aiming))
            aiming.ConectarMando(mando);

        if (hijo.TryGetComponent<Shoot>(out var shoot))
            shoot.ConectarMando(mando);

        padreInstanciado = null;
        AsignarAnimator(hijo,slotId);
    }
    
    private void AsignarAnimator(GameObject obj, int slotId)
    {
        int animIndex = slotId - 1; // slot 1 → índice 0
        if (obj.TryGetComponent(out Animator animator))
        {
           if(animators.Length > animIndex && animators[animIndex] != null)
               animator.runtimeAnimatorController = animators[animIndex];
           else
               Debug.LogWarning($"[CalderoManager] No hay animator para slot {slotId} (índice {animIndex})");
           if (animator == null)
           {
               if (obj.GetComponentInChildren<Animator>())
               {
                   if(animators.Length > animIndex && animators[animIndex] != null)
                       animator.runtimeAnimatorController = animators[animIndex];
               }

           }
        }
        
    }
}