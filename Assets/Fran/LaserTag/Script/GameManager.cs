using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

    public struct DatosEquipo
    {
        public Gamepad movimiento;
        public Gamepad disparo;
        public GameObject player;
    }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private List<DatosEquipo> equipos = new List<DatosEquipo>();

    private bool todosCompletos = true;

    private void Awake()
    {
        
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void RegistrarEquipo(Gamepad mandoMov, Gamepad mandoDisparo, GameObject jugador)
    {
        DatosEquipo equipo = new DatosEquipo();
        equipo.movimiento = mandoMov;
        equipo.disparo = mandoDisparo;
        equipo.player = jugador;
        equipos.Add(equipo);
        
    }
    
    public DatosEquipo ObtenerDatosEquipo(int indice)
    {
        return equipos[indice];
    }
}
