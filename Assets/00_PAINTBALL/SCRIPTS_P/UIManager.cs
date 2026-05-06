using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<GameObject> listaCorazonesAzules;
    public List<GameObject> listaCorazonesRojos;
    public List<GameObject> listaCorazonesAmarillos;

    // Cola de listas: la primera disponible se asigna al siguiente jugador
    private Queue<List<GameObject>> corazonesDisponibles;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        

        // Se encolan en orden: jugador 1, 2, 3
        corazonesDisponibles = new Queue<List<GameObject>>();
        corazonesDisponibles.Enqueue(listaCorazonesAzules);
        corazonesDisponibles.Enqueue(listaCorazonesRojos);
        corazonesDisponibles.Enqueue(listaCorazonesAmarillos);
    }

    // El LaserTagManager llama esto al spawnear cada jugador
    public List<GameObject> ReclamarCorazones()
    {
        if (corazonesDisponibles.Count == 0)
        {
            Debug.LogWarning("[UIManager] No quedan corazones disponibles.");
            return null;
        }
        return corazonesDisponibles.Dequeue();
    }

    public void ActualizarCorazones(List<GameObject> corazones, int vidasRestantes)
    {
        if (corazones == null)
        {
            Debug.LogWarning("[UIManager] ActualizarCorazones: lista null, ignorando.");
            return;
        }

        for (int i = 0; i < corazones.Count; i++)
        {
            corazones[i].SetActive(i < vidasRestantes);
        }
    }
}